using Microsoft.Toolkit.Uwp;
using SubstrateCore.Common;
using SubstrateCore.Services;
using SubstrateCore.Utils;
using SubstrateCore.ViewModels;
using System;
using System.Collections.Generic;
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


        public async Task GetReferencesAsync()
        {
            NoFindProject.Clear();
            var filePaths = ProjectNames.Split("\r");
            var children = new HashSet<string>();
            var childrenChildren = new HashSet<string>();
            await dispatcherQueue.EnqueueAsync(() => { _isLoading = true; });
            try
            {
                var projects = await _projectService.LoadProduces();

                foreach (var filePath in filePaths)
                {
                    var xml = await XmlUtil.LoadAsync(filePath);
                    xml.Descendants("None").Select(a => a.Attribute("Include")?.Value).ToList()
                        .ForEach(a =>
                        {
                            var name = a.Split(@"\").Last().Replace(".dll", "").Trim();
                            children.Add(name);
                        });

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
