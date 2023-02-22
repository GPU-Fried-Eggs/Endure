#if WINDOWS
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.Maui.LifecycleEvents;
using Endure.ViewModels;
using System.Runtime.InteropServices;
using Windows.System;
using WinRT;
using Window = Microsoft.UI.Xaml.Window;

namespace Endure.Services;

public class DispatcherQueueHelper
{
    [StructLayout(LayoutKind.Sequential)]
    private struct DispatcherQueueOptions
    {
        internal int dwSize;
        internal int threadType;
        internal int apartmentType;
    }

    [DllImport("CoreMessaging.dll")]
    private static extern unsafe int CreateDispatcherQueueController(DispatcherQueueOptions options, nint* dispatcherQueueController);

    private nint m_dispatcherQueueController = nint.Zero;

    public void EnsureWindowsSystemDispatcherQueueController()
    {
        if (DispatcherQueue.GetForCurrentThread() != null) return;

        if (m_dispatcherQueueController == nint.Zero)
        {
            DispatcherQueueOptions options;
            options.dwSize = Marshal.SizeOf(typeof(DispatcherQueueOptions));
            options.threadType = 2;    // DQTYPE_THREAD_CURRENT
            options.apartmentType = 2; // DQTAT_COM_STA

            unsafe
            {
                var dispatcherQueueController = nint.Zero;
                CreateDispatcherQueueController(options, &dispatcherQueueController);
                m_dispatcherQueueController = dispatcherQueueController;
            }
        }
    }
}

public class BackdropService
{
    private ISystemBackdropControllerWithTargets? m_controller;
    private readonly SystemBackdropConfiguration? m_configuration;
    private readonly Window m_window;

    public BackdropService(Window window)
    {
        m_window = window;
        var dispatcherQueueHelper = new DispatcherQueueHelper();
        dispatcherQueueHelper.EnsureWindowsSystemDispatcherQueueController();

        m_configuration = new SystemBackdropConfiguration { IsInputActive = true };

        switch (App.Current.BackdropStyle)
        {
            case BackdropStyle.Mica:
            default:
                TrySetMicaBackdrop();
                break;
            case BackdropStyle.Acrylic:
                TrySetAcrylicBackdrop();
                break;
        }
    }

    private void WindowActivated(object sender, WindowActivatedEventArgs args)
    {
        if (m_configuration != null)
        {
            m_configuration.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
        }
    }

    private void WindowClosed(object sender, WindowEventArgs args)
    {
        if (m_controller != null)
        {
            m_controller.Dispose();
            m_controller = null;
        }

        m_window.Activated -= WindowActivated;
    }

    private void WindowThemeChanged(AppTheme theme)
    {
        if (m_configuration != null)
        {
            m_configuration.Theme = theme switch
            {
                AppTheme.Light => SystemBackdropTheme.Light,
                AppTheme.Dark => SystemBackdropTheme.Dark,
                _ => SystemBackdropTheme.Default,
            };
        }
    }

    private void WindowBackdropChanged(BackdropStyle style)
    {
        if (m_controller != null)
        {
            m_controller.Dispose();
            m_controller = null;
        }

        m_window.Activated -= WindowActivated;
        m_window.Closed -= WindowClosed;

        switch (App.Current.BackdropStyle)
        {
            case BackdropStyle.Mica:
            default:
                TrySetMicaBackdrop();
                break;
            case BackdropStyle.Acrylic:
                TrySetAcrylicBackdrop();
                break;
        }
    }

    private void TrySetMicaBackdrop()
    {
        if (MicaController.IsSupported())
        {
            m_window.Activated += WindowActivated;
            m_window.Closed += WindowClosed;

            WindowThemeChanged(App.Current.Theme);

            m_controller = new MicaController();
            m_controller.AddSystemBackdropTarget(m_window.As<ICompositionSupportsSystemBackdrop>());
            m_controller.SetSystemBackdropConfiguration(m_configuration);

            App.Current.ThemeChanged = WindowThemeChanged;
            App.Current.BackdropChanged = WindowBackdropChanged;
        }
    }

    private void TrySetAcrylicBackdrop()
    {
        if (DesktopAcrylicController.IsSupported())
        {
            m_window.Activated += WindowActivated;
            m_window.Closed += WindowClosed;

            WindowThemeChanged(App.Current.Theme);

            m_controller = new DesktopAcrylicController();
            m_controller.AddSystemBackdropTarget(m_window.As<ICompositionSupportsSystemBackdrop>());
            m_controller.SetSystemBackdropConfiguration(m_configuration);

            App.Current.ThemeChanged = WindowThemeChanged;
            App.Current.BackdropChanged = WindowBackdropChanged;
        }
    }
}
#endif

public static class BackdropServiceExtension
{
    public static MauiAppBuilder ConfigureBackdropBackground(this MauiAppBuilder builder)
    {
#if WINDOWS
        builder.ConfigureLifecycleEvents(events =>
        {
            events.AddWindows(wndLifeCycleBuilder =>
            {
                wndLifeCycleBuilder.OnWindowCreated(window =>
                {
                    var _ = new BackdropService(window);
                });
            });
        });
#endif
        return builder;
    }
}

