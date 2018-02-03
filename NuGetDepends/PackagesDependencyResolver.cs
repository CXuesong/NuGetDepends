using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NuGet;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using NuGetDepends.ObjectModel;
using Serilog;

namespace NuGetDepends
{
    public class PackagesDependencyResolver
    {
        private readonly ILogger logger;

        public PackagesDependencyResolver(ILogger logger)
        {
            this.logger = logger.ForContext<PackagesDependencyResolver>();
        }

        public string ProjectFramework { get; set; }
        
        public async Task<IList<PackageDependencyNode>> ResolveDependenciesAsync(IEnumerable<PackageIdentity> packages, CancellationToken cancellationToken)
        {
            var targetFramework = NuGetFramework.Parse(ProjectFramework);
            var providers = Repository.Provider.GetCoreV3()     // Add v3 API support
                .ToList();
            var packageSource = new PackageSource("https://api.nuget.org/v3/index.json");
            var sourceRepository = new SourceRepository(packageSource, providers);
            var dependencyResource = await sourceRepository.GetResourceAsync<DependencyInfoResource>(cancellationToken);
            var results = new List<PackageDependencyNode>();
            foreach (var package in packages)
            {
                var identity = new PackageIdentity(package.Id, package.Version);
                logger.Information("Resolving dependencies for {Id}.", identity);
                var depends = await dependencyResource.ResolvePackage(identity,
                    targetFramework, new NuGetSerilogLogger(logger), cancellationToken);
                if (depends == null)
                    throw new ArgumentException($"Missing package metadata for {identity}.");
                results.Add(new PackageDependencyNode(package,
                    depends.Dependencies.Select(pd => (pd.Id, pd.VersionRange)).ToList()));
            }
            return results;
        }

        public async Task<ICollection<PackageDependencyNode>> ShowDependenciesAsync(IEnumerable<PackageIdentity> packages, CancellationToken cancellationToken)
        {
            if (packages == null) throw new ArgumentNullException(nameof(packages));
            var packagesDict = packages.ToDictionary(p => p.Id, p => p, StringComparer.OrdinalIgnoreCase);
            var nodes = await ResolveDependenciesAsync(packagesDict.Values, cancellationToken);
            var roots = nodes.Where(n => nodes.All(n1 => n1.Dependencies.All(dep => dep.Id != n.Package.Id))).ToList();
            var visitedNodes = new HashSet<string>();
            logger.Information("Start generating dependency tree.");
            Console.WriteLine();
            Visit(roots.Select(n => (n.Package.Id, (VersionRange)null)), 0);
            return roots;

            void Visit(IEnumerable<(string Id, VersionRange VersionRange)> deps, int level)
            {
                foreach (var dep in deps)
                {
                    var node = nodes.FirstOrDefault(n => n.Package.Id == dep.Id);
                    var isFirstVisit = visitedNodes.Add(dep.Id);
                    for (int i = 0; i < level; i++) Console.Write("..");
                    if (node == null)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("{0} {1}", dep.Id, dep.VersionRange);
                        Console.WriteLine("    This dependency does not exist in the given package list.");
                        Console.ResetColor();
                    }
                    else if (isFirstVisit)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("{0} {1} <{2}>", dep.Id, dep.VersionRange, packagesDict[node.Package.Id].Version);
                        Console.ResetColor();
                        Visit(node.Dependencies, level + 1);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine("{0} {1}", dep.Id, dep.VersionRange);
                        Console.ResetColor();
                    }
                }
            }
        }

    }
}
