using Microsoft.Toolkit.Mvvm.ComponentModel;
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

namespace SubstrateApp.ViewModels
{
    public class TargetPathPageViewModel : ObservableRecipient
    {
        private DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        IProjectService _projectService;

        ISearchPathService _searchPathService;

        public SearchFilePathViewModel SearchFilePathViewModel { get; }

        public TargetPathPageViewModel(IProjectService projectService, ISearchPathService searchPathService, SearchFilePathViewModel searchFilePathViewModel)
        {
            _projectService = projectService;
            _searchPathService = searchPathService;
            SearchFilePathViewModel = searchFilePathViewModel;
        }

        public Dictionary<string, string> NotInRemote { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> CanFix { get; set; } = new Dictionary<string, string>();

        private bool _isLoading;

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private string _wrongTargetPaths;
        public string WrongTargetPaths
        {
            get => _wrongTargetPaths;
            set => SetProperty(ref _wrongTargetPaths, value);
        }

        public async void ReplaceTargetDir(string path, IEnumerable<string> replaces)
        {


        }

        public async Task OrderTargetPath()
        {
            var path = PathUtil.GetPhysicalPath(SearchFilePathViewModel.SearchPath);
            var xml = await ProjectUtil.LoadAsync(path);

            var nones = xml.Descendants(SubstrateConst.None).ToList();
            var noneParent = nones.First().Parent;
            foreach (var item in nones)
            {
                item.Remove();
            }
            var order = nones.OrderBy(a => a.Attribute(SubstrateConst.PackagePath).Value.Length)
                  .ThenBy(a => a.Attribute(SubstrateConst.Include).Value)
                  .ToList();

            foreach (var item in order)
            {
                noneParent.Add(item);
            }

            var customs = xml.Descendants(SubstrateConst.QCustomInput).ToList();
            var customParent = customs.First().Parent;
            foreach (var item in customs)
            {
                item.Remove();
            }
            var ordercustoms = customs.OrderBy(a => a.Attribute(SubstrateConst.Include).Value)
                  .ToList();


            foreach (var item in ordercustoms)
            {
                customParent.Add(item);
            }

            await ProjectUtil.SaveAsync(xml, path);

        }


        public async void ClickCheck()
        {
            var path = PathUtil.GetPhysicalPath(SearchFilePathViewModel.SearchPath);

            await Task.Run(async () =>
             {
                 await VerifyPathFromRemote(path);

                 await _searchPathService.Save(PathUtil.TrimToRelativePath(path));
             });
        }

        private async Task<bool> CheckCanFix(string name)
        {
            //var Projects = await _projectService.LoadProduces();

            //var p1 = Projects.ContainsKey(name) ? (Projects[name]?.Produced?.RelativePath) : null;
            //if (p1 != null)
            //{
            //    var targetPath = PathUtil.GetTargetPath(name, PathUtil.GetPhysicalPath(p1));
            //    var remotePath = (SubstrateConst.redmondRemote + PathUtil.ReplacePathVirable(targetPath)).Replace("/", "\\");

            //    var targetPath2 = PathUtil.GetTargetPathNew(name, "");
            //    var remotePath2 = (SubstrateConst.redmondRemote + PathUtil.ReplacePathVirable(targetPath2)).Replace("/", "\\");
            //    if (File.Exists(remotePath))
            //    {
            //        CanFix.Add(name, targetPath);
            //        return true;
            //    }
            //    else if (File.Exists(remotePath2))
            //    {
            //        CanFix.Add(name, targetPath2);
            //        return true;
            //    }
            //}
            //return false;
            throw new NotImplementedException();
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
                var xml = await ProjectUtil.LoadAsync(path);

                var nones = xml.GetIncludes(SubstrateConst.None).Where(a => a.Value.Attribute(SubstrateConst.Include).Value.StartsWith(SubstrateConst.TargetPathDir));

                var customs = xml.GetIncludes(SubstrateConst.QCustomInput);

                foreach (var item in nones)
                {
                    var attrpath = item.Value.AttrInclude();
                    attrpath = PathUtil.ReplacePathVirable(attrpath);

                    var remotePath = (SubstrateConst.redmondRemote + attrpath).Replace("/", "\\");

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
