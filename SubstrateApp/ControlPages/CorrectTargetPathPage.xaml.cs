using SubstrateCore.Common;
using SubstrateCore.Utils;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace SubstrateApp.ControlPages
{
    public sealed partial class CorrectTargetPathPage : Page, INotifyPropertyChanged
    {
        public string WrongTargetPath { get; set; }

        public CorrectTargetPathPage()
        {
            this.InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void FilePathBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {

        }

        private void FilePathBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {

        }

        private async void VerifyTargetPathBtn_Click(object sender, RoutedEventArgs e)
        {
            var path = PathUtil.GetPhysicalPath(FilePathBox.Text);
            await Task.Run(async () =>
            {
                var result = await remotePath(path);
                WrongTargetPath = string.Join("\r\n", result.Keys);
            });
        }

        private async Task<Dictionary<string, string>> remotePath(string path)
        {
            var xml = await XmlUtil.LoadAsync(path);

            var nones = xml.GetIncludes(ProjectConst.None);

            var customs = xml.GetIncludes(ProjectConst.QCustomInput);

            List<string> notInRemote = new List<string>();

            foreach (var item in nones.Keys)
            {
                var attrpath = nones[item].AttrInclude();
                attrpath = PathUtil.ReplacePathVirable(attrpath);

                var remotePath = (ProjectConst.redmondRemote + attrpath).Replace("/", "\\");

                if (!File.Exists(remotePath))
                {
                    notInRemote.Add(item + ".dll");
                }
            }

            return notInRemote.ToDictionary(a => a, null);
        }
    }
}
