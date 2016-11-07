using System;
using Windows.ApplicationModel;

namespace Kliva.Models
{
    public class AppVersion : IComparable
    {
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Patch { get; set; }
        public int Iteration { get; set; }

        public AppVersion(PackageVersion appVersion)
        {
            Major = appVersion.Major;
            Minor = appVersion.Minor;
            Patch = appVersion.Build;
            Iteration = appVersion.Revision;
        }

        public int CompareTo(object obj)
        {
            if (obj is AppVersion)
            {
                AppVersion toCompare = (AppVersion)obj;

                if (Major > toCompare.Major)
                    return 1;

                if (Major < toCompare.Major)
                    return -1;

                if (Minor > toCompare.Minor)
                    return 1;

                if (Minor < toCompare.Minor)
                    return -1;

                if (Patch > toCompare.Patch)
                    return 1;

                if (Patch < toCompare.Patch)
                    return -1;

                if (Iteration > toCompare.Iteration)
                    return 1;

                if (Iteration < toCompare.Iteration)
                    return -1;
            }

            return 0;
        }

        public override string ToString()
        {
            return $"v{Major}.{Minor}.{Patch}";
        }
    }
}
