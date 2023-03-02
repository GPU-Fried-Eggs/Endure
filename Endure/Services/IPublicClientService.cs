using Microsoft.Identity.Client;

namespace Endure.Services;

public interface IPublicClientService
{
    Task<string?> AcquireTokenSilentAsync(string[] scopes);

    Task<AuthenticationResult?> AcquireTokenInteractiveAsync(IEnumerable<string> scopes);

    Task<IAccount?> GetAccountFromCacheAsync();

    Task SignOutAsync();
}