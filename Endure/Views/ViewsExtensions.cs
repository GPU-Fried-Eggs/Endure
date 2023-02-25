namespace Endure.Views;

public static class ViewsExtensions
{
    public static MauiAppBuilder ConfigureViews(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<ReviewPage>();
        builder.Services.AddSingleton<SettingsPage>();

        return builder;
    }
}