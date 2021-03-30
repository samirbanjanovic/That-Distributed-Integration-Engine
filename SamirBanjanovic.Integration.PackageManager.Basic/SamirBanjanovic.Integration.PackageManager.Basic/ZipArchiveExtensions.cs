using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace OnTrac.Integration.PackageManager.Basic
{
    public static class ZipArchiveExtensions
    {
        public static void ExtractToDirectory(this ZipArchive zipArchive, string destinationDirectory, bool overwrite)
        {
            if(!overwrite &&  Directory.Exists(destinationDirectory))
            {
                throw new IOException($"Directory \"{destinationDirectory}\" already exists.");
            }

            var entries = zipArchive.Entries;
            foreach (var entry in zipArchive.Entries)
            {
                string entryPath = Path.Combine(destinationDirectory, entry.FullName);
                // directories in archives are identified by empty string 
                // in name property
                if (string.IsNullOrEmpty(entry.Name))
                {
                    Directory.CreateDirectory(entryPath);
                }
                else
                {// entry is a file at current level - extract content to file

                    //assume we're not adding directory - thus we need to check
                    //if we need to create the complete path tree path

                    //extract directory for path
                    var directoryPath = Path.GetDirectoryName(entryPath);

                    // create requried path if it doesn't already exist
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    //extact file content
                    entry.ExtractToFile(entryPath, true);
                }
            }
        }
    }

   
}
