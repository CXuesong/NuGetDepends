using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using NuGet.Packaging.Core;
using NuGet.Versioning;

namespace NuGetDepends
{
    /// <summary>
    /// Parses <c>packages.config</c>.
    /// </summary>
    public static class PackagesConfigParser
    {

        public static IList<PackageIdentity> ParseFrom(TextReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            var doc = XDocument.Load(reader);
            return doc.Root.Elements("package")
                .Select(xp => new PackageIdentity((string)xp.Attribute("id"),
                    NuGetVersion.Parse((string)xp.Attribute("version"))))
                .ToList();
        }

        public static IList<PackageIdentity> ParseFrom(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (Directory.Exists(path))
            {
                path = Path.Combine(path, "packages.config");
            }

            return ParseFrom(File.OpenText(path));
        }

    }
}
