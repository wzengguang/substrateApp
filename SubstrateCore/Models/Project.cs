using SubstrateCore.Utils;

namespace SubstrateCore.Models
{
    public class Project
    {
        public ProjectTypeEnum ProjectType { get; set; }

        public string Framework { get; set; }

        public string Name { get; set; }

        public string RelativePath { get; set; }

        public string Content { get; set; }

        public bool Unnecessary { get; set; }

        public string PhysicalPath { get { return PathUtil.GetPhysicalPath(RelativePath); } }

        public string TargetPath { get { return PathUtil.GetTargetPath(Name, PhysicalPath); } }


        public Project() { }

        public Project(string name, string relativePath, ProjectTypeEnum projectType, string framework, bool unnecessary = false)
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
            Project other = obj as Project;
            if (other == null) return false;

            return other.Name == Name;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

    }
}
