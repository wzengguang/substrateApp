using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Uwp;
using SubstrateApp.Utils;
using SubstrateCore.Common;
using SubstrateCore.Configuration;
using SubstrateCore.Models;
using SubstrateCore.Services;
using SubstrateCore.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Storage;
using Windows.System;

namespace SubstrateCore.ViewModels
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

