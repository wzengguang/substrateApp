using SubstrateApp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SubstrateCore.Utils
{
    public class SubstrateNuGets
    {

        public HashSet<string> KnownSDKs { get; set; }

        public HashSet<string> KnownNuGets { get; set; }

        public HashSet<string> ExcludedNuGets { get; set; }

        public Dictionary<string, string> NuGetReplacement { get; set; }

        public HashSet<string> DefinedNuGets { get; set; }
        // ------------------------------------------------------------

        internal static async Task<HashSet<string>> GetDefinedNuGets()
        {
            var doc = await XmlUtil.LoadAsync("Packages.props");

            var packages = doc.GetAll(Tags.PackageReference).Select(a => a.Attribute(Tags.Update).Value);

            var defined = new HashSet<string>(packages, StringComparer.OrdinalIgnoreCase);
            return defined;
        }
        private static SubstrateNuGets instance;

        public async static Task<SubstrateNuGets> Instance()
        {
            if (instance == null)
            {
                instance = await Jsonutil.LoadAsync<SubstrateNuGets>("Models/substrate.nugets.json", true);
                instance.KnownSDKs = new HashSet<string>(instance.KnownSDKs, StringComparer.OrdinalIgnoreCase);
                instance.KnownNuGets = new HashSet<string>(instance.KnownNuGets, StringComparer.OrdinalIgnoreCase);
                instance.ExcludedNuGets = new HashSet<string>(instance.ExcludedNuGets, StringComparer.OrdinalIgnoreCase);
                instance.NuGetReplacement = new Dictionary<string, string>(instance.NuGetReplacement, StringComparer.OrdinalIgnoreCase);
                instance.DefinedNuGets = await SubstrateNuGets.GetDefinedNuGets();
            }

            return instance;
        }
    }


}
