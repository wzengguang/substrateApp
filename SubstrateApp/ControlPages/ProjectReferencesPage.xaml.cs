using SubstrateApp.Configuration;
using SubstrateApp.ViewModels;
using SubstrateCore.Models;
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
    public sealed partial class ProjectReferencesPage : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public ProjectReferenceViewModel ViewModel { get; }
        public ProjectReferencesPage()
        {
            this.InitializeComponent();
            ViewModel = ServiceProvider.Current.GetService<ProjectReferenceViewModel>();
        }

        private void QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            ViewModel.GetReference().ConfigureAwait(false);
        }
    }
}
