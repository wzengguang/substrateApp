using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Uwp;
using SubstrateCore.Services;
using SubstrateApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml.Controls;
namespace SubstrateApp.ViewModels
{
    public class SearchFilePathViewModel : ObservableRecipient
    {
        private DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        private string searchPath = "";
        public string SearchPath { get => searchPath; set => SetProperty(ref searchPath, value); }


        public List<string> Suggestions { get; set; } = new List<string>();

        ISearchPathService _searchPathService;
        IProjectService _projectService;
        public SearchFilePathViewModel(ISearchPathService searchPathService, IProjectService projectService)
        {
            _searchPathService = searchPathService;
            _projectService = projectService;
        }

        public async void SearchPathTb_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            Suggestions.Clear();

        }

        public void FilePathTb_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            SearchPath = args.SelectedItem.ToString();
        }

        public async Task UpdateSearch(string path)
        {
            await _searchPathService.Save(path);
        }
    }
}