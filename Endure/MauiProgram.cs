using Endure.Services;
using Endure.ViewModels;
using Endure.Views;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;

namespace Endure;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureAppServices()
            .ConfigureViewModels()
            .ConfigureViews()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Digital-Regular.ttf", "DigitalRegular");
            })
            .ConfigureLifecycleEvents(events =>
            {
#if WINDOWS
                WinUI.Services.WindowsServicesExtensions.RegisterLifecycleEvent(events);
#endif
                ViewModelsExtensions.RegisterLifecycleEvent(events);
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}