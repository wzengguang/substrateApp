using SubstrateApp.Utils;
using SubstrateCore.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public async static Task<XDocument> LoadDocAsync(string path)
        {
            path = PathUtil.GetPhysicalPath(path);

            XDocument xml = null;
            try
            {
                StorageFile f = await StorageFile.GetFileFromPathAsync(path);
                using (var stream = await f.OpenStreamForReadAsync())
                {
                    xml = XDocument.Load(stream);
                }
            }
            catch (Exception)
            {
                // file can't found
                return null;
            }

            return xml;
        }

        public async static Task<XElement> LoadAsync(string relativePath, bool fromApp = false)
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
            return await XmlUtil.LoadAsync(file.Path);
        }

        private async static Task<XElement> LoadAsync(string path)
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
            catch (Exception)
            {
                // file can't found
                return null;
            }

            return xml;
        }

        public static async Task SaveAsync(XElement xElement, string path)
        {
            path = PathUtil.GetPhysicalPath(path);

            try
            {
                StorageFile file = await StorageFile.GetFileFromPathAsync(path);
                using (Stream stream = await file.OpenStreamForWriteAsync().ConfigureAwait(false))
                {
                    stream.SetLength(0);
                    xElement.Save(stream, SaveOptions.OmitDuplicateNamespaces);
                }

                var text = (await FileIO.ReadLinesAsync(file)).ToList();
                if (text[0].Contains("<?xml version="))
                {
                    text.RemoveAt(0);
                }
                int r = 0;
                while (r < text.Count)
                {
                    if (text[r].Contains("</ItemGroup>") || text[r].Contains("</PropertyGroup>") || text[r].Contains("<Import Project="))
                    {
                        text.Insert(r + 1, "");
                    }
                    r++;
                }
                await FileIO.WriteLinesAsync(file, text);
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
            return xml.Attribute(SubstrateConst.Include)?.Value?.Trim();
        }

        public static string NameFromIncludeAttr(this XElement xml)
        {
            return xml.Attribute(SubstrateConst.Include)?.Value?.Trim().Split("\\").Last().Replace(".dll", "");
        }

    }

    public static class XDocumentUtil
    {
        public static string TryGetAssemblyName(this XDocument doc, string path)
        {
            string assemblyName = doc.GetFirst(Tags.AssemblyName)?.Value;

            if (string.IsNullOrEmpty(assemblyName))
            {
                string fileName = Path.GetFileNameWithoutExtension(path);
                assemblyName = fileName;
            }
            else if (assemblyName.EqualsIgnoreCase(@"$(RootNamespace)"))
            {
                assemblyName = doc.GetFirst(Tags.RootNamespace).Value;
            }

            return assemblyName;
        }

    }
}
