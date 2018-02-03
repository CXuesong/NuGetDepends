using System;
using System.Collections.Generic;
using System.Text;
using NuGet.Packaging.Core;
using NuGet.Versioning;

namespace NuGetDepends.ObjectModel
{
    public class PackageDependencyNode
    {
        public PackageDependencyNode(PackageIdentity package, IReadOnlyCollection<(string, VersionRange)> dependencies)
        {
            Package = package;
            Dependencies = dependencies;
        }

        /// <summary>
        /// Package ID without version information.
        /// </summary>
        public PackageIdentity Package { get; }

        /// <summary>
        /// ID of dependencies without version information.
        /// </summary>
        public IReadOnlyCollection<(string Id,VersionRange VersionRange)> Dependencies { get; }

    }
}
