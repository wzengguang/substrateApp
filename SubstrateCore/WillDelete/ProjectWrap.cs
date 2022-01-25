using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubstrateApp.DataModel
{
    public class ProjectWrap
    {
        public string AssemblyName { get; set; }

        public Project NetFramwork { get; set; }

        public Project NetCore { get; set; }
    }
}
