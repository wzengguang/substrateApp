using SubstrateCore.Configuration;
using SubstrateCore.ViewModels;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SubstrateApp.ControlPages
{
    public sealed partial class RemoveReferencePage : Page
    {
        public RemoveReferenceViewModel ViewModel { get; }
        public RemoveReferencePage()
        {
            this.InitializeComponent();

            ViewModel = ServiceLocator.Current.GetService<RemoveReferenceViewModel>();
        }



        private async void RemoveBtn_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(async () => await ViewModel.RemoveReferenceAsync());
        }

        private void FilePathBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            ViewModel.SearchFilePathViewModel.FilePathTb_SuggestionChosen(sender, args);
        }

        private void FilePathBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                ViewModel.SearchFilePathViewModel.SearchPathTb_TextChanged(sender, args);
            }
        }
    }
}
