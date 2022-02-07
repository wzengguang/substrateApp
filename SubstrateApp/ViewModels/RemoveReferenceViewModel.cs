using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Uwp;
using SubstrateCore.Common;
using SubstrateCore.Services;
using SubstrateCore.Utils;
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
    public class RemoveReferenceViewModel : ObservableRecipient
    {
        private DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        private ISearchPathService _searchPathService;
        private IProjectService _projectService;

        public SearchFilePathViewModel SearchFilePathViewModel { get; }

        public RemoveReferenceViewModel(IProjectService projectService, ISearchPathService searchPathService, SearchFilePathViewModel searchFilePathViewModel)
        {
            _projectService = projectService;
            _searchPathService = searchPathService;
            SearchFilePathViewModel = searchFilePathViewModel;
        }

        private bool _isLoading;

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string _needRemoved;
        public string NeedRemoved
        {
            get => _needRemoved;
            set => SetProperty(ref _needRemoved, value);
        }


        public async Task RemoveReferenceAsync()
        {
            await dispatcherQueue.EnqueueAsync(() => { _isLoading = true; });
            try
            {
                var path = PathUtil.GetPhysicalPath(SearchFilePathViewModel.SearchPath);

                var removes = ControlUtil.GetProjectNamesFromTextBox(NeedRemoved);

                var xml = await ProjectUtil.LoadAsync(path);
                var nones = xml.GetIncludes(SubstrateConst.None);

                var customs = xml.GetIncludes(SubstrateConst.QCustomInput);

                foreach (var remove in removes)
                {
                    var key = remove.Content;
                    if (nones.ContainsKey(key))
                    {
                        nones[key].Remove();
                    }
                    if (customs.ContainsKey(key))
                    {
                        customs[key].Remove();
                    }
                }

                await ProjectUtil.SaveAsync(xml, path);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                await dispatcherQueue.EnqueueAsync(() => { _isLoading = false; });
            }
        }
    }
}
