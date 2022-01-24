using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubstrateCore.Models
{
    public class ProjectSet : Dictionary<string, ProjectWrap>
    {
        public ProjectSet() : base(StringComparer.OrdinalIgnoreCase)
        {

        }
        public new ProjectWrap this[string name]
        {
            get
            {
                return this.ContainsKey(name) ? this[name] : null;
            }
            set
            {
                Add(name, value);
            }
        }

        public bool AddProject(string key, Project obj)
        {
            if (!this.ContainsKey(key))
            {
                this.Add(key, new ProjectWrap(key, obj.ProjectType));
            }

            if (obj.ProjectType == ProjectTypeEnum.Substrate)
            {
                switch (obj.Framework)
                {
                    case FrameworkConst.NetCore:
                        this[key].NetCore = obj;
                        break;
                    case FrameworkConst.NetStd:
                        this[key].NetStd = obj;
                        break;
                    case FrameworkConst.NetFramework:
                        this[key].NetFramework = obj;
                        break;
                }
            }

            return true;
        }
    }
}
