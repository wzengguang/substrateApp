using Microsoft.Toolkit.Uwp;
using SubstrateCore.Services;
using SubstrateCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml.Controls;

public class SearchFilePathViewModel : BindableBase
{
    private DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

    private string searchPath = "";
    public string SearchPath { get => searchPath; set => Set(ref searchPath, value); }


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
        var text = searchPath.Replace(".dll", "");
        if (text.Contains(":") && text.IndexOf("src") != -1)
        {
            var srcIndex = text.IndexOf("src");
            text = text.Substring(srcIndex + 3);
        }
        await Task.Run(async () =>
         {
             if (String.IsNullOrWhiteSpace(text))
             {
                 await dispatcherQueue.EnqueueAsync(async () =>
                 {
                     Suggestions.AddRange(await _searchPathService.GetAll());
                 });
                 return;
             }

             var projects = await _projectService.LoadProduces();
             foreach (var project in projects)
             {
                 if (Suggestions.Count > 10)
                 {
                     break;
                 }
                 var pn = project.Value?.NetFramework?.RelativePath;
                 var pro = project.Value?.Produced?.RelativePath;
                 if ((pn != null && pn.Contains(text)) || (pro != null && pro.Contains(text)))
                 {
                     await dispatcherQueue.EnqueueAsync(() =>
                    {
                        Suggestions.Add(pn);
                    });
                 }
             }
         });
    }

    public void FilePathTb_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        SearchPath = args.SelectedItem.ToString();
    }
}