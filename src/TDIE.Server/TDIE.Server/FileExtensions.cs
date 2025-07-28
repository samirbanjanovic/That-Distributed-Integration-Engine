using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace TDIE.Server
{
    public static class FileExtensions
    {
        public static async Task CopyFilesAsync(FileStream sourceFileStream, FileStream destinationFileStream)
        {
            byte[] buffer = new byte[4096];
            int charactersRead = 0;
            while ((charactersRead = await sourceFileStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
            {
                await destinationFileStream.WriteAsync(buffer, 0, charactersRead).ConfigureAwait(false);
            }
        }

        public static async Task CopyToAsync(this FileInfo sourceFile, string destinationDirectory)
        {
            using (FileStream sourceFileStream = File.OpenRead(sourceFile.FullName))
            {
                string destinationFile = Path.Combine(destinationDirectory, sourceFile.Name);
                using (FileStream destinationFileStream = File.Create(destinationFile))
                {
                    await CopyFilesAsync(sourceFileStream, destinationFileStream).ConfigureAwait(false);
                }
            }
        }

        public static async Task CopyToAsync(this DirectoryInfo sourceDirectory, string destinationDirectory)
        {
            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }

            foreach(var file in sourceDirectory.GetFiles())
            {
                await file.CopyToAsync(destinationDirectory).ConfigureAwait(false);
            }
        }
    }
}
