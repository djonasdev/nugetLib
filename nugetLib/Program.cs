using System;
using System.Reflection;

namespace nugetLib
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string verb = null;
            object subOptions = null;

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
                else
                {
                    Console.Error.WriteLine("Error: unknown command '" + verb + "'");
                    Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Unhandled Exception thrown! {ex}");
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
            Zipper.AddItem(addSubOption.TargetFile, addSubOption.File);
            WriteLine("Operation Add File/Folder successfully finished!");
        }
    }
}
