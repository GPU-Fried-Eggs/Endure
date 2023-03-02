using System.Diagnostics;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Abstractions;

namespace Endure.Services;

public class IdentityLogger : IIdentityLogger
{
    private readonly EventLogLevel m_minLogLevel;

    public IdentityLogger(EventLogLevel minLogLevel = EventLogLevel.LogAlways) => m_minLogLevel = minLogLevel;

    public bool IsEnabled(EventLogLevel eventLogLevel) => eventLogLevel >= m_minLogLevel;

    public void Log(LogEntry entry) => Debug.WriteLine($"MSAL: EventLogLevel: {entry.EventLogLevel}, Message: {entry.Message} ");
}

public class PublicClientService : IPublicClientService
{
    public static PublicClientService Instance { get; } = new();

    public PublicClientApplicationBuilder PublicClientApplicationBuilder { get; }

    public IPublicClientApplication? PublicClientApplication { get; set; }

    public AuthenticationResult? AuthResult { get; private set; }

    public object? ParentWindow { get; set; }

    public PublicClientService()
    {
        PublicClientApplicationBuilder = PublicClientApplicationBuilder.Create(Constants.ClientId)
            .WithExperimentalFeatures()
            .WithB2CAuthority($"https://{Constants.Host}/tfp/{Constants.Domain}/{Constants.SignUpSignInPolicyId}")
            .WithLogging(new IdentityLogger(EventLogLevel.Warning), enablePiiLogging: false);
    }

    public void CreatePlatformInstance(object parent, string redirectUri)
    {
        ParentWindow = parent;
        PublicClientApplication = PublicClientApplicationBuilder
            .WithRedirectUri(redirectUri)
            .Build();
    }

    public async Task<string?> AcquireTokenSilentAsync(string[] scopes)
    {
        if (PublicClientApplication is null) return null;

        var existingUser = await GetAccountFromCacheAsync().ConfigureAwait(false);

        try
        {
            AuthResult = existingUser is not null
                ? await PublicClientApplication
                    .AcquireTokenSilent(scopes, existingUser)
                    .ExecuteAsync()
                    .ConfigureAwait(false)
                : await AcquireTokenInteractiveAsync(scopes);
        }
        catch (MsalUiRequiredException msalUiRequiredException)
        {
            // A MsalUiRequiredException happened on AcquireTokenSilentAsync. This indicates you need to call AcquireTokenInteractive to acquire a token interactively
            Debug.WriteLine($"MsalUiRequiredException: {msalUiRequiredException.Message}");

            AuthResult = await PublicClientApplication
                .AcquireTokenInteractive(scopes)
                .ExecuteAsync()
                .ConfigureAwait(false);
        }
        catch (MsalException msalException)
        {
            Debug.WriteLine($"Error Acquiring Token interactively:{Environment.NewLine}{msalException}");
        }

        return AuthResult?.AccessToken;
    }

    public async Task<AuthenticationResult?> AcquireTokenInteractiveAsync(IEnumerable<string> scopes)
    {
        if (PublicClientApplication is null) return null;
        
        if (PublicClientApplication.IsUserInteractive())
        {
            return await PublicClientApplication.AcquireTokenInteractive(scopes)
                .WithParentActivityOrWindow(ParentWindow)
                .ExecuteAsync()
                .ConfigureAwait(false);
        }

        // If the operating system does not have UI (e.g. SSH into Linux), you can fallback to device code, however this
        // flow will not satisfy the "device is managed" CA policy.
        return await PublicClientApplication.AcquireTokenWithDeviceCode(scopes, dcr =>
        {
            Console.WriteLine(dcr.Message);
            return Task.CompletedTask;
        }).ExecuteAsync().ConfigureAwait(false);
    }

    public async Task<IAccount?> GetAccountFromCacheAsync()
    {
        if (PublicClientApplication is null) return null;

        // get accounts from cache
        var accounts = await PublicClientApplication.GetAccountsAsync();

        // Error corner case: we should always have 0 or 1 accounts, not expecting > 1
        // This is just an example of how to resolve this ambiguity, which can arise if more apps share a token cache.
        // Note that some apps prefer to use a random account from the cache.
        var enumerable = accounts as IAccount[] ?? accounts.ToArray();

        if (enumerable.Length > 1)
        {
            foreach (var account in enumerable)
                await PublicClientApplication.RemoveAsync(account);

            return null;
        }

        return enumerable.SingleOrDefault();
    }

    public async Task SignOutAsync()
    {
        if (PublicClientApplication is null) return;

        var existingUser = await GetAccountFromCacheAsync().ConfigureAwait(false);

        await PublicClientApplication.RemoveAsync(existingUser).ConfigureAwait(false);
    }
}