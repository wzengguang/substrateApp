namespace Mint.Common.Extensions
{
    using System;

    public static class VersionExtension
    {
        public static Version IncrementRevision(this Version version)
        {
            return AddVersion(version, 0, 0, 0, 1);
        }

        public static Version IncrementBuild(this Version version)
        {
            return IncrementBuild(version, true);
        }

        public static Version IncrementBuild(this Version version, bool resetLowerNumbers)
        {
            return AddVersion(version, 0, 0, 1, resetLowerNumbers ? -version.Revision : 0);
        }

        public static Version IncrementMinor(this Version version)
        {
            return IncrementMinor(version, true);
        }

        public static Version IncrementMinor(this Version version, bool resetLowerNumbers)
        {
            return AddVersion(version, 0, 1, resetLowerNumbers ? -version.Build : 0, resetLowerNumbers ? -version.Revision : 0);
        }

        public static Version IncrementMajor(this Version version)
        {
            return IncrementMajor(version, true);
        }

        public static Version IncrementMajor(this Version version, bool resetLowerNumbers)
        {
            return AddVersion(version, 1, resetLowerNumbers ? -version.Minor : 0, resetLowerNumbers ? -version.Build : 0, resetLowerNumbers ? -version.Revision : 0);
        }

        public static Version AddVersion(this Version version, string addVersion)
        {
            return AddVersion(version, new Version(addVersion));
        }

        public static Version AddVersion(this Version version, Version addVersion)
        {
            return AddVersion(version, addVersion.Major, addVersion.Minor, addVersion.Build, addVersion.Revision);
        }

        public static Version AddVersion(this Version version, int major, int minor, int build, int revision)
        {
            return SetVersion(version, version.Major + major, version.Minor + minor, version.Build + build, version.Revision + revision);
        }

        public static Version SetVersion(this Version version, int major, int minor, int build, int revision)
        {
            return new Version(major, minor, build, revision);
        }
    }
}
