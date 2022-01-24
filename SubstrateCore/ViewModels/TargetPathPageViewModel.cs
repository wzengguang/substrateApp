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
    public class TargetPathPageViewModel : BindableBase
    {
        private DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        private IProjectService ProjectService { get; }

        public TargetPathPageViewModel(IProjectService projectService)
        {
            ProjectService = projectService;
        }

        public Dictionary<string, string> NotInRemote { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> CanFix { get; set; } = new Dictionary<string, string>();

        private bool _isLoading;

        public bool IsLoading
        {
            get => _isLoading;
            set => Set(ref _isLoading, value);
        }

        private string _wrongTargetPaths;
        public string WrongTargetPaths
        {
            get => _wrongTargetPaths;
            set => Set(ref _wrongTargetPaths, value);
        }

        public async Task ReplaceTargetDir(string path, IEnumerable<string> replaces)
        {
            try
            {
                var xml = await XmlUtil.LoadAsync(path);

                var nones = xml.GetIncludes(ProjectConst.None).Where(a => a.Value.Attribute(ProjectConst.Include).Value.StartsWith(ProjectConst.TargetPathDir)).ToDictionary(a => a.Key, b => b.Value);

                var customs = xml.GetIncludes(ProjectConst.QCustomInput);

                foreach (var item in replaces)
                {
                    if (nones.ContainsKey(item))
                    {
                        var target = nones[item].AttrInclude().Replace(@"$(FlavorPlatformDir)", @"bin\$(Configuration)\netcoreapp3.1");
                        nones[item].SetAttributeValue(ProjectConst.Include, target);
                        customs[item].SetAttributeValue(ProjectConst.Include, target);
                    }
                }

                await XmlUtil.SaveAsync(xml, path);
            }
            catch (Exception e)
            {
                await dispatcherQueue.EnqueueAsync(() =>
                {
                    WrongTargetPaths = e.StackTrace;
                });
            }

        }


        public async void ClickCheck(string path)
        {
            await Task.Run(async () =>
             {
                 await VerifyPathFromRemote(path);
             });
        }

        private async Task<bool> CheckCanFix(string name)
        {
            var Projects = await ProjectService.LoadProduces();

            var p1 = Projects.ContainsKey(name) ? (Projects[name]?.Produced?.RelativePath) : null;
            if (p1 != null)
            {
                var targetPath = PathUtil.GetTargetPath(name, PathUtil.GetPhysicalPath(p1));
                var remotePath = (ProjectConst.redmondRemote + PathUtil.ReplacePathVirable(targetPath)).Replace("/", "\\");

                var targetPath2 = PathUtil.GetTargetPathNew(name, "");
                var remotePath2 = (ProjectConst.redmondRemote + PathUtil.ReplacePathVirable(targetPath2)).Replace("/", "\\");
                if (File.Exists(remotePath))
                {
                    CanFix.Add(name, targetPath);
                    return true;
                }
                else if (File.Exists(remotePath2))
                {
                    CanFix.Add(name, targetPath2);
                    return true;
                }
            }
            return false;
        }

        private async Task VerifyPathFromRemote(string path)
        {
            await dispatcherQueue.EnqueueAsync(() =>
            {
                CanFix.Clear();
                NotInRemote.Clear();
                WrongTargetPaths = "";
                IsLoading = true;
            });
            try
            {
                var xml = await XmlUtil.LoadAsync(path);

                var nones = xml.GetIncludes(ProjectConst.None).Where(a => a.Value.Attribute(ProjectConst.Include).Value.StartsWith(ProjectConst.TargetPathDir));

                var customs = xml.GetIncludes(ProjectConst.QCustomInput);

                foreach (var item in nones)
                {
                    var attrpath = item.Value.AttrInclude();
                    attrpath = PathUtil.ReplacePathVirable(attrpath);

                    var remotePath = (ProjectConst.redmondRemote + attrpath).Replace("/", "\\");

                    if (!File.Exists(remotePath) && !(await CheckCanFix(item.Key)))
                    {
                        await dispatcherQueue.EnqueueAsync(() =>
                        {
                            WrongTargetPaths = string.Join("\r\n", NotInRemote.Keys);
                            NotInRemote.Add(item.Key + ".dll", attrpath);
                        });
                    }
                }
            }
            catch (Exception e)
            {
                await dispatcherQueue.EnqueueAsync(() =>
                {
                    WrongTargetPaths = e.Message + "\r\n" + e.StackTrace;
                });
            }
            finally
            {
                await dispatcherQueue.EnqueueAsync(() =>
                {
                    IsLoading = false;
                });
            }

        }
    }
}
