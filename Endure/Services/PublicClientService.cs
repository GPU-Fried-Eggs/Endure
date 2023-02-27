using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Endure.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Extensions.Msal;
using Microsoft.IdentityModel.Abstractions;

namespace Endure.Services;

public class IdentityLogger : IIdentityLogger
{
    private readonly EventLogLevel m_minLogLevel;

    public IdentityLogger(EventLogLevel minLogLevel = EventLogLevel.LogAlways) => m_minLogLevel = minLogLevel;

    public bool IsEnabled(EventLogLevel eventLogLevel) => eventLogLevel >= m_minLogLevel;

    public void Log(LogEntry entry) => Debug.WriteLine($"MSAL: EventLogLevel: {entry.EventLogLevel}, Message: {entry.Message} ");
}

public class PlatformConfig
{
    public static PlatformConfig Instance { get; } = new();

    /// <summary>
    /// Platform specific Redirect URI
    /// </summary>
    public string RedirectUri { get; set; } = $"msal{PublicClientService.Instance.MsalClientHandler.AzureAdb2C.ClientId}://auth";

    /// <summary>
    /// Platform specific parent window
    /// </summary>
    public object ParentWindow { get; set; }
}

public class MSALClientHandler
{
    public AzureADB2C? AzureAdb2C;
    
    public AuthenticationResult AuthResult { get; private set; }

    public IPublicClientApplication? PublicClientApplication { get; private set; }

    public bool UseEmbedded { get; set; } = false;

    private PublicClientApplicationBuilder m_publicClientApplicationBuilder;

    public MSALClientHandler(AzureADB2C azureAdb2C)
    {
        AzureAdb2C = azureAdb2C;
        
        m_publicClientApplicationBuilder = PublicClientApplicationBuilder.Create(azureAdb2C.ClientId)
            .WithExperimentalFeatures()
            .WithB2CAuthority($"{azureAdb2C.Instance}/tfp/{azureAdb2C.Domain}/{azureAdb2C.SignUpSignInPolicyid}")
            .WithLogging(new IdentityLogger(EventLogLevel.Warning), enablePiiLogging: false);
    }

    /// <summary>
    /// Initializes the public client application of MSAL.NET with the required information to correctly authenticate the user.
    /// </summary>
    /// <returns></returns>
    public async Task<IAccount> InitializePublicClientAppAsync()
    {
        // Initialize the MSAL library by building a public client application
        PublicClientApplication = m_publicClientApplicationBuilder
            .WithRedirectUri(PlatformConfig.Instance.RedirectUri)
            .Build();

        //await AttachTokenCache();

        return await FetchSignedInUserFromCache().ConfigureAwait(false);
    }

    /// <summary>
    /// Attaches the token cache to the Public Client app.
    /// </summary>
    /// <returns>IAccount list of already signed-in users (if available)</returns>
    private async Task<IEnumerable<IAccount>> AttachTokenCache()
    {
        if (DeviceInfo.Current.Platform != DevicePlatform.WinUI)
        {
            return null;
        }

        // Cache configuration and hook-up to public application. Refer to https://github.com/AzureAD/microsoft-authentication-extensions-for-dotnet/wiki/Cross-platform-Token-Cache#configuring-the-token-cache
        var storageProperties = new StorageCreationPropertiesBuilder(AzureAdb2C.CacheFileName, FileSystem.Current.CacheDirectory)
            .Build();

        var msalcachehelper = await MsalCacheHelper.CreateAsync(storageProperties);
        msalcachehelper.RegisterCache(PublicClientApplication.UserTokenCache);

        // If the cache file is being reused, we'd find some already-signed-in accounts
        return await PublicClientApplication.GetAccountsAsync().ConfigureAwait(false);
    }
    
    /// <summary>
    /// Signs in the user and obtains an Access token for a provided set of scopes
    /// </summary>
    /// <param name="scopes"></param>
    /// <returns> Access Token</returns>
    public async Task<string> SignInUserAndAcquireAccessToken(string[] scopes)
    {
        var existingUser = await FetchSignedInUserFromCache().ConfigureAwait(false);

        try
        {
            // 1. Try to sign-in the previously signed-in account
            if (existingUser != null)
            {
                AuthResult = await PublicClientApplication
                    .AcquireTokenSilent(scopes, existingUser)
                    .ExecuteAsync()
                    .ConfigureAwait(false);
            }
            else
            {
                AuthResult = await SignInUserInteractivelyAsync(scopes);
            }
        }
        catch (MsalUiRequiredException ex)
        {
            // A MsalUiRequiredException happened on AcquireTokenSilentAsync. This indicates you need to call AcquireTokenInteractive to acquire a token interactively
            Debug.WriteLine($"MsalUiRequiredException: {ex.Message}");

            AuthResult = await PublicClientApplication
                .AcquireTokenInteractive(scopes)
                .ExecuteAsync()
                .ConfigureAwait(false);
        }
        catch (MsalException msalEx)
        {
            Debug.WriteLine($"Error Acquiring Token interactively:{Environment.NewLine}{msalEx}");
        }

        return AuthResult.AccessToken;
    }

