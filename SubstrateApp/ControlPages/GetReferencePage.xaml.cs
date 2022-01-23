using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SubstrateApp.ControlPages
{
    public sealed partial class GetReferencePage : Page
    {
        public string Xaml { get; set; } = "12345";
        public GetReferencePage()
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
