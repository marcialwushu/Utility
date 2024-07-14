using NuGet.Common;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace NuGetDependencyReader
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string solutionPath = @"C:\Users\marci\Documents\Workspace\CSharp projetcts"; // Mude para o caminho da sua solução
            var projects = Directory.GetFiles(solutionPath, "*.csproj", SearchOption.AllDirectories);

            foreach (var project in projects)
            {
                Console.WriteLine($"Project: {Path.GetFileName(project)}");
                var packages = GetPackagesFromProject(project);
                foreach (var package in packages)
                {
                    Console.WriteLine($"{package.Id} - {package.Version}");
                    await PrintDependencies(package);
                }
            }
        }

        static List<PackageIdentity> GetPackagesFromProject(string projectPath)
        {
            var packages = new List<PackageIdentity>();
            var lines = File.ReadAllLines(projectPath);
            foreach (var line in lines)
            {
                if (line.Contains("<PackageReference"))
                {
                    var id = GetAttributeValue(line, "Include");
                    var version = GetAttributeValue(line, "Version");
                    if (id != null && version != null)
                    {
                        packages.Add(new PackageIdentity(id, new NuGetVersion(version)));
                    }
                }
            }
            return packages;
        }

        static string GetAttributeValue(string line, string attribute)
        {
            var startIndex = line.IndexOf(attribute + "=\"") + attribute.Length + 2;
            var endIndex = line.IndexOf("\"", startIndex);
            if (startIndex > attribute.Length + 1 && endIndex > startIndex)
            {
                return line.Substring(startIndex, endIndex - startIndex);
            }
            return null;
        }

        static async Task PrintDependencies(PackageIdentity package)
        {
            var logger = NullLogger.Instance;
            var cache = new SourceCacheContext();
            var repository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");

            var resource = await repository.GetResourceAsync<FindPackageByIdResource>();

            var dependencyInfo = await resource.GetDependencyInfoAsync(
                package.Id,
                package.Version,
                cache,
                logger,
                System.Threading.CancellationToken.None);

            if (dependencyInfo != null)
            {
                foreach (var dependencyGroup in dependencyInfo.DependencyGroups)
                {
                    foreach (var dependency in dependencyGroup.Packages)
                    {
                        Console.WriteLine($"  - {dependency.Id} {dependency.VersionRange}");
                        //await PrintDependencies(new PackageIdentity(dependency.Id, dependency.VersionRange.MinVersion));
                    }
                }
            }
        }
    }
}
