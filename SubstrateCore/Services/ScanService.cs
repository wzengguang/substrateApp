using SubstrateCore.Common;
using SubstrateCore.Models;
using SubstrateCore.Repositories;
using SubstrateCore.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SubstrateCore.Services
{
    public class ScanService : IScanService
    {
        private SQLiteDataRepository _reop;
        public ScanService(SQLiteDataRepository reop)
        {
            _reop = reop;
        }

        public async Task ScanFileOfNonCoreXTProjectRestoreEntry()
        {
            try
            {
                XElement projectRestoreEntryXml = await XmlUtil.LoadAsync("NonCoreXTProjectRestoreEntry\\dirs.proj");

                var producedPaths = projectRestoreEntryXml.Descendants(SubstrateConst.ProjectFile)
                    .Select(x => x.Attribute(SubstrateConst.Include).Value.ReplaceIgnoreCase(SubstrateConst.Inetroot, "")
                    .Trim()).ToList();

                foreach (var producedPath in producedPaths)
                {
                    var p = PathUtil.GetPhysicalPath(producedPath);
                    XElement xml = await XmlUtil.LoadAsync(producedPath);

                    var assembliesName = ProjectUtil.InferAssemblyName(p, xml) ?? Path.GetFileNameWithoutExtension(p);

                    string framework = xml.GetFirst(SubstrateConst.TargetFramework)?.Value
                      ?? MSBuildUtils.InferFrameworkByPath(p);

                    var project = new ProjectInfo(assembliesName, PathUtil.TrimToRelativePath(p), ProjectTypeEnum.Substrate, framework);

                    await _reop.AddProject(project);
                };
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
    }
}
