using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Uwp;
using SubstrateCore;
using SubstrateCore.Models;
using SubstrateCore.Services;
using SubstrateCore.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Storage;
using Windows.System;

namespace SubstrateApp.ViewModels
{
    public class ProjectReferenceViewModel : ObservableRecipient
    {
        private DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        public SearchFilePathViewModel SearchFilePathViewModel { get; }

        public ProjectReferenceViewModel(IProjectService projectService, ISearchPathService searchPathService, SearchFilePathViewModel searchFilePathViewModel)
        {
            SearchFilePathViewModel = searchFilePathViewModel;
        }

        private bool _isCanQuery = true;

        public bool IsCanQuery
        {
            get => _isCanQuery;
            set => SetProperty(ref _isCanQuery, value);
        }

        public ObservableCollection<TreeNode> TreeDataSource = new ObservableCollection<TreeNode>();

        public ObservableCollection<string> Result { get; set; } = new ObservableCollection<string>();

        private async Task GetKnownTargetPathFromNupkgFolderFiler(StorageFolder folder, Dictionary<string, string> projects)
        {

            var fs = await folder.GetFilesAsync();

            foreach (StorageFile fileInfo in fs)
            {
                if (fileInfo.FileType == ".csproj")
                {
                    var xml = await ProjectUtil.LoadAsync(fileInfo.Path);
                    foreach (var none in xml.Descendants("None"))
                    {
                        var attr = none.Attribute("Include")?.Value;
                        var name = attr.Split("\\").Last().Replace(".dll", "");
                        if (!projects.ContainsKey(name))
                        {
                            projects.Add(name, attr);
                        }
                    }
                }
            }

            var directs = (await folder.GetFoldersAsync()).Where(a => a.Name != "obj" && a.Name != "objd" && a.Name != ".pkgrefgen").ToArray();

            foreach (var dd in directs)
            {
                await Task.Run(async () =>
                {
                    await GetKnownTargetPathFromNupkgFolderFiler(dd, projects);
                });
            }
        }

        public async Task GetReference()
        {
            IsCanQuery = false;
            await dispatcherQueue.EnqueueAsync(() =>
            {
                Result.Clear();
                TreeDataSource.Clear();
            });
            string path = PathUtil.GetPhysicalPath(SearchFilePathViewModel.SearchPath);
            TreeNode treeNode = new TreeNode { NodeValue = PathUtil.ConvertPhysicalPathToRelative(path) };
            XDocument xDocument = await ProjectUtil.LoadDocAsync(path);
            if (xDocument == null)
            {
                return;
            }

            await FindNotNet6ProjectReference(xDocument, treeNode, 0);
            await dispatcherQueue.EnqueueAsync(() =>
            {
                TreeDataSource.Add(treeNode);

                var list = treeNode.ToList();
                foreach (var item in list)
                {
                    Result.Add(item);
                }
            });

            // await ProjectUtil.SaveAsync(xDocument.Root, path);

            IsCanQuery = true;
        }

        private async Task FindNotNet6ProjectReference(XDocument xDocument, TreeNode treeNode, int deep, int maxDeep = 10)
        {
            deep++;
            List<Task> tasks = new List<Task>();
            var projectReferenceElements = xDocument.Root.AllDescendent("ProjectReference");

            foreach (var projectReferenceElement in projectReferenceElements)
            {
                if (projectReferenceElement.Parent.CheckAttribute("Condition", true, "==", "net472") != null)
                {
                    continue;
                }

                string includePath = projectReferenceElement.Attribute("Include")?.Value;
                if (includePath != null)
                {
                    string relativePath = includePath.ReplaceIgnoreCase("$(INETROOT)\\", string.Empty);
                    var doc = await ProjectUtil.LoadAsync(relativePath);
                    if (doc != null)
                    {
                        var propertyGroupElement = doc.FirstChild("PropertyGroup");
                        string propertyGroupElementValue = propertyGroupElement?.Value;
                        if (propertyGroupElementValue != null && !propertyGroupElementValue.ContainIgnoreCase("net6"))
                        {
                            TreeNode childTree = new TreeNode { Parent = treeNode, NodeValue = relativePath };
                            treeNode.Children.Add(childTree);
                            if (deep <= maxDeep)
                            {
                                XDocument xd = await ProjectUtil.LoadDocAsync(relativePath);
                                if (xd != null)
                                {
                                    tasks.Add(Task.Run(() => FindNotNet6ProjectReference(xd, childTree, deep)));
                                }
                            }
                        }
                    }
                }
            }

            await Task.WhenAll(tasks);
        }
    }
}
