using Microsoft.Maui.LifecycleEvents;

namespace Endure.Services;

public static class ServicesExtensions
{
    public static MauiAppBuilder ConfigureServices(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<IHttpsClientHandlerService, HttpsClientHandlerService>();
        builder.Services.AddSingleton<IMemoService, MemoService>();

        return builder;
    }

    public static void RegisterLifecycleEvent(ILifecycleBuilder events)
    {
#if WINDOWS
        events.AddWindows(windows => windows
            .OnWindowCreated(WindowsBackdropService.Create));
#endif
    }
}