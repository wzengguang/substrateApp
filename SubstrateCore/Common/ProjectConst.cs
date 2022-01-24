using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubstrateCore.Common
{
    public static class ProjectConst
    {
        public const string TargetFramework = "TargetFramework";

        public const string Include = "Include";

        public const string None = "None";

        public const string QCustomInput = "QCustomInput";

        public const string TargetPathDir = "$(TargetPathDir)";

        public const string FlavorPlatformDir = "$(FlavorPlatformDir)";

        public const string Configuration = "$(Configuration)";

        public const string Inetroot = "$(Inetroot)";

        public const string DebugAmd64 = @"debug\amd64";

        public const string Debug = @"Debug";

        public const string BuildArchitecture = @"$(BuildType)\$(BuildArchitecture)";

        public const string redmondRemote = @"//redmond/exchange/Build/SUBSTRATE/LATEST/target/";

    }
}
