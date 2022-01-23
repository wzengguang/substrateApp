using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SubstrateApp.ControlPages
{
    public sealed partial class AmendPackageReferencePage : Page
    {
        public string Xaml { get; set; } = "AddReferencePage";
        public AmendPackageReferencePage()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button b)
            {

            }
        }
    }
}
