using SubstrateApp.Configuration;
using SubstrateApp.ViewModels;
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

            ViewModel = ServiceProvider.Current.GetService<GetReferenceViewModel>();
        }

    }
}
