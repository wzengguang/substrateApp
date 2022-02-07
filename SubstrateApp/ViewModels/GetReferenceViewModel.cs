using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Uwp;
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
using Microsoft.Toolkit.Mvvm.Input;
using System.Runtime.Versioning;
using SubstrateCore.Models;

namespace SubstrateApp.ViewModels
{
    public class GetReferenceViewModel : ObservableRecipient
    {
        private DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        private ISearchPathService _searchPathService;
        private IProjectService _projectService;

        public string _projectNames = "";
        public string ProjectNames
        {
            get => _projectNames;
            set => SetProperty(ref _projectNames, value);
        }

        public ObservableCollection<string> References = new ObservableCollection<string>();

        public IAsyncRelayCommand GetReferenceCommand { get; set; }

        public GetReferenceViewModel(IProjectService projectService, ISearchPathService searchPathService)
        {
            _projectService = projectService;
            _searchPathService = searchPathService;
            GetReferenceCommand = new AsyncRelayCommand(GetProjectReferences);
        }


        private async Task GetProjectReferences()
        {
            References.Clear();

            var pNames = ControlUtil.GetProjectNamesFromTextBox(ProjectNames);

            foreach (var item in pNames)
            {
                var project = await _projectService.GetProject(item);

                var references = await _projectService.GetProjectReferences(project);

                foreach (var n in references)
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
    }
}
