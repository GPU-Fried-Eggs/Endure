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
            .ConfigureServices()
            .ConfigureViews()
            .ConfigureViewModels()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Digital-Regular.ttf", "DigitalRegular");
            })
            .ConfigureLifecycleEvents(events =>
            {
                ServicesExtensions.RegisterLifecycleEvent(events);
                ViewModelsExtensions.RegisterLifecycleEvent(events);
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}