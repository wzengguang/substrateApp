using Microsoft.Toolkit.Uwp;
using SubstrateApp.Configuration;
using SubstrateCore.Common;
using SubstrateCore.Utils;
using SubstrateApp.ViewModels;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace SubstrateApp.ControlPages
{
    public sealed partial class CorrectTargetPathPage : Page, INotifyPropertyChanged
    {
        private DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        public TargetPathPageViewModel ViewModel { get; }

        public CorrectTargetPathPage()
        {
            this.InitializeComponent();
            ViewModel = ServiceProvider.Current.GetService<TargetPathPageViewModel>();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void FilePathBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                ViewModel.SearchFilePathViewModel.SearchPathTb_TextChanged(sender, args);
            }
        }

        private void FilePathBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            ViewModel.SearchFilePathViewModel.FilePathTb_SuggestionChosen(sender, args);
        }

        private void VerifyTargetPathBtn_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ClickCheck();
        }

        private async void OrderTargetPathBtn_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.OrderTargetPath();
        }
    }
}
