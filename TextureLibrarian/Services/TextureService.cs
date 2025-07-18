using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using TextureLibrarian.Models;

namespace TextureLibrarian.Services
{
    public class TextureService
    {
        private readonly string[] _supportedImageExtensions = { ".jpg", ".jpeg", ".png", ".tiff", ".tif", ".bmp", ".exr", ".hdr" };
        private readonly Dictionary<string, TexturePassType> _passTypeMap = new Dictionary<string, TexturePassType>
        {
            { "basecolor", TexturePassType.BaseColor },
            { "base_color", TexturePassType.BaseColor },
            { "diffuse", TexturePassType.BaseColor },
            { "albedo", TexturePassType.BaseColor },
            { "col", TexturePassType.BaseColor },
            { "normal", TexturePassType.Normal },
            { "nor", TexturePassType.Normal },
            { "nrm", TexturePassType.Normal },
            { "roughness", TexturePassType.Roughness },
            { "rough", TexturePassType.Roughness },
            { "rgh", TexturePassType.Roughness },
            { "height", TexturePassType.Height },
            { "disp", TexturePassType.Displacement },
            { "displacement", TexturePassType.Displacement },
            { "ao", TexturePassType.AO },
            { "ambient", TexturePassType.AO },
            { "occlusion", TexturePassType.AO },
            { "metallic", TexturePassType.Metallic },
            { "metal", TexturePassType.Metallic },
            { "specular", TexturePassType.Specular },
            { "spec", TexturePassType.Specular },
            { "opacity", TexturePassType.Opacity },
            { "alpha", TexturePassType.Opacity },
            { "emission", TexturePassType.Emission },
            { "emit", TexturePassType.Emission }
        };

        public async Task<List<TextureItem>> LoadTexturesFromDirectoryAsync(string directoryPath)
        {
            var textures = new List<TextureItem>();
            var categories = MaterialCategory.GetDefaultCategories();

            await Task.Run(() =>
            {
                var subdirectories = Directory.GetDirectories(directoryPath);

                foreach (var subDir in subdirectories)
                {
                    var dirName = Path.GetFileName(subDir);
                    var imageFiles = Directory.GetFiles(subDir, "*.*", SearchOption.AllDirectories)
                        .Where(f => _supportedImageExtensions.Contains(Path.GetExtension(f).ToLower()))
                        .ToList();

                    if (imageFiles.Any())
                    {
                        var texture = new TextureItem
                        {
                            Name = dirName,
                            FolderPath = subDir,
                            ImportDate = Directory.GetCreationTime(subDir)
                        };

                        // Categorize texture
                        texture.Category = CategorizeTexture(dirName, categories);

                        // Load texture passes
                        LoadTexturePasses(texture, imageFiles);

                        // Generate thumbnail
                        GenerateThumbnail(texture);

                        // Extract resolution info
                        ExtractResolutionInfo(texture);

                        textures.Add(texture);
                    }
                }
            });

            return textures;
        }

        private MaterialCategory CategorizeTexture(string textureName, List<MaterialCategory> categories)
        {
            var lowerName = textureName.ToLower();

            foreach (var category in categories)
            {
                if (category.Keywords.Any(keyword => lowerName.Contains(keyword.ToLower())))
                {
                    return category;
                }
            }

            return categories.First(c => c.Name == "Uncategorized");
        }

        private void LoadTexturePasses(TextureItem texture, List<string> imageFiles)
        {
            texture.Passes = new List<TexturePass>();

            foreach (var file in imageFiles)
            {
                var fileName = Path.GetFileNameWithoutExtension(file).ToLower();
                var passType = TexturePassType.Unknown;

                foreach (var kvp in _passTypeMap)
                {
                    if (fileName.Contains(kvp.Key))
                    {
                        passType = kvp.Value;
                        break;
                    }
                }

                texture.Passes.Add(new TexturePass
                {
                    Name = Path.GetFileName(file),
                    FilePath = file,
                    Type = passType
                });
            }
        }

        private void GenerateThumbnail(TextureItem texture)
        {
            try
            {
                string thumbnailPath = null;

                // For metal textures, try to find a metallic pass first, then base color
                if (texture.Category?.Name == "Metal")
                {
                    var metallicPass = texture.Passes.FirstOrDefault(p => p.Type == TexturePassType.Metallic);
                    thumbnailPath = metallicPass?.FilePath;
                }

                // If no metallic pass or not metal, try base color
                if (thumbnailPath == null)
                {
                    var baseColorPass = texture.Passes.FirstOrDefault(p => p.Type == TexturePassType.BaseColor);
                    thumbnailPath = baseColorPass?.FilePath;
                }

                // Fall back to first available pass
                if (thumbnailPath == null)
                {
                    thumbnailPath = texture.Passes.FirstOrDefault()?.FilePath;
                }

                if (thumbnailPath != null && File.Exists(thumbnailPath))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(thumbnailPath);
                    bitmap.DecodePixelWidth = 200;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze();

                    texture.Thumbnail = bitmap;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error generating thumbnail: {ex.Message}");
            }
        }

        private void ExtractResolutionInfo(TextureItem texture)
        {
            try
            {
                var firstImage = texture.Passes.FirstOrDefault()?.FilePath;
                if (firstImage != null && File.Exists(firstImage))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(firstImage);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();

                    texture.Resolution = $"{bitmap.PixelWidth}x{bitmap.PixelHeight}";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error extracting resolution: {ex.Message}");
                texture.Resolution = "Unknown";
            }
        }
    }
}