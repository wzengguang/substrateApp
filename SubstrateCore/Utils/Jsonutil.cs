using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;

namespace SubstrateCore.Utils
{
    public class Jsonutil
    {

        public async static Task<T> LoadAsync<T>(string relativePath, bool fromApp = false) where T : class
        {
            StorageFile file;
            try
            {
                if (fromApp)
                {
                    Uri dataUri = new Uri("ms-appx:///" + relativePath);
                    file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
                }
                else
                {
                    file = await StorageFile.GetFileFromPathAsync(PathUtil.GetPhysicalPath(relativePath));
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message + e.StackTrace);
                return null;
            }
            return await LoadAsync<T>(file.Path);
        }

        private async static Task<T> LoadAsync<T>(string path) where T : class
        {
            path = PathUtil.GetPhysicalPath(path);

            T obj = null;
            try
            {
                StorageFile f = await StorageFile.GetFileFromPathAsync(path);
                using (var stream = await f.OpenStreamForReadAsync())
                {
                    obj = JsonSerializer.Deserialize<T>(stream);
                }
            }
            catch (Exception)
            {
                // file can't found
                return null;
            }

            return obj;
        }
    }
}
