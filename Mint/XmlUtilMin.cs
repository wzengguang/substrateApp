using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Storage;

namespace SubstrateCore.Utils
{
    public static class XmlUtilMin
    {
        public async static Task<XDocument> LoadDocAsync(string path)
        {
            path = PathUtilMin.GetPhysicalPath(path);

            XDocument xml = null;
            try
            {
                StorageFile f = await StorageFile.GetFileFromPathAsync(path);
                using (var stream = await f.OpenStreamForReadAsync())
                {
                    xml = XDocument.Load(stream);
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return xml;
        }

        public async static Task<XElement> LoadAsync(string path)
        {
            path = PathUtilMin.GetPhysicalPath(path);

            XElement xml = null;
            try
            {
                StorageFile f = await StorageFile.GetFileFromPathAsync(path);
                using (var stream = await f.OpenStreamForReadAsync())
                {
                    xml = XElement.Load(stream);
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return xml;
        }


    }
}
