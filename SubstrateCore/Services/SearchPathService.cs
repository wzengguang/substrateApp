using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Windows.Storage;

namespace SubstrateCore.Services
{
    public class SearchPathService : ISearchPathService
    {
        private List<string> searchPaths = new List<string>();

        public async Task<List<string>> GetAll()
        {
            if (searchPaths.Count != 0)
            {
                return searchPaths;
            }

            StorageFile file = await GetSubstrateStorageFile();

            using (var fs = await file.OpenStreamForReadAsync())
            {
                XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
                DataContractSerializer ser = new DataContractSerializer(typeof(List<string>));
                var readobject = (List<string>)ser.ReadObject(reader, true);
                searchPaths = readobject ?? new List<string>();
                reader.Close();
            }
            return searchPaths;
        }

        public async Task Save(string path)
        {
            UpdateSearchedProjectFile(path);

            StorageFile file = await GetSubstrateStorageFile();

            using (var fs = await file.OpenStreamForWriteAsync())
            {
                DataContractSerializer ser = new DataContractSerializer(typeof(List<string>));
                ser.WriteObject(fs, searchPaths);
            }
        }

        private void UpdateSearchedProjectFile(string add)
        {
            if (!searchPaths.Contains(add))
            {
                searchPaths.Insert(0, add);
            }

            if (searchPaths.Count > 10)
            {
                searchPaths.RemoveAt(searchPaths.Count - 1);
            }
        }

        private static async Task<StorageFile> GetSubstrateStorageFile()
        {
            var filePath = "\\local\\SubstrateSearchPath.xml";
            StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile file;
            try
            {
                file = await storageFolder.GetFileAsync(filePath);
            }
            catch (Exception)
            {
                file = await storageFolder.CreateFileAsync(filePath, CreationCollisionOption.ReplaceExisting);
            }

            return file;
        }
    }
}
