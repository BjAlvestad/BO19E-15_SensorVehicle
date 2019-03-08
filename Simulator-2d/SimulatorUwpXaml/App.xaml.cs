using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SimulatorUwpXaml.SensorVehicleApp_interface;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=402347&clcid=0x409

namespace SimulatorUwpXaml
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
{
    static string deviceFamily;

    public SimulatorAppServiceProvider AppServiceProvider { get; } = new SimulatorAppServiceProvider();

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        this.InitializeComponent();
        this.Suspending += OnSuspending;

        //API check to ensure the "RequiresPointerMode" property exists, ensuring project is running on build 14393 or later
        if (Windows.Foundation.Metadata.ApiInformation.IsPropertyPresent("Windows.UI.Xaml.Application", "RequiresPointerMode"))
        {
            //If running on the Xbox, disable the default on screen pointer
            if (IsXbox())
            {
                Application.Current.RequiresPointerMode = ApplicationRequiresPointerMode.WhenRequested;
            }
        }
    }

    /// <summary>
    /// Detection code in Windows 10 to identify the platform it is being run on
    /// This function returns true if the project is running on an XboxOne
    /// </summary>
    public static bool IsXbox()
    {
        if (deviceFamily == null)
            deviceFamily = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily;

        return deviceFamily == "Windows.Xbox";
    }

    /// <summary>
    /// Invoked when the application is launched normally by the end user.  Other entry points
    /// will be used such as when the application is launched to open a specific file.
    /// </summary>
    /// <param name="e">Details about the launch request and process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs e)
    {
        // By default we want to fill the entire core window.
        ApplicationView.GetForCurrentView().SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

        Frame rootFrame = Window.Current.Content as Frame;

        // Do not repeat app initialization when the Window already has content,
        // just ensure that the window is active
        if (rootFrame == null)
        {
            // Create a Frame to act as the navigation context and navigate to the first page
            rootFrame = new Frame();

            rootFrame.NavigationFailed += OnNavigationFailed;

            if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                //TODO: Load state from previously suspended application
            }

            // Place the frame in the current Window
            Window.Current.Content = rootFrame;
        }

        if (rootFrame.Content == null)
        {
            // When the navigation stack isn't restored navigate to the first page,
            // configuring the new page by passing required information as a navigation
            // parameter
            rootFrame.Navigate(typeof(GamePage), e.Arguments);
        }
        // Ensure the current window is active
        Window.Current.Activate();
    }

    /// <summary>
    /// OnActivated event handler receives all activation events.
    /// This is set up to handle Protocol activation events (i.e. simulator being launched from SensorVehicle application).
    /// More information on: https://docs.microsoft.com/en-us/windows/uwp/launch-resume/handle-uri-activation
    /// </summary>
    /// <param name="args"></param>
    protected override void OnActivated(IActivatedEventArgs args)
    {
        if (args.Kind == ActivationKind.Protocol)
        {
            ProtocolActivatedEventArgs eventArgs = args as ProtocolActivatedEventArgs;
            // TODO: Handle URI activation
            // The received URI is eventArgs.Uri.AbsoluteUri

            //TODO: The code below is copied from OnLaunched(). If this is not significantly changed by the completion of the software, the common code should be extracted.
            // By default we want to fill the entire core window.
            ApplicationView.GetForCurrentView().SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content, just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();
                rootFrame.NavigationFailed += OnNavigationFailed;
                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }
                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page, configuring the new page by passing required information as a navigation parameter
                rootFrame.Navigate(typeof(GamePage), eventArgs);
            }

            // Ensure the current window is active
            Window.Current.Activate();

            //TODO: For additional tips see reply on https://social.msdn.microsoft.com/Forums/sqlserver/en-US/a857bd53-abd9-40ad-9c89-b23d512abebe/uwpapp-hangscrashes-when-launched-by-uri?forum=wpdevelop
        }
    }

    protected override void OnBackgroundActivated(BackgroundActivatedEventArgs args)
    {
        base.OnBackgroundActivated(args);
        AppServiceProvider.OnBackgroundActivated(args);
    }

    /// <summary>
    /// Invoked when Navigation to a certain page fails
    /// </summary>
    /// <param name="sender">The Frame which failed navigation</param>
    /// <param name="e">Details about the navigation failure</param>
    void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
    {
        throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
    }

    /// <summary>
    /// Invoked when application execution is being suspended.  Application state is saved
    /// without knowing whether the application will be terminated or resumed with the contents
    /// of memory still intact.
    /// </summary>
    /// <param name="sender">The source of the suspend request.</param>
    /// <param name="e">Details about the suspend request.</param>
    private void OnSuspending(object sender, SuspendingEventArgs e)
    {
        var deferral = e.SuspendingOperation.GetDeferral();
        //TODO: Save application state and stop any background activity
        deferral.Complete();
    }
}
}