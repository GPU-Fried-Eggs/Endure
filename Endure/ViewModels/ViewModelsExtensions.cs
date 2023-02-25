using Microsoft.Maui.LifecycleEvents;

namespace Endure.ViewModels;

public static class ViewModelsExtensions
{
    public static MauiAppBuilder ConfigureViewModels(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<ReviewViewModel>();
        builder.Services.AddSingleton<SettingsViewModel>();

        return builder;
    }

    public static void RegisterLifecycleEvent(ILifecycleBuilder events) { }
}