using Microsoft.Toolkit.Mvvm.ComponentModel;
using SubstrateCore.Services;
using System.Collections.Generic;
using Windows.System;

namespace SubstrateApp.ViewModels
{
    public class ToolViewModel : ObservableRecipient
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
            set => SetProperty(ref _isLoading, value);
        }

        private string _input;
        public string Input
        {
            get => _input;
            set => SetProperty(ref _input, value);
        }

        private string _output;
        public string OutPut
        {
            get => _output;
            set => SetProperty(ref _output, value);
        }

        private string output1;
        public string OutPut1
        {
            get => output1;
            set => SetProperty(ref output1, value);
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

    }
}
