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

        [HelpVerbOption]
        public string GetUsage(string verb)
        {
            return HelpText.AutoBuild(this, verb);
        }
    }

    internal class AboutSubOption { }

    internal class AddSubOption
    {
        [Option('t', "target", Required = true, HelpText = "The path to the '*.nupkg' file")]
        public string TargetFile { get; set; }

        [Option('f', "file", Required = true, HelpText = "The path to the file or folder you wan't to add")]
        public string File { get; set; }
    }
}
