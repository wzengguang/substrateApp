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
    public class ProjectGroup
    {
        public ProjectTypeEnum ProjectType { get; set; }

        public string Name { get; set; }

        public bool IsProduced { get; set; }

        [XmlIgnore]
        public Project Produced { get { return NetCore ?? NetStd; } }

        [XmlIgnore]
        public Project NetFramework { get; set; }

        [XmlIgnore]
        public Project NetCore { get; set; }

        [XmlIgnore]
        public Project NetStd { get; set; }

        public ProjectGroup() { }

        public ProjectGroup(string name, ProjectTypeEnum type)
        {
            this.Name = name;
            this.ProjectType = type;

        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }
    }
}
