using Microsoft.Toolkit.Uwp;
using SubstrateCore.Common;
using SubstrateCore.Models;
using SubstrateCore.Services;
using SubstrateCore.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Storage;
using Windows.System;

namespace SubstrateCore.ViewModels
{
    public class ToolViewModel : BindableBase
    {
        private DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        IProjectService _projectService;

        ISearchPathService _searchPathService;

        public SearchFilePathViewModel SearchFilePathViewModel { get; }

        public ToolViewModel(IProjectService projectService, ISearchPathService searchPathService, SearchFilePathViewModel searchFilePathViewModel)
        {
            _projectService = projectService;
            _searchPathService = searchPathService;
            SearchFilePathViewModel = searchFilePathViewModel;
        }


        private bool _isLoading;

        public bool IsLoading
        {
            get => _isLoading;
            set => Set(ref _isLoading, value);
        }

        private string _input;
        public string Input
        {
            get => _input;
            set => Set(ref _input, value);
        }

        private string _output;
        public string OutPut
        {
            get => _output;
            set => Set(ref _output, value);
        }

        private string output1;
        public string OutPut1
        {
            get => output1;
            set => Set(ref output1, value);
        }

        public void HandlerMissing()
        {
            HashSet<string> outputs = new HashSet<string>();
            foreach (var item in Input.Split("\r"))
            {
                var s = item.Split("->");
                var t = s[1];

                outputs.Add(t);
            }
            OutPut = string.Join("\r", outputs);

        }


        public async void ConvertToTargetReference()
        {
            IsLoading = true;
            var xml = await XmlUtil.LoadAsync("DataModel/pr.xml", true);
            var dlls = xml.GetIncludes(SubstrateConst.None);

            var projects = await _projectService.LoadProduces();

            List<string> result = new List<string>();
            var notFind = new List<string>();
            foreach (var project in dlls)
            {
                if (projects.ContainsKey(project.Key))
                {
                    var p = projects[project.Key].Produced ?? projects[project.Key].NetFramework;
                    result.Add("<ProjectReference Include=\"$(Inetroot)\\" + p.RelativePath + "\" />");
                }
                else
                {
                    var include = project.Value.Attribute("Include").Value;
                    if (project.Key.Contains(".resources") && include.Contains("\\en\\"))
                    {
                        continue;
                    }

                    var str = "<Reference Include=\"" + project.Key + "\"><HintPath>" + include
                        + "</HintPath ></Reference>";
                    notFind.Add(str);
                }
            }

            await dispatcherQueue.EnqueueAsync(() =>
             {
                 OutPut = Converters.ToRichString(result.OrderBy(a => a));
                 OutPut1 = Converters.ToRichString(notFind.OrderBy(a => a));
                 IsLoading = false;
             });
        }
    }
}
