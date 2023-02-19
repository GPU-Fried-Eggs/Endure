#if ANDROID
using System.Net.Security;
using Xamarin.Android.Net;
#endif

namespace Endure.Services;

public class HttpsClientHandlerService : IHttpsClientHandlerService
{
    public HttpMessageHandler? GetPlatformMessageHandler()
    {
#if ANDROID
        var handler = new AndroidMessageHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                if (cert is { Issuer: "CN=localhost" })
                    return true;
                return errors == SslPolicyErrors.None;
            }
        };
        return handler;
#elif IOS
        var handler = new NSUrlSessionHandler
        {
            TrustOverrideForUrl = (sender, url, trust) =>
            {
                if (url.StartsWith("https://localhost"))
                    return true;
                return false;
            }
        };
        return handler;
#elif WINDOWS || MACCATALYST
        return null;
#else
        throw new PlatformNotSupportedException("Only Android, iOS, MacCatalyst, and Windows supported.");
#endif
    }
}