    /// <summary>
    /// Signs the in user and acquire access token for a provided set of scopes.
    /// </summary>
    /// <param name="scopes">The scopes.</param>
    /// <param name="extraclaims">The extra claims, usually from CAE. We basically handle CAE by sending the user back to Azure AD for
    /// additional processing and requesting a new access token for Graph</param>
    /// <returns></returns>
    public async Task<String> SignInUserAndAcquireAccessToken(IEnumerable<string> scopes, string extraclaims)
    {
        try
        {
            // Send the user to Azure AD for re-authentication as a silent acquisition wont resolve any CAE scenarios like an extra claims request
            AuthResult = await PublicClientApplication.AcquireTokenInteractive(scopes)
                .WithClaims(extraclaims)
                .ExecuteAsync()
                .ConfigureAwait(false);
        }
        catch (MsalException msalEx)
        {
            Debug.WriteLine($"Error Acquiring Token:{Environment.NewLine}{msalEx}");
        }

        return AuthResult.AccessToken;
    }

    /// <summary>
    /// Shows a pattern to sign-in a user interactively in applications that are input constrained and would need to fall-back on device code flow.
    /// </summary>
    /// <param name="scopes">The scopes.</param>
    /// <param name="existingAccount">The existing account.</param>
    /// <returns></returns>
    public async Task<AuthenticationResult> SignInUserInteractivelyAsync(string[] scopes, IAccount existingAccount = null)
    {
        if (PublicClientApplication == null) throw new NullReferenceException();

        if (PublicClientApplication.IsUserInteractive())
        {
            return await PublicClientApplication.AcquireTokenInteractive(scopes)
                .WithParentActivityOrWindow(PlatformConfig.Instance.ParentWindow)
                .ExecuteAsync()
                .ConfigureAwait(false);
        }

        // If the operating system does not have UI (e.g. SSH into Linux), you can fallback to device code, however this
        // flow will not satisfy the "device is managed" CA policy.
        return await PublicClientApplication.AcquireTokenWithDeviceCode(scopes, (dcr) =>
        {
            Console.WriteLine(dcr.Message);
            return Task.CompletedTask;
        }).ExecuteAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Removes the first signed-in user's record from token cache
    /// </summary>
    public async Task SignOutUserAsync()
    {
        var existingUser = await FetchSignedInUserFromCache().ConfigureAwait(false);
        await SignOutUserAsync(existingUser).ConfigureAwait(false);
    }

    /// <summary>
    /// Removes a given user's record from token cache
    /// </summary>
    /// <param name="user">The user.</param>
    public async Task SignOutUserAsync(IAccount user)
    {
        if (PublicClientApplication == null) return;

        await PublicClientApplication.RemoveAsync(user).ConfigureAwait(false);
    }

    public async Task<IAccount> FetchSignedInUserFromCache()
    {
        // get accounts from cache
        IEnumerable<IAccount> accounts = await PublicClientApplication.GetAccountsAsync();

        // Error corner case: we should always have 0 or 1 accounts, not expecting > 1
        // This is just an example of how to resolve this ambiguity, which can arise if more apps share a token cache.
        // Note that some apps prefer to use a random account from the cache.
        if (accounts.Count() > 1)
        {
            foreach (var acc in accounts)
                await PublicClientApplication.RemoveAsync(acc);

            return null;
        }

        return accounts.SingleOrDefault();
    }
}

public class PublicClientService : IPublicClientService
{
    public static PublicClientService Instance => new(); 

    private static IConfiguration m_configuration;

    public MSALClientHandler MsalClientHandler;

    private DownstreamApi? m_downstreamApi;

    public bool UseEmbedded { get; set; } = false;

    [MethodImpl(MethodImplOptions.NoInlining)]
    private PublicClientService()
    {
        var embeddedConfigFileName = $"{Assembly.GetCallingAssembly().GetName().Name}.appsettings.json";
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedConfigFileName);
        m_configuration = new ConfigurationBuilder().AddJsonStream(stream ?? throw new ApplicationException("configure file missing")).Build();

        var azureAdb2C = m_configuration.GetSection("AzureAdB2C").Get<AzureADB2C>();
        MsalClientHandler = new MSALClientHandler(azureAdb2C);

        m_downstreamApi = m_configuration.GetSection("DownstreamApi").Get<DownstreamApi>();
    }
    
    /// <summary>
    /// Acquire the token silently
    /// </summary>
    /// <returns>An access token</returns>
    public async Task<string> AcquireTokenSilentAsync()
    {
        // Get accounts by policy
        return await AcquireTokenSilentAsync(GetScopes()).ConfigureAwait(false);
    }

    /// <summary>
    /// Acquire the token silently
    /// </summary>
    /// <param name="scopes">desired scopes</param>
    /// <returns>An access token</returns>
    public async Task<string> AcquireTokenSilentAsync(string[] scopes)
    {
        return await MsalClientHandler.SignInUserAndAcquireAccessToken(scopes).ConfigureAwait(false);
    }

    /// <summary>
    /// Perform the interactive acquisition of the token for the given scope
    /// </summary>
    /// <param name="scopes">desired scopes</param>
    /// <returns></returns>
    public async Task<AuthenticationResult> AcquireTokenInteractiveAsync(string[] scopes)
    {
        MsalClientHandler.UseEmbedded = UseEmbedded;
        return await MsalClientHandler.SignInUserInteractivelyAsync(scopes).ConfigureAwait(false);
    }

    /// <summary>
    /// It will sign out the user.
    /// </summary>
    /// <returns></returns>
    public async Task SignOutAsync()
    {
        await MsalClientHandler.SignOutUserAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Gets scopes for the application
    /// </summary>
    /// <returns>An array of all scopes</returns>
    public string[] GetScopes()
    {
        return m_downstreamApi?.ScopesArray ?? new []{ string.Empty };
    }
}