using SubstrateApp.Configuration;
using SubstrateApp.ViewModels;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SubstrateApp.ControlPages
{
    public sealed partial class HandleMissingLogPage : Page
    {
        public ToolViewModel ViewModel { get; }
        public HandleMissingLogPage()
        {
            this.InitializeComponent();

            ViewModel = ServiceProvider.Current.GetService<ToolViewModel>();
        }


        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.HandlerMissing();

        }

        private void TargetToPackageBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
