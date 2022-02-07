using SubstrateApp.Configuration;
using SubstrateApp.ViewModels;
using SubstrateCore.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Storage;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SubstrateApp.ControlPages
{
    public sealed partial class AddReferencePage : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        public AddReferenceViewModel ViewModel { get; }
        public AddReferencePage()
        {
            this.InitializeComponent();
            ViewModel = ServiceProvider.Current.GetService<AddReferenceViewModel>();
        }

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

        private void AddAssemblyBtn_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.AddReference().ConfigureAwait(false);

        }
    }
}
