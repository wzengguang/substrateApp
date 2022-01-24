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

namespace SubstrateCore.Models
{
    public partial class SubstrateData
    {
        private List<Project> AllProjects { get; set; }

        [XmlIgnore]
        public ProjectSet ProjectSet { get; private set; } = new ProjectSet();

        public List<string> SearchedProjectFile { get; set; }


        [XmlIgnore]
        private static SubstrateData instance;

        [XmlIgnore]
        public static SubstrateData Instance { get { return instance; } }


        public void UpdateSearchedProjectFile(string add)
        {
            if (SearchedProjectFile == null)
            {
                SearchedProjectFile = new List<string>();
            }

            if (!SearchedProjectFile.Contains(add))
            {
                SearchedProjectFile.Insert(0, add);
            }

            if (SearchedProjectFile.Count > 10)
            {
                SearchedProjectFile.RemoveAt(SearchedProjectFile.Count - 1);
            }
        }

        public static async Task<SubstrateData> Load()
        {
            StorageFile file = await GetSubstrateStorageFile();

            using (var fs = await file.OpenStreamForReadAsync())
            {
                XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
                DataContractSerializer ser = new DataContractSerializer(typeof(SubstrateData));
                var readobject = (SubstrateData)ser.ReadObject(reader, true);
                SubstrateData.instance = readobject ?? new SubstrateData();
                reader.Close();
            }
            return SubstrateData.instance;
        }

        public static async void Save()
        {
            StorageFile file = await GetSubstrateStorageFile();

            using (var fs = await file.OpenStreamForWriteAsync())
            {
                DataContractSerializer ser = new DataContractSerializer(typeof(SubstrateData));
                ser.WriteObject(fs, SubstrateData.instance);
            }
        }

        private static async Task<StorageFile> GetSubstrateStorageFile()
        {
            StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile file;
            try
            {
                file = await storageFolder.GetFileAsync("\\local\\SubstrateData.xml");
            }
            catch (Exception e)
            {
                await storageFolder.CreateFileAsync("\\local\\SubstrateData.xml", Windows.Storage.CreationCollisionOption.ReplaceExisting);
                file = await storageFolder.GetFileAsync("\\local\\SubstrateData.xml");
            }

            return file;
        }

    }
}
