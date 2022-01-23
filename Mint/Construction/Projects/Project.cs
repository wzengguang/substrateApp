namespace Mint.Substrate.Construction
{
    using Mint.Substrate.Utilities;

    public class Project : IProject
    {
        public string Name { get; set; }

        public string FilePath { get; set; }

        public ProjectType Type { get; set; }

        public string Framework { get; set; }

        public bool IsProduced { get; set; }

        public string TargetPath { get; set; }
        public Project()
        {
        }
        public Project(string filePath)
        {
            FilePath = filePath;
        }

        public Project(string name, string framework, string filePath, ProjectType type)
        {
            this.Name = name;

            this.FilePath = filePath;

            this.Type = type;

            this.Framework = framework;

            this.IsProduced = (this.Framework == Frameworks.NetStd || this.Framework == Frameworks.NetCore || this.Framework == Frameworks.Net) || this.Type == ProjectType.CPP;

            bool hasTargetPath = this.Type == ProjectType.CPP ||
                                 this.Type == ProjectType.Substrate;

            if (this.IsProduced && hasTargetPath)
            {
                this.TargetPath = MSBuildUtils.ResolveTargetPath(name, filePath);
            }
        }
    }
}
