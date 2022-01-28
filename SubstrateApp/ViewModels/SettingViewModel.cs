using Microsoft.Toolkit.Mvvm.ComponentModel;
using SubstrateApp.Configuration;
using SubstrateCore.Services;
using Windows.System;

namespace SubstrateApp.ViewModels
{
    public class SettingViewModel : ObservableRecipient
    {
        private DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        private bool _isLoading;

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private string _substrateDirectory = AppSettings.Current.SubstrateDir;

        public string SubstrateDirectory
        {
            get => _substrateDirectory;
            set => SetProperty(ref _substrateDirectory, value);
        }

        private IProjectService _projectService;
        public SettingViewModel(IProjectService projectService)
        {
            _projectService = projectService;

        }

    }
}

