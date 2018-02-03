using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NuGetDepends.CommandLine;
using Serilog;

namespace NuGetDepends
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .CreateLogger();
            var cmdArgs = new CommandArguments(args.Select(CommandLineParser.ParseArgument));
            var fileName = (string)cmdArgs[0];
            var packages = PackagesConfigParser.ParseFrom(fileName);
            logger.Information("Loaded {Count} packages from {FileName}.", packages.Count, fileName);

            var resolver = new PackagesDependencyResolver(logger)
            {
                ProjectFramework = (string)cmdArgs["Framework"]
            };
            await resolver.ShowDependenciesAsync(packages, CancellationToken.None);
        }
    }
}
