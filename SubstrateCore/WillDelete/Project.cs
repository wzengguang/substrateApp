using Mint.Substrate;
using Mint.Substrate.Construction;
using SubstrateApp.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace SubstrateApp.DataModel
{
    public class Project
    {
        public ProjectType AssemblyType { get; set; }
        public string Framework { get; private set; }
        public bool IsProduced { get; private set; }
        public string Name { get; set; }

        /// <summary>
        /// sources\dev\PopImap\nupkg\XSO\Microsoft.Exchange.XSO.csproj
        /// </summary>
        public string RelativeFilePath { get; set; }

        /// <summary>
        /// C:\Substrate\src\sources\dev\PopImap\nupkg\XSO\Microsoft.Exchange.XSO.csproj
        /// </summary>
        [XmlIgnore]
        public string PhysicalFilePath { get { return Path.Combine(SubstrateData.Instance.RootDir, RelativeFilePath); } }

        public string TargetPath { get { return SubstrateUtil.ResolveTargetPath(Name, PhysicalFilePath); } }

        [XmlIgnore]
        public Project NetCore { get; set; }

        private ReferenceSet references;

        [XmlIgnore]
        protected ReferenceSet References { get { return GetReferences(); } }


        [XmlIgnore]
        private static LookupTable lookupTable = null;
        [XmlIgnore]
        private static LookupTable GetLookupTable
        {
            get
            {
                if (lookupTable == null)
                {
                    lookupTable = new LookupTable();
                }
                return lookupTable;
            }
        }
        private ReferenceSet GetReferences()
        {
            if (references == null)
            {
                var resolver = new ReferenceResolver(PhysicalFilePath, GetLookupTable);
                references = Repo.Load<BuildFile>(PhysicalFilePath).GetReferences(resolver);

                //XDocument xml = XDocument.Load(PhysicalFilePath);
                //var refs = xml.GetAll(Tags.Reference, Tags.ProjectReference, Tags.PackageReference);
                //foreach (var refElement in refs)
                //{
                //    var reference = Resolve(refElement);
                //    this.references.TryAdd(reference.ReferenceName, reference);
                //}
            }

            return references;
        }

        //public Reference Resolve(in XElement element)
        //{
        //    string actualName;
        //    string refName, dllName;

        //    if (element.Is(Tags.ProjectReference))
        //    {
        //        ResolveProjectReference(element, out refName, out dllName);
        //        return new Reference(refName, dllName, ReferenceType.Substrate);
        //    }

        //    else if (element.Is(Tags.PackageReference))
        //    {
        //        ResolvePackageReference(element, out refName, out dllName);
        //        return new Reference(refName, dllName, ReferenceType.NuGet);
        //    }

        //    else
        //    {
        //        ResolveNormalReference(element, out refName, out dllName, out bool unnecessary);

        //        // 1. try as SDK
        //        if (this.lookupTable.IsSDK(refName, out actualName))
        //        {
        //            return new Reference(actualName, dllName, ReferenceType.SDK, unnecessary);
        //        }

        //        // 2. try as NuGet
        //        if (this.lookupTable.IsDefinedNuGet(refName, out actualName, out _) ||
        //            this.lookupTable.IsKnownNuGet(refName, out actualName))
        //        {
        //            return new Reference(actualName, dllName, ReferenceType.NuGet, unnecessary);
        //        }

        //        // any reference starts with $(Pkg is a package reference
        //        MatchCollection matchs = this.pkgPattern.Matches(element.ToString());
        //        if (matchs.Any())
        //        {
        //            string packageName = matchs[0].Groups[1].Value.Replace("_", ".");
        //            return new Reference(packageName, dllName, ReferenceType.NuGet, unnecessary);
        //        }

        //        // 3. try as normal project
        //        if (this.lookupTable.IsProducedProject(refName, out IProject? project))
        //        {
        //            string assemblyName = project.Name;
        //            return new Reference(assemblyName, $"{assemblyName}.dll", ReferenceType.Substrate, unnecessary);
        //        }
        //        else
        //        {
        //            return new Reference(refName, $"{refName}.dll", ReferenceType.Substrate, unnecessary);
        //        }
        //    }
        //}


        //private void ResolveProjectReference(in XElement element, out string refName, out string dllName)
        //{
        //    string include = element.GetAttribute(Tags.Include).Value;
        //    string parent = Directory.GetParent(this.PhysicalFilePath).FullName;
        //    MSBuildUtils.TryResolveBuildVariables(parent, include, out string normalPath);
        //    string fullPath = PathUtils.GetAbsolutePath(parent, normalPath);

        //    if (File.Exists(fullPath))
        //    {
        //        refName = Repo.Load<BuildFile>(fullPath).AssemblyName;
        //        dllName = $"{refName}.dll";
        //    }
        //    else
        //    {
        //        string errMsg = $"Can not resolve ProjectReference: '{include}' in '{this.filePath}', last resolve result: {fullPath}";
        //        throw new FileNotFoundException(errMsg);
        //    }
        //}

        public Project() { }
        public Project(string name, ProjectType type)
        {
            this.Name = name;
            this.AssemblyType = type;

        }

        public Project(string name, string framework, string filePath, ProjectType type)
        {
            this.Name = name;

            this.RelativeFilePath = filePath;

            this.AssemblyType = type;

            this.Framework = framework;

            this.IsProduced = (this.Framework == Utils.Frameworks.NetStd || this.Framework == Utils.Frameworks.NetCore || this.Framework == Utils.Frameworks.Net) || this.AssemblyType == ProjectType.CPP;

        }
    }

    public enum ProjectType
    {
        CPP,

        NuGet,

        Substrate,

        Unknown
    }
}
