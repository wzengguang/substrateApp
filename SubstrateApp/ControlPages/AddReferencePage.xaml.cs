using SubstrateApp.DataModel;
using SubstrateApp.Utils;
using SubstrateCore.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Storage;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SubstrateApp.ControlPages
{
    public sealed partial class AddReferencePage : Page
    {
        public string Xaml { get; set; } = "";

        public AddReferencePage()
        {
            this.InitializeComponent();
        }

        private void FilePathBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {

            }
        }
        private void FilePathBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
        }

        private void FilePathBox_GotFocus(object sender, RoutedEventArgs e)
        {
            // ((AutoSuggestBox)sender).ItemsSource = SubstrateData.Instance.SearchedProjectFile;
        }

        private void AddAssemblyBtn_Click(object sender, RoutedEventArgs e)
        {
            var path = FilePathBox.Text.Trim();
            string str;
            AddAssemblyBox.TextDocument.GetText(TextGetOptions.None, out str);
            Task.Run(async () =>
           {
               var NotProduceds = await AddAssembly(path, str.Split("\r\n").Select(a => a.Replace(".dll", "").Trim()));
               XamlPresenter.Text = String.Join("\r\n", NotProduceds);
           });
        }

        private async Task<List<string>> AddAssembly(string filePath, IEnumerable<string> assemblies)
        {

            var xml = await XmlUtil.LoadAsync(filePath);

            var nones = xml.Descendants("None")
                .ToDictionary(a => a.Attribute("Include").Value.Split("\\").Last().Replace(".dll", ""), a => a);
            var qCustomInputs = xml.Descendants("QCustomInput")
                .ToDictionary(a => a.Attribute("Include").Value.Split("\\").Last().Replace(".dll", ""), a => a);


            var NotProduceds = new List<string>();
            foreach (var item in assemblies)
            {
                string name;
                string targetPath;
                var all = await SubstrateUtil.AllProduced();
                var r = all.Keys.OrderBy(a => a).ToList();
                if (all.ContainsKey(item))
                {
                    var produced = all[item];
                    name = produced.Name;
                    var t = ApplicationData.Current.LocalSettings.Values[SettingConstant.SubstrateDir] as string;
                    targetPath = produced.NetCore.TargetPath;
                }
                else
                {
                    name = item;
                    targetPath = item;
                    NotProduceds.Add(item);
                }

                if (!nones.ContainsKey(name))
                {
                    var ele = new XElement("None", new XAttribute("Pack", "true"), new XAttribute("PackagePath", @"lib\netcoreapp3.1"), new XAttribute("Include", targetPath));
                    nones.Values.Last().AddAfterSelf(ele);

                    var ele2 = new XElement("QCustomInput", new XAttribute("Visible", "false"), new XAttribute("Include", targetPath));
                    qCustomInputs.Values.Last().AddAfterSelf(ele2);
                }
                else if (nones[name].Attribute("Include").Value != targetPath)
                {
                    nones[name].SetAttributeValue("Include", targetPath);
                    qCustomInputs[name].SetAttributeValue("Include", targetPath);
                }
            }

            await XmlUtil.SaveAsync(xml, filePath);

            SubstrateData.Instance.UpdateSearchedProjectFile(filePath);

            return NotProduceds;
        }
    }
}
