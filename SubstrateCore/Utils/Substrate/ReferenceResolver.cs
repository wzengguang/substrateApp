namespace SubstrateCore.Utils
{
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using SubstrateApp.Utils;
    using SubstrateCore.Models;

    public class ReferenceResolver
    {
        private readonly string filePath;

        private static readonly Regex pkgPattern = new Regex(@"\$\(Pkg(.*?)([_\d]*)\).*");

        public async static Task<Project> Resolve(XElement element, string path)
        {
            string actualName;
            string refName, dllName;

            if (element.Is(Tags.ProjectReference))
            {
                refName = await ResolveProjectReference(path, element);
                return new Project(refName, null, ProjectTypeEnum.Substrate, null);
            }

            else if (element.Is(Tags.PackageReference))
            {
                ResolvePackageReference(element, out refName, out dllName);
                return new Project(refName, null, ProjectTypeEnum.NuGet, null);
            }

            else
            {
                ResolveNormalReference(element, out refName, out dllName, out bool unnecessary);

                // 1. try as SDK
                if ((await SubstrateNuGets.Instance()).KnownSDKs.TryGetValue(refName, out actualName))
                {
                    return new Project(actualName, null, ProjectTypeEnum.SDK, null, unnecessary);
                }

                // 2. try as NuGet
                if ((await SubstrateNuGets.Instance()).DefinedNuGets.TryGetValue(refName, out actualName) ||
                    (await SubstrateNuGets.Instance()).KnownNuGets.TryGetValue(refName, out actualName))
                {
                    return new Project(actualName, null, ProjectTypeEnum.NuGet, null, unnecessary);
                }

                // any reference starts with $(Pkg is a package reference
                MatchCollection matchs = ReferenceResolver.pkgPattern.Matches(element.ToString());
                if (matchs.Any())
                {
                    string packageName = matchs[0].Groups[1].Value.Replace("_", ".");
                    return new Project(packageName, null, ProjectTypeEnum.NuGet, null, unnecessary);
                }

                // 3. try as normal project
                //if (this.lookupTable.IsProducedProject(refName, out ProjectInfo project))
                //{
                //    string assemblyName = project.Name;
                //    return new ProjectInfo(assemblyName, null, ProjectTypeEnum.Substrate, null, unnecessary);
                //}
                return new Project(refName, null, ProjectTypeEnum.Substrate, null, unnecessary);
            }
        }

        private static async Task<string> ResolveProjectReference(string path, XElement element)
        {
            string include = element.GetAttribute(Tags.Include).Value;
            string parent = Directory.GetParent(path).FullName;
            MSBuildUtils.TryResolveBuildVariables(parent, include, out string normalPath);
            string fullPath = PathUtils.GetAbsolutePath(parent, normalPath);

            if (File.Exists(fullPath))
            {
                return (await XmlUtil.LoadDocAsync(fullPath)).TryGetAssemblyName(path);
            }
            else
            {
                string errMsg = $"Can not resolve ProjectReference: '{include}' in '{path}', last resolve result: {fullPath}";
                throw new FileNotFoundException(errMsg);
            }
        }

        private static void ResolvePackageReference(in XElement element, out string refName, out string dllName)
        {
            string include = element.GetAttribute(Tags.Include)?.Value;
            if (include == null)
            {
                include = element.GetAttribute("Update")?.Value;
            }
            refName = include;
            dllName = $"{refName}.dll";
        }

        private static void ResolveNormalReference(in XElement element, out string refName, out string dllName, out bool unnecessary)
        {
            refName = string.Empty;
            dllName = string.Empty;
            unnecessary = false;

            string include = element.GetAttribute(Tags.Include).Value; // Assert any reference SHOULD have Include
            var hintPath = element.GetFirst(Tags.HintPath);

            /*
                References with no hintpath could be:
                a) SDK references, which has name only, we'll have to guess it's dll name
                b) Other type references, it's include SHOULD be the dll path, we'll have guess it's assembly name
            */
            if (hintPath == null)
            {
                if (include.EndsWithIgnoreCase("dll") || include.EndsWithIgnoreCase("exe"))
                {
                    refName = Path.GetFileNameWithoutExtension(include);
                    dllName = Path.GetFileName(include);
                }
                else
                {
                    refName = include;
                    dllName = $"{include}.dll";
                }
            }

            /*
                References with hintpath normally has it's name in 'Include'
            */
            else
            {
                if (include.EndsWithIgnoreCase("dll") || include.EndsWithIgnoreCase("exe"))
                {
                    refName = Path.GetFileNameWithoutExtension(include);
                }
                else
                {
                    refName = include;
                }
                dllName = Path.GetFileName(hintPath.Value);
            }

            if (element.HasAttribute(Tags.AllowedUnnecessary, "true"))
            {
                unnecessary = true;
            }
            else
            {
                var au = element.GetFirst(Tags.AllowedUnnecessary);
                unnecessary = au != null && au.Value.EqualsIgnoreCase("true");
            }
        }
    }
}
