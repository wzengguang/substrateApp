namespace Mint.Substrate.Construction
{
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using Mint.Substrate.Utilities;
    using SubstrateCore.Utils;

    public class XMLFile
    {
        public string FilePath { get; }

        public XDocument Document { get; set; }

        public XNamespace Namespace { get; set; }

        public XMLFile(string path)
        {
            //ErrorUtils.VerifyThrowFileNotExists(path);
            //ErrorUtils.VerifyThrowXmlLoadFail(path);

            this.FilePath = path;

            Task.Run(async () =>
            {
                this.Document = await XmlUtilMin.LoadDocAsync(path);
                this.Namespace = this.Document.Root.GetDefaultNamespace();
            });
        }

        public void Save(bool omniDeclaration = false)
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = omniDeclaration,
                Indent = true
            };

            using (var writer = XmlWriter.Create(FilePath, settings))
            {
                this.Document.Save(writer);
            }
        }
    }
}
