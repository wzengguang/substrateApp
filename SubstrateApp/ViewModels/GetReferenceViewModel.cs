using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Uwp;
using SubstrateApp.Utils;
using SubstrateCore.Common;
using SubstrateCore.Services;
using SubstrateCore.Utils;
using SubstrateApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml.Controls;

namespace SubstrateApp.ViewModels
{
    public class GetReferenceViewModel : ObservableRecipient
    {
        private DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        private ISearchPathService _searchPathService;
        private IProjectService _projectService;

        public GetReferenceViewModel(IProjectService projectService, ISearchPathService searchPathService)
        {
            _projectService = projectService;
            _searchPathService = searchPathService;
        }

        private bool _isLoading;

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string _projectNames = "";
        public string ProjectNames
        {
            get => _projectNames;
            set => SetProperty(ref _projectNames, value);
        }

        public List<string> NoFindProject = new List<string>();

        public ObservableCollection<string> References = new ObservableCollection<string>();

        public async Task GetCurrentPathIncludes()
        {
            await dispatcherQueue.EnqueueAsync(() =>
            {
                References.Clear();
            });

            var pNames = PathUtil.ConvertProjectNameArrayFromTextBox(ProjectNames);

            foreach (var item in pNames)
            {
                var xml = await XmlUtil.LoadAsync(item);
                var nones = xml.GetIncludes(SubstrateConst.None);
                foreach (var n in nones.Keys)
                {
                    if (!References.Contains(n))
                    {
                        await dispatcherQueue.EnqueueAsync(() =>
                        {
                            References.Add(n);
                        });
                    }
                }
            }
        }


        public async Task GetReferencesAsync()
        {
            await dispatcherQueue.EnqueueAsync(() =>
            {
                _isLoading = true;
                NoFindProject.Clear();
                References.Clear();
            });

            var pNames = ProjectNames.Split("\r").Select(a => a.Replace(".dll", "").Trim());
            var children = new HashSet<string>();

            throw new NotImplementedException();
        }
    }
}
