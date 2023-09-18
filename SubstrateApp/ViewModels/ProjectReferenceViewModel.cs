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

        private bool _isLoading;

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ObservableCollection<string> Result { get; set; } = new ObservableCollection<string>();

        private async Task<Dictionary<string, string>> GetKnownTargetPathFromLocalFile(Dictionary<string, string> dic)
        {
            Uri dataUri = new Uri("ms-appx:///DataModel/rightTagetPath.xml");

            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);

            var xml = await ProjectUtil.LoadDocAsync(file.Path);

            foreach (var item in xml.Descendants("None"))
            {
                var attr = item.Attribute("Include")?.Value;
                var name = attr.Split("\\").Last().Replace(".dll", "");
                if (!dic.ContainsKey(name))
                {
                    dic.Add(name, attr);
                }
            }
            return dic;
        }

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
            await dispatcherQueue.EnqueueAsync(() =>
            {
                Result.Clear();
            });

            string path = PathUtil.GetPhysicalPath(SearchFilePathViewModel.SearchPath);

            XDocument xDocument = await ProjectUtil.LoadDocAsync(path);
            if (xDocument == null)
            {
                return;
            }

            TreeNode treeNode = new TreeNode { NodeValue = path };
            await FindNotNet6ProjectReference(xDocument, treeNode, 0);
            var list = treeNode.ToList();
            await dispatcherQueue.EnqueueAsync(() =>
            {
                foreach (var item in list)
                {
                    Result.Add(item);
                }
            });

            await ProjectUtil.SaveAsync(xDocument.Root, path);
        }

        private async Task FindNotNet6ProjectReference(XDocument xDocument, TreeNode treeNode, int deep, int maxDeep = 10)
        {
            deep++;

            var projectReferenceElements = xDocument.Root.AllDescendent("ProjectReference");

            foreach (var projectReferenceElement in projectReferenceElements)
            {
                string includePath = projectReferenceElement.Attribute("Include")?.Value;
                if (includePath != null)
                {
                    string fullPath = SubstratePath.Combine(includePath.ReplaceIgnoreCase("$(INETROOT)\\", string.Empty));
                    var doc = await ProjectUtil.LoadAsync(fullPath);
                    if (doc != null)
                    {
                        var propertyGroupElement = doc.FirstChild("PropertyGroup");
                        string propertyGroupElementValue = propertyGroupElement?.Value;
                        if (propertyGroupElementValue != null && !propertyGroupElementValue.ContainIgnoreCase("net6"))
                        {
                            TreeNode childTree = new TreeNode { Parent = treeNode, NodeValue = includePath };

                            treeNode.Children.Add(childTree);
                            if (deep <= maxDeep)
                            {
                                XDocument xd = await ProjectUtil.LoadDocAsync(fullPath);
                                if (xd != null)
                                {
                                    await FindNotNet6ProjectReference(xd, childTree, deep);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
