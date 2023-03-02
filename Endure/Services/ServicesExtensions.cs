namespace Endure.Services;

public static class ServicesExtensions
{
    public static MauiAppBuilder ConfigureAppServices(this MauiAppBuilder builder)
    {
        // Azure Auth
        builder.Services.AddSingleton<IPublicClientService, PublicClientService>();

        // Api binding
        builder.Services.AddSingleton<IHttpsClientHandlerService, HttpsClientHandlerService>();
        builder.Services.AddSingleton<IMemoService, MemoService>();

        return builder;
    }
}