using SubstrateCore.Common;
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
    public static class XmlUtil
    {
        public async static Task<XElement> LoadAsync(string path)
        {
            path = PathUtil.GetPhysicalPath(path);

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

        public static async Task SaveAsync(XElement xElement, string path)
        {
            path = PathUtil.GetPhysicalPath(path);

            try
            {
                StorageFile file = await StorageFile.GetFileFromPathAsync(path);

                using (Stream stream = await file.OpenStreamForWriteAsync())
                {
                    xElement.Save(stream);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static Dictionary<string, XElement> GetIncludes(this XElement xml, string tag)
        {
            var result = new Dictionary<string, XElement>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in xml.Descendants(tag))
            {
                var name = item.NameFromIncludeAttr();
                if (!result.ContainsKey(name))
                {
                    result.Add(name, item);
                }
            }

            return result;
        }

        public static string AttrInclude(this XElement xml)
        {
            return xml.Attribute(ProjectConst.Include)?.Value?.Trim();
        }

        public static string NameFromIncludeAttr(this XElement xml)
        {
            return xml.Attribute(ProjectConst.Include)?.Value?.Trim().Split("\\").Last().Replace(".dll", "");
        }
    }
}
