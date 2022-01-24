using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Windows.Storage;
using Windows.UI.Xaml;

namespace SubstrateCore.Models
{
    public class SubstrateData
    {
        public List<Project> AllProjects { get; set; } = new List<Project>();

        public List<string> SearchedProjectFile { get; set; } = new List<string>();

    }
}
