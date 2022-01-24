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

namespace SubstrateCore.Models
{
    public class Project
    {
        public ProjectTypeEnum ProjectType { get; set; }
        public string Framework { get; set; }

        public string Name { get; set; }

        public string RelativePath { get; set; }


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

        [XmlIgnore]
        private Dictionary<string, Project> references;

        [XmlIgnore]
        protected Dictionary<string, Project> References { get { return references == null ? new Dictionary<string, Project>() : references; } }


        public Project() { }


        public Project(string name, string relativePath, ProjectTypeEnum projectType, string framework)
        {
            this.Name = name;
            this.ProjectType = projectType;
            this.RelativePath = relativePath;
            Framework = framework;

        }
    }
}
