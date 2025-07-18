using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using TextureLibrarian.Models;
using TextureLibrarian.Services;

namespace TextureLibrarian.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly TextureService _textureService;
        private readonly FileService _fileService;
        private readonly DragDropService _dragDropService;

        private ObservableCollection<TextureItem> _textures;
        private ObservableCollection<TextureItem> _filteredTextures;
        private ObservableCollection<MaterialCategory> _categories;
        private string _searchText;
        private MaterialCategory _selectedCategory;
        private string _libraryPath;
        private bool _isLoading;
        private double _thumbnailSize;
        private bool _isDarkTheme;
        public bool HasTextures => FilteredTextures?.Count > 0;

        public ObservableCollection<TextureItem> Textures
        {
            get => _textures;
            set => SetProperty(ref _textures, value);
        }

        public ObservableCollection<TextureItem> FilteredTextures
        {
            get => _filteredTextures;
            set => SetProperty(ref _filteredTextures, value);
        }

        public ObservableCollection<MaterialCategory> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                SetProperty(ref _searchText, value);
                FilterTextures();
            }
        }

        public MaterialCategory SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                SetProperty(ref _selectedCategory, value);
                FilterTextures();
            }
        }

        public string LibraryPath
        {
            get => _libraryPath;
            set => SetProperty(ref _libraryPath, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public double ThumbnailSize
        {
            get => _thumbnailSize;
            set => SetProperty(ref _thumbnailSize, value);
        }

        public bool IsDarkTheme
        {
            get => _isDarkTheme;
            set => SetProperty(ref _isDarkTheme, value);
        }

        public ICommand SelectLibraryCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ImportZipCommand { get; }
        public ICommand OpenFolderCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public ICommand SelectTextureCommand { get; }

        public MainViewModel()
        {
            _textureService = new TextureService();
            _fileService = new FileService();
            _dragDropService = new DragDropService();

            Textures = new ObservableCollection<TextureItem>();
            FilteredTextures = new ObservableCollection<TextureItem>();
            Categories = new ObservableCollection<MaterialCategory>(MaterialCategory.GetDefaultCategories());

            ThumbnailSize = 150;
            IsDarkTheme = true;

            SelectLibraryCommand = new RelayCommand(SelectLibrary);
            RefreshCommand = new RelayCommand(RefreshLibrary);
            ImportZipCommand = new RelayCommand(ImportZip);
            OpenFolderCommand = new RelayCommand(OpenFolder);
            ClearSearchCommand = new RelayCommand(ClearSearch);
            SelectTextureCommand = new RelayCommand(SelectTexture);

            LoadSettings();
        }

        private void SelectLibrary()
        {
            var dialog = new OpenFileDialog
            {
                Title = "Select Library Folder",
                ValidateNames = false,
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "Folder Selection"
            };

            if (dialog.ShowDialog() == true)
            {
                LibraryPath = Path.GetDirectoryName(dialog.FileName);
                RefreshLibrary();
                SaveSettings();
            }
        }

        private async void RefreshLibrary()
        {
            if (string.IsNullOrEmpty(LibraryPath) || !Directory.Exists(LibraryPath))
                return;

            IsLoading = true;
            try
            {
                var textures = await _textureService.LoadTexturesFromDirectoryAsync(LibraryPath);
                Textures.Clear();
                foreach (var texture in textures)
                {
                    Textures.Add(texture);
                }
                FilterTextures();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading textures: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async void ImportZip()
        {
            var dialog = new OpenFileDialog
            {
                Title = "Select ZIP files to import",
                Filter = "ZIP files (*.zip)|*.zip",
                Multiselect = true
            };

            if (dialog.ShowDialog() == true)
            {
                IsLoading = true;
                try
                {
                    foreach (var zipFile in dialog.FileNames)
                    {
                        await _fileService.ExtractZipToLibraryAsync(zipFile, LibraryPath);
                    }
                    RefreshLibrary();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error importing ZIP: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    IsLoading = false;
                }
            }
        }

        private void OpenFolder(object parameter)
        {
            if (parameter is TextureItem texture && Directory.Exists(texture.FolderPath))
            {
                System.Diagnostics.Process.Start("explorer.exe", texture.FolderPath);
            }
        }

        private void ClearSearch()
        {
            SearchText = "";
            SelectedCategory = null;
        }

        private void SelectTexture(object parameter)
        {
            if (parameter is TextureItem texture)
            {
                // Toggle selection
                texture.IsSelected = !texture.IsSelected;
            }
        }

        private void FilterTextures()
        {
            FilteredTextures.Clear();
            var filtered = Textures.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(t => t.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                                               t.Tags.Any(tag => tag.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
            }

            if (SelectedCategory != null && SelectedCategory.Name != "Uncategorized")
            {
                filtered = filtered.Where(t => t.Category?.Name == SelectedCategory.Name);
            }

            foreach (var texture in filtered)
            {
                FilteredTextures.Add(texture);
            }

            OnPropertyChanged(nameof(HasTextures));
        }

        private void LoadSettings()
        {
            // Load settings from file or registry
            var settingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TextureLibrarian", "settings.json");
            if (File.Exists(settingsPath))
            {
                try
                {
                    var json = File.ReadAllText(settingsPath);
                    var settings = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

                    if (settings.ContainsKey("LibraryPath"))
                        LibraryPath = settings["LibraryPath"].ToString();
                    if (settings.ContainsKey("ThumbnailSize"))
                        ThumbnailSize = Convert.ToDouble(settings["ThumbnailSize"]);
                    if (settings.ContainsKey("IsDarkTheme"))
                        IsDarkTheme = Convert.ToBoolean(settings["IsDarkTheme"]);
                }
                catch { }
            }
        }

        private void SaveSettings()
        {
            var settingsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TextureLibrarian");
            Directory.CreateDirectory(settingsDir);

            var settings = new Dictionary<string, object>
            {
                ["LibraryPath"] = LibraryPath ?? "",
                ["ThumbnailSize"] = ThumbnailSize,
                ["IsDarkTheme"] = IsDarkTheme
            };

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(settings, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(Path.Combine(settingsDir, "settings.json"), json);
        }
    }
}
