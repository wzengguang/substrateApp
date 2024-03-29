using SubstrateApp.Helper;
using System;
using System.Linq;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using SubstrateApp.ViewModels;
using System.Threading.Tasks;
using SubstrateApp.Configuration;

namespace SubstrateApp
{
    /// <summary>
    /// A page that displays the app's settings.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        private ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public string Version
        {
            get
            {
                var version = Windows.ApplicationModel.Package.Current.Id.Version;
                return string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
            }
        }

        SettingViewModel ViewModel { get; }
        ScanViewModel ScanViewModel { get; }
        public SettingsPage()
        {
            this.InitializeComponent();

            Loaded += OnSettingsPageLoaded;

            ViewModel = ServiceProvider.Current.GetService<SettingViewModel>();
            ScanViewModel = ServiceProvider.Current.GetService<ScanViewModel>();

            if (ElementSoundPlayer.State == ElementSoundPlayerState.On)
                soundToggle.IsOn = true;
            if (ElementSoundPlayer.SpatialAudioMode == ElementSpatialAudioMode.On)
                spatialSoundBox.IsChecked = true;
            if (NavigationRootPage.Current.NavigationView.PaneDisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Auto)
            {
                navigationLocation.SelectedIndex = 0;
            }
            else
            {
                navigationLocation.SelectedIndex = 1;
            }

            //screenshotModeToggle.IsOn = UIHelper.IsScreenshotMode;
            //screenshotFolderLinkContent.Text = UIHelper.ScreenshotStorageFolder.Path;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            NavigationRootPage.Current.NavigationView.Header = "Settings";
        }

        private void OnSettingsPageLoaded(object sender, RoutedEventArgs e)
        {
            var currentTheme = ThemeHelper.RootTheme.ToString();
            (ThemePanel.Children.Cast<RadioButton>().FirstOrDefault(c => c?.Tag?.ToString() == currentTheme)).IsChecked = true;
        }

        private async void SubstrateDirBtn_Click(object sender, RoutedEventArgs e)
        {
            FolderPicker fPicker = new FolderPicker();
            var selectedFolder = await fPicker.PickSingleFolderAsync();
            fPicker.FileTypeFilter.Add("*");
            if (selectedFolder != null)
            {
                Windows.Storage.AccessCache.StorageApplicationPermissions.MostRecentlyUsedList.AddOrReplace("substrateDir", selectedFolder);

                Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", selectedFolder);
                AppSettings.Current.SubstrateDir = selectedFolder.Path;
                ViewModel.SubstrateDirectory = selectedFolder.Path;
            }
        }

        private void OnThemeRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            var selectedTheme = ((RadioButton)sender)?.Tag?.ToString();
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;

            if (selectedTheme != null)
            {
                ThemeHelper.RootTheme = App.GetEnum<ElementTheme>(selectedTheme);
            }
        }

        private void OnThemeRadioButtonKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Up)
            {
                NavigationRootPage.Current.PageHeader.Focus(FocusState.Programmatic);
            }
        }
        private void spatialSoundBox_Checked(object sender, RoutedEventArgs e)
        {
            if (soundToggle.IsOn == true)
            {
                ElementSoundPlayer.SpatialAudioMode = ElementSpatialAudioMode.On;
            }
        }

        private void soundToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (soundToggle.IsOn == true)
            {
                spatialSoundBox.IsEnabled = true;
                ElementSoundPlayer.State = ElementSoundPlayerState.On;
            }
            else
            {
                spatialSoundBox.IsEnabled = false;
                spatialSoundBox.IsChecked = false;

                ElementSoundPlayer.State = ElementSoundPlayerState.Off;
                ElementSoundPlayer.SpatialAudioMode = ElementSpatialAudioMode.Off;
            }
        }

        private void navigationToggle_Toggled(object sender, RoutedEventArgs e)
        {
            NavigationOrientationHelper.IsLeftMode = navigationLocation.SelectedIndex == 0;
        }

        private void screenshotModeToggle_Toggled(object sender, RoutedEventArgs e)
        {
            //UIHelper.IsScreenshotMode = screenshotModeToggle.IsOn;
        }

        private void spatialSoundBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (soundToggle.IsOn == true)
            {
                ElementSoundPlayer.SpatialAudioMode = ElementSpatialAudioMode.Off;
            }
        }

        private void navigationLocation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NavigationOrientationHelper.IsLeftMode = navigationLocation.SelectedIndex == 0;
        }

        private async void FolderButton_Click(object sender, RoutedEventArgs e)
        {
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            folderPicker.FileTypeFilter.Add(".png"); // meaningless, but you have to have something
            StorageFolder folder = await folderPicker.PickSingleFolderAsync();

            if (folder != null)
            {
                UIHelper.ScreenshotStorageFolder = folder;
                //screenshotFolderLink.Content = UIHelper.ScreenshotStorageFolder.Path;
            }
        }

        private async void screenshotFolderLink_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchFolderAsync(UIHelper.ScreenshotStorageFolder);
        }

        private void OnResetTeachingTipsButtonClick(object sender, RoutedEventArgs e)
        {
            ProtocolActivationClipboardHelper.ShowCopyLinkTeachingTip = true;
        }

        private void soundPageHyperlink_Click(Windows.UI.Xaml.Documents.Hyperlink sender, Windows.UI.Xaml.Documents.HyperlinkClickEventArgs args)
        {
            this.Frame.Navigate(typeof(ItemPage), "Sound");
        }

    }
}
