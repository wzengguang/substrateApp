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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

                var queue = new ConcurrentQueue<ProjectInfo>();
                await Director(current, queue);

                Dictionary<string, Project> projects = new Dictionary<string, Project>();
                foreach (var item in queue)
                {
                    // projects.AddProject(item);
                }
                await _projectService.Save(projects);

            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                await dispatcherQueue.EnqueueAsync(() =>
                {
                    ScaningFolder = "";
                    IsLoading = false;
                });
            }
        }

        public async Task Director(StorageFolder folder, ConcurrentQueue<ProjectInfo> projects)
        {
            await dispatcherQueue.EnqueueAsync(() => { ScaningFolder = folder.Path; });


            var fs = await folder.GetFilesAsync();

            foreach (StorageFile fileInfo in fs)
            {
                if (fileInfo.FileType == ".csproj" || fileInfo.FileType == ".vcxproj")
                {
                    var p = new ProjectInfo() { RelativePath = PathUtil.TrimToRelativePath(fileInfo.Path), ProjectType = ProjectTypeEnum.Substrate };
                    try
                    {
                        var xml = await XmlUtil.LoadDocAsync(fileInfo.Path);
                        var assemblyName = xml.GetFirst(SubstrateConst.AssemblyName)?.Value;
                        var framework = xml.GetFirst(SubstrateConst.TargetFramework)?.Value;
                        var package = xml.GetFirst(SubstrateConst.PackageId)?.Value;
                        var nuspe = xml.GetFirst(SubstrateConst.NuspecFile)?.Value;
                        p.Name = assemblyName ?? package ?? nuspe;
                        p.Framework = framework;
                        if (p.Framework == null && package != null)
                        {
                            p.Framework = FrameworkConst.Nupkg;
                        }
                        if (p.Name == null)
                        {
                            p.Name = fileInfo.Name;
                        }

                        projects.Append(p);
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            }

            var directs = (await folder.GetFoldersAsync()).Where(a => a.Name != "obj" && a.Name != "objd").ToArray();

            foreach (var dd in directs)
            {
                await Task.Run(async () =>
                 {
                     await Director(dd, projects);
                 });
            }
        }
    }
}
