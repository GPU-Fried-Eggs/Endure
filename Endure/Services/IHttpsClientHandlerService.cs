namespace Endure.Services;

public interface IHttpsClientHandlerService
{
    HttpMessageHandler? GetPlatformMessageHandler();
}