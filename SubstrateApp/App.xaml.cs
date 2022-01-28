using SubstrateApp.Common;
using SubstrateApp.Configuration;
using SubstrateApp.Data;
using SubstrateApp.Helper;
using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.System.Profile;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SubstrateApp
{
    sealed partial class App : Application
    {
        public new static App Current => (App)Application.Current;
        public App()
        {
            Startup.ConfigureAsync();
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            this.Resuming += App_Resuming;
            this.UnhandledException += Application_UnhandledException;
            this.RequiresPointerMode = ApplicationRequiresPointerMode.WhenRequested;

            if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 6))
            {
                this.FocusVisualKind = AnalyticsInfo.VersionInfo.DeviceFamily == "Xbox" ? FocusVisualKind.Reveal : FocusVisualKind.HighVisibility;
            }
        }

        public void EnableSound(bool withSpatial = false)
        {
            ElementSoundPlayer.State = ElementSoundPlayerState.On;

            if (!withSpatial)
                ElementSoundPlayer.SpatialAudioMode = ElementSpatialAudioMode.Off;
            else
                ElementSoundPlayer.SpatialAudioMode = ElementSpatialAudioMode.On;
        }

        public static TEnum GetEnum<TEnum>(string text) where TEnum : struct
        {
            if (!typeof(TEnum).GetTypeInfo().IsEnum)
            {
                throw new InvalidOperationException("Generic parameter 'TEnum' must be an enum.");
            }
            return (TEnum)Enum.Parse(typeof(TEnum), text);
        }

        private async void App_Resuming(object sender, object e)
        {
            // We are being resumed, so lets restore our state!
            try
            {
                await SuspensionManager.RestoreAsync();
            }
            finally
            {
                switch (NavigationRootPage.RootFrame?.Content)
                {
                    case ItemPage itemPage:
                        itemPage.SetInitialVisuals();
                        break;
                    //                    case NewControlsPage _:
                    case AllControlsPage _:
                        NavigationRootPage.Current.NavigationView.AlwaysShowHeader = false;
                        break;
                }
            }

        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
#if DEBUG
            //if (System.Diagnostics.Debugger.IsAttached)
            //{
            //    this.DebugSettings.EnableFrameRateCounter = true;
            //}

            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.BindingFailed += DebugSettings_BindingFailed;
            }
#endif
            //draw into the title bar
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;

            await EnsureWindow(args);
        }

        protected async override void OnActivated(IActivatedEventArgs args)
        {
            await EnsureWindow(args);

            base.OnActivated(args);
        }

        private async Task EnsureWindow(IActivatedEventArgs args)
        {
            // No matter what our destination is, we're going to need control data loaded - let's knock that out now.
            // We'll never need to do this again.
            await ControlInfoDataSource.Instance.GetGroupsAsync();

            Frame rootFrame = GetRootFrame();

            ThemeHelper.Initialize();

            if (args.PreviousExecutionState == ApplicationExecutionState.Terminated || args.PreviousExecutionState == ApplicationExecutionState.Suspended)
            {
                try
                {
                    await SuspensionManager.RestoreAsync();
                }
                catch (SuspensionManagerException)
                {
                    //Something went wrong restoring state.
                    //Assume there is no state and continue
                }

                Window.Current.Activate();

                UpdateNavigationBasedOnSelectedPage(rootFrame);
                return;
            }

            Type targetPageType = typeof(AllControlsPage);
            string targetPageArguments = string.Empty;

            if (args.Kind == ActivationKind.Launch)
            {
                targetPageArguments = ((LaunchActivatedEventArgs)args).Arguments;
            }
            else if (args.Kind == ActivationKind.Protocol)
            {
                Match match;

                string targetId = string.Empty;

                switch (((ProtocolActivatedEventArgs)args).Uri?.AbsoluteUri)
                {
                    case string s when IsMatching(s, "(/*)category/(.*)"):
                        targetId = match.Groups[2]?.ToString();
                        if (targetId == "AllControls")
                        {
                            targetPageType = typeof(AllControlsPage);
                        }
                        else if (ControlInfoDataSource.Instance.Groups.Any(g => g.UniqueId == targetId))
                        {
                            targetPageType = typeof(SectionPage);
                        }
                        break;

                    case string s when IsMatching(s, "(/*)item/(.*)"):
                        targetId = match.Groups[2]?.ToString();
                        if (ControlInfoDataSource.Instance.Groups.Any(g => g.Items.Any(i => i.UniqueId == targetId)))
                        {
                            targetPageType = typeof(ItemPage);
                        }
                        break;
                }

                targetPageArguments = targetId;

                bool IsMatching(string parent, string expression)
                {
                    match = Regex.Match(parent, expression);
                    return match.Success;
                }
            }

            rootFrame.Navigate(targetPageType, targetPageArguments);

            //if (targetPageType == typeof(NewControlsPage))
            //{
            //    ((Microsoft.UI.Xaml.Controls.NavigationViewItem)((NavigationRootPage)Window.Current.Content).NavigationView.MenuItems[0]).IsSelected = true;
            //}
            //else
            if (targetPageType == typeof(ItemPage))
            {
                NavigationRootPage.Current.EnsureNavigationSelection(targetPageArguments);
            }

            // Ensure the current window is active
            Window.Current.Activate();
        }

        private static void UpdateNavigationBasedOnSelectedPage(Frame rootFrame)
        {
            // Check if we brought back an ItemPage
            if (rootFrame.Content is ItemPage itemPage)
            {
                // We did, so bring the selected item back into view
                string name = itemPage.Item.Title;
                if (Window.Current.Content is NavigationRootPage nav)
                {
                    // Finally brings back into view the correct item.
                    // But first: Update page layout!
                    nav.EnsureItemIsVisibleInNavigation(name);
                }
            }
        }

        private Frame GetRootFrame()
        {
            Frame rootFrame;
            if (!(Window.Current.Content is NavigationRootPage rootPage))
            {
                rootPage = new NavigationRootPage();
                rootFrame = (Frame)rootPage.FindName("rootFrame");
                if (rootFrame == null)
                {
                    throw new Exception("Root frame not found");
                }
                SuspensionManager.RegisterFrame(rootFrame, "AppFrame");
                rootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];
                rootFrame.NavigationFailed += OnNavigationFailed;

                Window.Current.Content = rootPage;
            }
            else
            {
                rootFrame = (Frame)rootPage.FindName("rootFrame");
            }

            return rootFrame;
        }

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            UpdateNavigationBasedOnSelectedPage(GetRootFrame());
            deferral.Complete();
        }

        private void DebugSettings_BindingFailed(object sender, BindingFailedEventArgs e)
        {

        }
        private async void Application_UnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            e.Handled = true;

            await new MessageDialog(e.Exception.ToString(), "Unknown Error").ShowAsync();
        }

    }
}
