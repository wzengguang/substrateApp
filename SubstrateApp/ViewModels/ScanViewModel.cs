using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp;
using SubstrateApp.Configuration;
using SubstrateApp.Utils;
using SubstrateCore.Common;
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
using System.Windows.Input;
using System.Xml.Linq;
using Windows.Storage;
using Windows.System;

namespace SubstrateApp.ViewModels
{
    public class ScanViewModel : ObservableRecipient
    {
        private string _substrateDirectory = AppSettings.Current.SubstrateDir;

        private DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        private string _scaningFolder = "";

        public string ScaningFolder
        {
            get => _scaningFolder;
            set => SetProperty(ref _scaningFolder, value);
        }

        public IAsyncRelayCommand ScanCommand { get; }

        private IProjectService _projectService;
        public ScanViewModel(IProjectService projectService)
        {
            _projectService = projectService;
            ScanCommand = new AsyncRelayCommand(ScanSubstrateFolder);
        }

        public async Task ScanSubstrateFolder()
        {
            try
            {
                var current = await StorageFolder.GetFolderFromPathAsync(Path.Combine(_substrateDirectory, "sources\\dev"));

                await TraverseProjectFile(current, SaveFileInfoToDb);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
            }
            finally
            {
                var c = await _projectService.GetProjectCount();

                await dispatcherQueue.EnqueueAsync(async () =>
                {
                    var count = await _projectService.GetProjectCount();

                    ScaningFolder = $"Scan project {count}.";
                });
            }
        }

        private async Task TraverseProjectFile(StorageFolder folder, Action<StorageFile> writeData)
        {
            await dispatcherQueue.EnqueueAsync(() => { ScaningFolder = folder.Path; });
            var fs = await folder.GetFilesAsync();

            foreach (StorageFile fileInfo in fs)
            {
                if (fileInfo.FileType == ".csproj" || fileInfo.FileType == ".vcxproj")
                {
                    if (writeData != null)
                    {
                        writeData.Invoke(fileInfo);
                    }
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
                            var subFolder = await StorageFolder.GetFolderFromPathAsync(Path.Combine(folder.Path, folderName));
                            await TraverseProjectFile(subFolder, writeData);
                        }
                    }
                }
            }
        }

        private async void SaveFileInfoToDb(StorageFile fileInfo)
        {
            var fileText = await FileIO.ReadTextAsync(fileInfo);
            var xml = XDocument.Parse(fileText).Root;
            var name = ProjectUtil.InferAssemblyName(fileInfo.Path, xml) ?? Path.GetFileNameWithoutExtension(fileInfo.Path);
            try
            {
                await _projectService.InsertOrUpdateProject(new Project
                {
                    Content = fileText,
                    Name = name,
                    RelativePath = PathUtil.TrimToRelativePath(fileInfo.Path),
                    Framework = ""
                });
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
    }
}

