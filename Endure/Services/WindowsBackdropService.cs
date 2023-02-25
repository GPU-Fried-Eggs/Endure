#if WINDOWS
using System.Runtime.InteropServices;
using Windows.System;
using Endure.ViewModels;
using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
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

public class WindowsBackdropService
{
    private ISystemBackdropControllerWithTargets? m_controller;
    private readonly SystemBackdropConfiguration? m_configuration;
    private readonly Window m_window;

    public WindowsBackdropService(Window window)
    {
        m_window = window;

        var dispatcherQueueHelper = new DispatcherQueueHelper();
        dispatcherQueueHelper.EnsureWindowsSystemDispatcherQueueController();

        m_configuration = new SystemBackdropConfiguration { IsInputActive = true };

        m_window.Activated += WindowActivated;
        m_window.Closed += WindowClosed;

        App.Current.RequestedThemeChanged += WindowThemeChanged;
        App.Current.BackdropChanged += WindowBackdropChanged;
    }

    public static void Create(Window window)
    {
        var service = new WindowsBackdropService(window);
        
        service.TrySetBackdrop(App.Current.BackdropStyle);
    }

    private void TrySetBackdrop(BackdropStyle style)
    {
        switch (style)
        {
            case BackdropStyle.Mica:
                TrySetMicaBackdrop();
                break;
            case BackdropStyle.Acrylic:
                TrySetAcrylicBackdrop();
                break;
            case BackdropStyle.None:
            default: break;
        }
    }

    private void TrySetMicaBackdrop()
    {
        if (MicaController.IsSupported())
        {
            WindowThemeChanged(null, new AppThemeChangedEventArgs(App.Current.Theme));

            m_controller = new MicaController();
            m_controller.AddSystemBackdropTarget(m_window.As<ICompositionSupportsSystemBackdrop>());
            m_controller.SetSystemBackdropConfiguration(m_configuration);
        }
    }

    private void TrySetAcrylicBackdrop()
    {
        if (DesktopAcrylicController.IsSupported())
        {
            WindowThemeChanged(null, new AppThemeChangedEventArgs(App.Current.Theme));

            m_controller = new DesktopAcrylicController();
            m_controller.AddSystemBackdropTarget(m_window.As<ICompositionSupportsSystemBackdrop>());
            m_controller.SetSystemBackdropConfiguration(m_configuration);
        }
    }

    private void WindowActivated(object? sender, WindowActivatedEventArgs args)
    {
        if (m_configuration != null)
        {
            m_configuration.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
        }
    }

    private void WindowClosed(object? sender, WindowEventArgs args)
    {
        if (m_controller != null)
        {
            m_controller.Dispose();
            m_controller = null;
        }

        m_window.Activated -= WindowActivated;
        m_window.Closed -= WindowClosed;

        App.Current.RequestedThemeChanged -= WindowThemeChanged;
        App.Current.BackdropChanged -= WindowBackdropChanged;
    }

    private void WindowThemeChanged(object? sender, AppThemeChangedEventArgs args)
    {
        if (m_configuration != null)
        {
            m_configuration.Theme = args.RequestedTheme switch
            {
                AppTheme.Light => SystemBackdropTheme.Light,
                AppTheme.Dark => SystemBackdropTheme.Dark,
                _ => SystemBackdropTheme.Default,
            };
        }
    }

    private void WindowBackdropChanged(object? sender, AppBackdropStyleChangedEventArgs args)
    {
        if (m_controller != null)
        {
            m_controller.Dispose();
            m_controller = null;
        }

        m_window.Activated -= WindowActivated;
        m_window.Closed -= WindowClosed;

        TrySetBackdrop(args.Style);
    }
}
#endif
