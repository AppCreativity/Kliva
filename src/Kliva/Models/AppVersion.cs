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
            this.Major = appVersion.Major;
            this.Minor = appVersion.Minor;
            this.Patch = appVersion.Build;
            this.Iteration = appVersion.Revision;
        }

        public int CompareTo(object obj)
        {
            if (obj is AppVersion)
            {
                AppVersion toCompare = (AppVersion)obj;

                if (this.Major > toCompare.Major)
                    return 1;

                if (this.Major < toCompare.Major)
                    return -1;

                if (this.Minor > toCompare.Minor)
                    return 1;

                if (this.Minor < toCompare.Minor)
                    return -1;

                if (this.Patch > toCompare.Patch)
                    return 1;

                if (this.Patch < toCompare.Patch)
                    return -1;

                if (this.Iteration > toCompare.Iteration)
                    return 1;

                if (this.Iteration < toCompare.Iteration)
                    return -1;
            }

            return 0;
        }

        public override string ToString()
        {
            return string.Format("v{0}.{1}.{2}", this.Major, this.Minor, this.Patch);
        }
    }
}
