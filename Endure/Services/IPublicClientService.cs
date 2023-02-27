using Microsoft.Identity.Client;

namespace Endure.Services;

public interface IPublicClientService
{
    Task SignOutAsync();

    Task<string> AcquireTokenSilentAsync(string[] scopes);

    Task<AuthenticationResult> AcquireTokenInteractiveAsync(string[] scopes);

    string[] GetScopes();
}