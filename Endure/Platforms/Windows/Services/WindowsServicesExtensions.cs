using Microsoft.Maui.LifecycleEvents;

namespace Endure.WinUI.Services;

public static class WindowsServicesExtensions
{
    public static void RegisterLifecycleEvent(ILifecycleBuilder events)
    {
#if WINDOWS
        events.AddWindows(windows =>
        {
            windows.OnWindowCreated(window =>
            {
                var service = new BackdropService(window);

                service.TrySetBackdrop(App.Current.BackdropStyle);
            });
        });
#endif
    }
}