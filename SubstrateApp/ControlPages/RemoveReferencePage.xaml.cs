using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SubstrateApp.ControlPages
{
    public sealed partial class RemoveReferencePage : Page
    {
        public string Xaml { get; set; } = "RemoveReference";
        public RemoveReferencePage()
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
