using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace TextureLibrarian.Services
{
    public class FileService
    {
        public async Task ExtractZipToLibraryAsync(string zipFilePath, string libraryPath)
        {
            if (!File.Exists(zipFilePath) || !Directory.Exists(libraryPath))
                throw new ArgumentException("Invalid file or directory path");

            var zipFileName = Path.GetFileNameWithoutExtension(zipFilePath);
            var extractPath = Path.Combine(libraryPath, zipFileName);

            // Create directory if it doesn't exist
            Directory.CreateDirectory(extractPath);

            await Task.Run(() =>
            {
                using (var archive = ZipFile.OpenRead(zipFilePath))
                {
                    foreach (var entry in archive.Entries)
                    {
                        // Skip directories
                        if (string.IsNullOrEmpty(entry.Name))
                            continue;

                        var destinationPath = Path.Combine(extractPath, entry.FullName);
                        var destinationDir = Path.GetDirectoryName(destinationPath);

                        if (!Directory.Exists(destinationDir))
                            Directory.CreateDirectory(destinationDir);

                        entry.ExtractToFile(destinationPath, overwrite: true);
                    }
                }
            });
        }

        public bool IsValidImageFile(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLower();
            return extension == ".jpg" || extension == ".jpeg" || extension == ".png" ||
                   extension == ".tiff" || extension == ".tif" || extension == ".bmp" ||
                   extension == ".exr" || extension == ".hdr";
        }

        public bool IsZipFile(string filePath)
        {
            return Path.GetExtension(filePath).ToLower() == ".zip";
        }
    }
}