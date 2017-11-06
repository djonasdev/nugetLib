using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NuGetLib;

namespace nugetLib
{
    /// <summary>
    /// Class for "zip" operations
    /// </summary>
    internal class Zipper
    {
        ///// <summary>
        ///// is adding an item to an existing Zip Archive
        ///// </summary>
        ///// <param name="pathZipFile"></param>
        ///// <param name="pathItem"></param>
        //public static void AddItem(string pathZipFile, string pathItem)
        //{
        //    Program.WriteLine($"Opening archive '{pathZipFile}'..");
        //    using (FileStream fileStream = new FileStream(pathZipFile, FileMode.Open))
        //    {
        //        using (ZipArchive archive = new ZipArchive(fileStream, ZipArchiveMode.Update))
        //        {
        //            Program.WriteLine("successfully opened");
        //            if (File.Exists(pathItem))
        //            {
        //                Program.WriteLine($"{pathItem} is a file. Starting to add the file to the archive..");

        //                FileInfo fileInfo = new FileInfo(pathItem);
        //                archive.CreateEntryFromFile(fileInfo.FullName, fileInfo.Name);
        //                Program.WriteLine($"{pathItem} successfully added.");
        //            }
        //            else if (Directory.Exists(pathItem))
        //            {
        //                Program.WriteLine($"{pathItem} is a directory. Starting to recursively add the files..");

        //                DirectoryInfo directoryInfo = new DirectoryInfo(pathItem);
        //                foreach (FileInfo file in directoryInfo.AllFilesAndFolders().Where(o => o is FileInfo).Cast<FileInfo>())
        //                {
        //                    Program.WriteLine($"Adding file '{file.FullName}'");
        //                    string relPath = file.FullName.Substring(directoryInfo.FullName.Length - directoryInfo.Name.Length);
        //                    archive.CreateEntryFromFile(file.FullName, relPath);
        //                }
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// is adding an item to an existing Archive
        /// </summary>
        /// <param name="pathZipFile"></param>
        /// <param name="pathItem"></param>
        public static void AddItem(string pathZipFile, string pathItem)
        {
            string appPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7zip", "7za.exe");
            if (SystemInformation.Is64BitOperatingSystem())
            {
                appPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7zip", "x64", "7za.exe");
            }

            //
            // Setup the process with the ProcessStartInfo class.
            //
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = appPath;
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            start.Arguments = $"a -r {pathZipFile} {pathItem}";
            
            //
            // Start the process.
            //
            using (Process process = Process.Start(start))
            {
                //
                // Read in all the text from the process with the StreamReader.
                //
                if(process == null)
                    throw new Exception("Failed to start the 7zip Process!");

                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    Console.Write(result);
                }

                process.WaitForExit();
                if (process.ExitCode != 0)
                    throw new Exception($"Fehlercode von 7zip: {process.ExitCode}");
            }
        }
    }

    public static class FileExtensions
    {
        public static IEnumerable<FileSystemInfo> AllFilesAndFolders(this DirectoryInfo dir)
        {
            foreach (var f in dir.GetFiles())
                yield return f;
            foreach (var d in dir.GetDirectories())
            {
                yield return d;
                foreach (var o in AllFilesAndFolders(d))
                    yield return o;
            }
        }
    }
}
