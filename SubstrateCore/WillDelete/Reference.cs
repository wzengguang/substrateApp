using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubstrateApp.DataModel
{
    public class Reference
    {
        string ReferenceName { get; }

        string ReferenceDll { get; }

        ReferenceType Type { get; }

        bool Unnecessary { get; }
    }

    public enum ReferenceType
    {
        SDK,

        NuGet,

        Substrate,

        Unknown
    }
}
