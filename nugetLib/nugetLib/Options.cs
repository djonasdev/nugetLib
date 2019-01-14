using CommandLine;
using CommandLine.Text;

namespace nugetLib
{
    internal class Options
    {
        [VerbOption("about", HelpText = "Prints informations about the nugetLib. ")]
        public AboutSubOption AboutVerb { get; set; }

        [VerbOption("add", HelpText = "Add a file or folder to a NugetPackage")]
        public AddSubOption AddVerb { get; set; }

        [VerbOption("frameworkassemblies", HelpText = "Modify the Framework Assemblies of the given nuspec file")]
        public FrameworkAssembliesSubOption FrameworkAssembliesVerb { get; set; }

        [HelpVerbOption]
        public string GetUsage(string verb)
        {
            return HelpText.AutoBuild(this, verb);
        }
    }

    internal class AboutSubOption { }

    internal class AddSubOption
    {
        [Option('t', "target", Required = true, HelpText = "The path to the '*.nupkg' file. If you supply a folder, the newest '*.nupkg' file is automatically taken.")]
        public string TargetFile { get; set; }

        [Option('f', "file", Required = true, HelpText = "The path to the file or folder you wan't to add")]
        public string File { get; set; }
    }

    internal class FrameworkAssembliesSubOption
    {
        [Option('n', "nuspecFile", Required = false, HelpText = "The path to the '*.nuspec' file. If not set, then the first found file is used.")]
        public string TargetNuspec { get; set; }

        [Option('p', "projectFile", Required = false, HelpText = "The path to the '*.csproj' file.  If not set, then the first found file is used.")]
        public string ProjectFile { get; set; }
    }
}
