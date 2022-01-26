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

        public string Name { get; set; }

        public bool IsProduced { get; set; }

        [XmlIgnore]
        public ProjectInfo Produced { get { return NetCore ?? NetStd; } }

        [XmlIgnore]
        public ProjectInfo NetFramework { get; set; }

        [XmlIgnore]
        public ProjectInfo NetCore { get; set; }

        [XmlIgnore]
        public ProjectInfo NetStd { get; set; }

        public Project() { }

        public Project(string name, ProjectTypeEnum type)
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
