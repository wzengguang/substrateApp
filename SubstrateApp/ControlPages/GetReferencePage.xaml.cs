using SubstrateCore.Configuration;
using SubstrateCore.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SubstrateApp.ControlPages
{
    public sealed partial class GetReferencePage : Page
    {


        public GetReferenceViewModel ViewModel { get; }

        public GetReferencePage()
        {
            this.InitializeComponent();

            ViewModel = ServiceLocator.Current.GetService<GetReferenceViewModel>();
        }

        private void GetReferenceBtn_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.GetCurrentPathIncludes().ConfigureAwait(false);

        }
    }
}
