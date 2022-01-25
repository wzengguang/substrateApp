namespace SubstrateApp.Utils
{
    using System;
    using System.Collections.Generic;

    public class IgnoreHashSet : HashSet<string>
    {
        public IgnoreHashSet() : base(StringComparer.OrdinalIgnoreCase)
        {
        }
    }
}
