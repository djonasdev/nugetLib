using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using NuGetLib;

namespace nugetLib
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string verb = null;
            object subOptions = null;

            //args = new[] { "frameworkassemblies", "-n D:\\Repositories\\gitlab.haprotec\\dotNet\\HtPlcFramework\\HtPlcFramework\\HtPlcFramework\\HtPlcFramework.nuspec", "-c D:\\Repositories\\gitlab.haprotec\\dotNet\\HtPlcFramework\\HtPlcFramework\\HtPlcFramework\\HtPlcFramework.csproj", "-p D:\\Repositories\\gitlab.haprotec\\dotNet\\HtPlcFramework\\HtPlcFramework\\HtPlcFramework\\packages.config" , "-r"};
            //args = new[] { "frameworkassemblies"};

            Options options = new Options();
            if (!CommandLine.Parser.Default.ParseArguments(args, options,
                (invokedVerb, invokedVerbInstance) =>
                {
                    // if parsing succeeds the verb name and correct instance
                    // will be passed to onVerbCommand delegate (string,object)
                    verb = invokedVerb;
                    subOptions = invokedVerbInstance;
                }))
            {
                Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
            }

            try
            {
                // execute the corresponding command function
                Type type = subOptions.GetType();
                if (type == typeof(AboutSubOption))
                {
                    //Console.WriteLine("##################################################################################################################");
                    Version version = Assembly.GetExecutingAssembly().GetName().Version;
                    Console.WriteLine("NuGetLib gives you additional features to handle NuGet Packages.");
                    Console.WriteLine($"Version: v{version.Major}.{version.Minor}.{version.Build}");
                    Console.WriteLine("Contact: info@Dominic-Jonas.de");
                    //Console.WriteLine("##################################################################################################################");
                }
                else if (type == typeof(AddSubOption))
                {
                    _AddSubOption((AddSubOption)subOptions);
                }
                else if (type == typeof(FrameworkAssembliesSubOption))
                {
                    _FrameworkAssembliesOption((FrameworkAssembliesSubOption)subOptions);
                }
                else
                {
                    Console.Error.WriteLine("Error: unknown command '" + verb + "'");
                    Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
                Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
            }
        }

        /// <summary>
        /// Schreibt etwas in die Console mit einem Zeitstempel
        /// </summary>
        /// <param name="message"></param>
        internal static void WriteLine(string message)
        {
            Console.WriteLine($"{DateTime.Now:hh:mm:ss.fff} | {message}");
        }

        private static void _AddSubOption(AddSubOption addSubOption)
        {
            WriteLine("Operation Add File/Folder started");

            string zipPath = addSubOption.TargetFile;
            if (Directory.Exists(addSubOption.TargetFile))
            {
                WriteLine("A folder is supplied as target. Looking for the newest NuGet package..");
                DirectoryInfo directoryInfo = new DirectoryInfo(addSubOption.TargetFile);
                FileInfo fileInfo = directoryInfo.GetFiles().Where(f => f.Name.EndsWith(".nupkg")).OrderBy(f => f.CreationTime).FirstOrDefault();
                if (fileInfo == null)
                {
                    throw new FileNotFoundException("No file ending with *.nupkg found in directory!");
                }
                else
                {
                    WriteLine($"NuGet package found: {fileInfo.Name}");
                    zipPath = fileInfo.FullName;
                }                
            }
            
            Zipper.AddItem(zipPath, addSubOption.File);
            WriteLine("Operation Add File/Folder successfully finished!");
        }

        private static void _FrameworkAssembliesOption(FrameworkAssembliesSubOption frameworkAssembliesSubOption)
        {
            WriteLine("Operation modify Framework assemblies started.");

            FileInfo projectFile;
            FileInfo nuspecFile;
            FileInfo packagesFile = null;

            if (string.IsNullOrEmpty(frameworkAssembliesSubOption.ProjectFile))
            {
                WriteLine($"looking for project file in '{Environment.CurrentDirectory}'..");

                projectFile = new FileInfo(Directory.GetFiles(Environment.CurrentDirectory).FirstOrDefault(f => f.ToLower().EndsWith(".csproj")));
                WriteLine($"found.. {projectFile.FullName}");
            }
            else
            {
                projectFile = new FileInfo(frameworkAssembliesSubOption.ProjectFile);
                WriteLine($"project file: {projectFile.FullName}");
            }

            if (string.IsNullOrEmpty(frameworkAssembliesSubOption.TargetNuspec))
            {
                WriteLine($"looking for nuspec file in '{Environment.CurrentDirectory}'..");

                nuspecFile = new FileInfo(Directory.GetFiles(Environment.CurrentDirectory).FirstOrDefault(f => f.ToLower().EndsWith(".nuspec")));
                WriteLine($"found.. {nuspecFile.FullName}");
            }
            else
            {
                nuspecFile = new FileInfo(frameworkAssembliesSubOption.TargetNuspec);
                WriteLine($"nuspec file: {projectFile.FullName}");
            }

            if (!string.IsNullOrEmpty(frameworkAssembliesSubOption.PackagesFile))
            {
                packagesFile = new FileInfo(frameworkAssembliesSubOption.PackagesFile);
                WriteLine($"packages file: {packagesFile.FullName}");
            }

            WriteLine("loading dependencies..");

            List<string> references = new List<string>();
            List<XElement> itemGroups = new List<XElement>();
            List<string> nugetElements = new List<string>();

            if (packagesFile != null)
            {
                foreach (var element in XDocument.Load(packagesFile.FullName).Root.ElementsAnyNamespace("package"))
                {
                    string value = element.Attribute("id").Value;
                    nugetElements.Add(value);
                    WriteLine($"nuget reference found '{value}'");
                }
            }

            WriteLine("loading project file references..");

            foreach (XElement xElement in XDocument.Load(projectFile.FullName).Descendants())
            {
                var itemGroup = xElement.ElementsAnyNamespace("ItemGroup").ToList();
                if (itemGroup.Any())
                {
                    itemGroups.AddRange(itemGroup);
                }
            }
            
            foreach (XElement xElement in itemGroups.ElementsAnyNamespace("Reference"))
            {
                WriteLine($"Reference found: {xElement}");
                var value = xElement.Attribute("Include").Value.Split(',');
                references.Add(value[0]);
            }

            // sort references before adding
            references = references.OrderBy(r => r).ToList();

            var nuspecDoc = XDocument.Load(nuspecFile.FullName);
            var metadata = nuspecDoc.ElementAnyNamespace("package").ElementAnyNamespace("metadata");
            var frameworkAssemblies = metadata.Descendants().FirstOrDefault(d => d.Name == "frameworkAssemblies");
            if (frameworkAssemblies == null)
            {
                frameworkAssemblies = new XElement("frameworkAssemblies");
                metadata.Add(frameworkAssemblies);
            }
            else if(frameworkAssembliesSubOption.Replace)
            {
                WriteLine("Delete all existing references in nuspec");
                frameworkAssemblies.Remove();
                frameworkAssemblies = new XElement("frameworkAssemblies");
                metadata.Add(frameworkAssemblies);
            }

            foreach (string reference in references)
            {
                if (frameworkAssemblies.Descendants().Any(d => d.Attribute("assemblyName").Value == reference))
                {
                    WriteLine($"Reference '{reference}' already in nuspec file. Skip it..");
                    continue;
                }
                if(nugetElements.Contains(reference))
                    continue;
                WriteLine($"Add reference '{reference}' into nuspec file");
                frameworkAssemblies.Add(new XElement("frameworkAssembly", new XAttribute("assemblyName", reference)));
            }

            nuspecDoc.Save(nuspecFile.FullName);
        }
    }
}
