using Microsoft.Toolkit.Uwp;
using Mint.Substrate.Construction;
using SubstrateApp.Utils;
using SubstrateCore.Common;
using SubstrateCore.Services;
using SubstrateCore.Utils;
using SubstrateCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml.Controls;

namespace SubstrateCore.ViewModels
{
    public class GetReferenceViewModel : BindableBase
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
            set => Set(ref _isLoading, value);
        }

        public string _projectNames = "";
        public string ProjectNames
        {
            get => _projectNames;
            set => Set(ref _projectNames, value);
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
            try
            {
                var projects = await _projectService.LoadProduces();

                foreach (var pName in pNames)
                {
                    if (projects.ContainsKey(pName))
                    {
                        projects[pName].Produced.GetReferences().Where(a => a.Type == ReferenceType.Substrate)
                               .Select(a => a.ReferenceName).ToList().ForEach(async a =>
                               {
                                   await dispatcherQueue.EnqueueAsync(() =>
                                  {
                                      References.Add(a);
                                  });
                               });
                    }
                    else
                    {
                        await dispatcherQueue.EnqueueAsync(() =>
                        {
                            NoFindProject.Add(pName);
                        });
                    }
                }
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
