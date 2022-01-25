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

        public void HandlerMissing()
        {
            HashSet<string> outputs = new HashSet<string>();
            foreach (var item in Input.Split("\r"))
            {
                var t = item.Split("->").Last().Trim();

                outputs.Add(t);
            }
            OutPut = string.Join("\r", outputs);

        }

    }
}
