using SubstrateApp.Utils;
using SubstrateCore.Common;
using SubstrateCore.Models;
using SubstrateCore.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using Tags = SubstrateApp.Utils.Tags;

namespace SubstrateCore.Models
{
    public class ProjectInfo
    {
        public ProjectTypeEnum ProjectType { get; set; }

        public string Framework { get; set; }

        public string Name { get; set; }

        public string RelativePath { get; set; }

        public string Content { get; set; }

        public bool Unnecessary { get; set; }

        [XmlIgnore]
        private string physicalPath;

        [XmlIgnore]
        public string PhysicalPath
        {
            get
            {
                if (physicalPath == null)
                {
                    physicalPath = PathUtil.GetPhysicalPath(RelativePath);
                }

                return physicalPath;
            }
            set
            {
                physicalPath = value;
            }
        }

        [XmlIgnore]
        private string targetPath = null;

        [XmlIgnore]
        public string TargetPath
        {
            get
            {
                if (targetPath == null)
                {
                    targetPath = PathUtil.GetTargetPath(Name, PhysicalPath);
                }
                return targetPath;

            }
            set
            {
                targetPath = value;
            }
        }

        public ProjectInfo() { }

        public ProjectInfo(string name, string relativePath, ProjectTypeEnum projectType, string framework, bool unnecessary = false)
        {
            this.Name = name;
            this.ProjectType = projectType;
            this.RelativePath = relativePath;
            Framework = framework;
            Unnecessary = unnecessary;

        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            ProjectInfo other = obj as ProjectInfo;
            if (other == null) return false;

            return other.Name == Name;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }


        private HashSet<ProjectInfo> resolvers = new HashSet<ProjectInfo>();

        public async Task<HashSet<ProjectInfo>> getReferences()
        {
            if (resolvers.Count == 0)
            {
                var doc = await XmlUtil.LoadDocAsync(physicalPath);
                var refs = doc.GetAll(Tags.Reference, Tags.ProjectReference, Tags.PackageReference);

                foreach (var item in refs)
                {
                    var r = await ReferenceResolver.Resolve(item, physicalPath);
                    resolvers.Add(r);
                }
            }

            return resolvers;
        }
    }
}
