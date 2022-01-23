using SubstrateApp.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Windows.Storage;
using Windows.UI.Xaml;

namespace SubstrateApp.DataModel
{
    public partial class SubstrateData
    {
        public List<ProjectWrap> ProjectWraps { get; set; }

        public List<string> SearchedProjectFile { get; set; }

        [XmlIgnore]
        public IgnoreCaseDictionary<ProjectWrap> Projects { get; set; }


        [XmlIgnore]
        private static SubstrateData instance;

        [XmlIgnore]
        public static SubstrateData Instance { get { return instance; } }

        [XmlIgnore]
        public string RootDir
        {
            get
            {
                return ApplicationData.Current.LocalSettings.Values[SettingConstant.SubstrateDir] as string;
            }
        }
    }
}
