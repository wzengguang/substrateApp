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

        private string _scaningFolder = "";

        public string ScaningFolder
        {
            get => _scaningFolder;
            set => SetProperty(ref _scaningFolder, value);
        }

        private IProjectService _projectService;
        private IScanService _scanService;
        public SettingViewModel(IProjectService projectService, IScanService scanService)
        {
            _projectService = projectService;
            _scanService = scanService;
        }

        public async Task ScanProducedFolder()
        {
            IsLoading = true;
            await _scanService.ScanFileOfNonCoreXTProjectRestoreEntry();
            IsLoading = false;
        }


        public async Task ScanSubstrateFolder()
        {
            await dispatcherQueue.EnqueueAsync(() => { IsLoading = true; });

            try
            {
                var current = await StorageFolder.GetFolderFromPathAsync(Path.Combine(PathUtil.SubstrateDir, "sources\\dev"));
                await TraverseProjectFile(current);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            finally
            {

                var c = await _projectService.CountProjectInfo();

                await dispatcherQueue.EnqueueAsync(async () =>
                {
                    var count = await _projectService.CountProjectInfo();

                    ScaningFolder = $"Scan project {count}.";
                    IsLoading = false;
                });
            }
        }

        private async void WriteDataToDb(StorageFile fileInfo)
        {
            var fileText = await FileIO.ReadTextAsync(fileInfo);
            var xml = XDocument.Parse(fileText).Root;
            var name = ProjectUtil.InferAssemblyName(fileInfo.Path, xml) ?? Path.GetFileNameWithoutExtension(fileInfo.Path);
            try
            {
                await _projectService.InsertOrUpdateProjectInfo(new ProjectInfo
                {
                    Content = fileText,
                    Name = name
                });
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private async Task TraverseProjectFile(StorageFolder folder)
        {
            await dispatcherQueue.EnqueueAsync(() => { ScaningFolder = folder.Path; });

            var fs = await folder.GetFilesAsync();

            foreach (StorageFile fileInfo in fs)
            {
                if (fileInfo.FileType == ".csproj" || fileInfo.FileType == ".vcxproj")
                {
                    WriteDataToDb(fileInfo);
                    continue;
                }
                if (fileInfo.Name == "dirs")
                {
                    var dirs = await FileIO.ReadLinesAsync(fileInfo);

                    foreach (var item in dirs)
                    {
                        var folderName = item.Replace("\\", "").Replace("{amd64}", "").Trim();

                        if (folderName.IndexOf("DIRS_NOT_YET_PORTED", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            break;
                        }

                        if (!string.IsNullOrEmpty(folderName) &&
                            folderName.IndexOf("DIRS", StringComparison.OrdinalIgnoreCase) < 0 &&
                            !folderName.StartsWith("#"))
                        {
                            await Task.Run(async () =>
                         {
                             try
                             {
                                 var subFolder = await StorageFolder.GetFolderFromPathAsync(Path.Combine(folder.Path, folderName));
                                 await TraverseProjectFile(subFolder);
                             }
                             catch (Exception e)
                             {
                                 Debug.WriteLine(e);
                             }
                         });

                        }
                    }
                }
            }
        }
    }
}
