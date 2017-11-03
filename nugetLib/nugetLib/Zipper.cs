using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace nugetLib
{
    /// <summary>
    /// Class for "zip" operations
    /// </summary>
    internal class Zipper
    {
        /// <summary>
        /// is adding an item to an existing Zip Archive
        /// </summary>
        /// <param name="pathZipFile"></param>
        /// <param name="pathItem"></param>
        public static void AddItem(string pathZipFile, string pathItem)
        {
            Program.WriteLine($"Opening zip - archive '{pathZipFile}'..");
            using (FileStream fileStream = new FileStream(pathZipFile, FileMode.Open))
            {
                using (ZipArchive archive = new ZipArchive(fileStream, ZipArchiveMode.Update))
                {
                    Program.WriteLine("successfully opened");

                    //Wir haben nur eine Datei
                    if (File.Exists(pathItem))
                    {
                        Program.WriteLine($"{pathItem} is a file. Starting to add the file to the archive..");

                        FileInfo fileInfo = new FileInfo(pathItem);
                        archive.CreateEntryFromFile(fileInfo.FullName, fileInfo.Name);
                        Program.WriteLine($"{pathItem} successfully added.");
                    }
                    else if(Directory.Exists(pathItem))
                    {
                        Program.WriteLine($"{pathItem} is a directory. Starting to recursively add the files..");

                        DirectoryInfo directoryInfo = new DirectoryInfo(pathItem);
                        foreach (FileInfo file in directoryInfo.AllFilesAndFolders().Where(o => o is FileInfo).Cast<FileInfo>())
                        {
                            Program.WriteLine($"Adding file '{file.FullName}'");
                            string relPath = file.FullName.Substring(directoryInfo.FullName.Length + 1);
                            archive.CreateEntryFromFile(file.FullName, relPath);
                        }
                    }
                }
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
