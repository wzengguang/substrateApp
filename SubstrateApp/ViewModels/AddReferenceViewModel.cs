using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Uwp;
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
    public class AddReferenceViewModel : ObservableRecipient
    {
        private DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        private ISearchPathService _searchPathService;
        private IProjectService _projectService;
        public SearchFilePathViewModel SearchFilePathViewModel { get; }

        public AddReferenceViewModel(IProjectService projectService, ISearchPathService searchPathService, SearchFilePathViewModel searchFilePathViewModel)
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

        private string _needAdd;

        public string NeedAdd
        {
            get => _needAdd;
            set => SetProperty(ref _needAdd, value);
        }

        public ObservableCollection<string> AddFail { get; set; } = new ObservableCollection<string>();

        private async Task<Dictionary<string, string>> GetKnownTargetPathFromLocalFile(Dictionary<string, string> dic)
        {
            Uri dataUri = new Uri("ms-appx:///DataModel/rightTagetPath.xml");

            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);

            var xml = await XmlUtil.LoadDocAsync(file.Path);

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

        private async Task<Dictionary<string, string>> GetTargetPathInNupkgFile()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            var path = Path.Combine(PathUtil.SubstrateDir, @"sources\dev\PopImap\nupkg");
            var folder = await StorageFolder.GetFolderFromPathAsync(path);
            await GetKnownTargetPathFromNupkgFolderFiler(folder, dic);

            await GetKnownTargetPathFromLocalFile(dic);

            return dic;
        }
        private async Task GetKnownTargetPathFromNupkgFolderFiler(StorageFolder folder, Dictionary<string, string> projects)
        {

            var fs = await folder.GetFilesAsync();

            foreach (StorageFile fileInfo in fs)
            {
                if (fileInfo.FileType == ".csproj")
                {
                    var xml = await XmlUtil.LoadAsync(fileInfo.Path);
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


        public async Task AddReference()
        {
            try
            {
                await Add();
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task Add()
        {
            await dispatcherQueue.EnqueueAsync(() =>
            {
                AddFail.Clear();
            });

            var path = PathUtil.GetPhysicalPath(SearchFilePathViewModel.SearchPath);

            await SearchFilePathViewModel.UpdateSearch(PathUtil.TrimToRelativePath(path));

            var readys = await GetTargetPathInNupkgFile();

            var needs = PathUtil.ConvertProjectNameArrayFromTextBox(NeedAdd);

            var xml = await XmlUtil.LoadAsync(path);
            var nones = xml.Descendants("None")
                    .ToDictionary(a => a.Attribute("Include").Value.Split("\\").Last().Replace(".dll", ""), a => a, StringComparer.OrdinalIgnoreCase);
            var qCustomInputs = xml.Descendants("QCustomInput")
                    .ToDictionary(a => a.Attribute("Include").Value.Split("\\").Last().Replace(".dll", ""), a => a, StringComparer.OrdinalIgnoreCase);

            foreach (var need in needs)
            {
                if (readys.ContainsKey(need))
                {
                    if (!nones.ContainsKey(need))
                    {
                        var ele = new XElement("None", new XAttribute("Pack", "true"), new XAttribute("PackagePath", @"lib\netcoreapp3.1"),
                            new XAttribute("Include", readys[need]));
                        nones.ElementAt(0).Value.Parent.Add(ele);
                    }
                    if (!qCustomInputs.ContainsKey(need))
                    {
                        var ele2 = new XElement("QCustomInput", new XAttribute("Visible", "false"),
                            new XAttribute("Include", readys[need]));
                        qCustomInputs.ElementAt(0).Value.Parent.Add(ele2);
                    }
                }
                else
                {
                    await dispatcherQueue.EnqueueAsync(() =>
                    {
                        AddFail.Add(need);
                    });
                }
            }

            await XmlUtil.SaveAsync(xml, path);

        }
    }
}
