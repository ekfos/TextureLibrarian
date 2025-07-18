using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TextureLibrarian.Models;
using TextureLibrarian.Services;
using TextureLibrarian.ViewModels;

namespace TextureLibrarian
{
    public partial class MainWindow : Window
    {
        private readonly DragDropService _dragDropService;
        private MainViewModel ViewModel => (MainViewModel)DataContext;

        public MainWindow()
        {
            InitializeComponent();
            _dragDropService = new DragDropService();

            // Enable drag and drop
            AllowDrop = true;
            Drop += MainWindow_Drop;
            DragOver += MainWindow_DragOver;
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ViewPass_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.DataContext is TextureItem texture)
            {
                var passType = Enum.Parse<TexturePassType>(menuItem.Tag.ToString());
                var pass = texture.Passes.FirstOrDefault(p => p.Type == passType);

                if (pass != null)
                {
                    try
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = pass.FilePath,
                            UseShellExecute = true
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error opening file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void DragAs_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.DataContext is TextureItem texture)
            {
                TexturePassType passType = TexturePassType.BaseColor;

                if (menuItem.Tag.ToString() == "Composite")
                {
                    // For composite, prefer metallic for metals, base color for others
                    if (texture.Category?.Name == "Metal")
                    {
                        var metallicPass = texture.Passes.FirstOrDefault(p => p.Type == TexturePassType.Metallic);
                        if (metallicPass != null)
                        {
                            passType = TexturePassType.Metallic;
                        }
                    }
                }
                else
                {
                    passType = Enum.Parse<TexturePassType>(menuItem.Tag.ToString());
                }

                _dragDropService.StartDragDrop(texture, passType);
            }
        }

        private void MainWindow_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Any(f => f.EndsWith(".zip", StringComparison.OrdinalIgnoreCase)))
                {
                    e.Effects = DragDropEffects.Copy;
                }
                else
                {
                    e.Effects = DragDropEffects.None;
                }
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        private async void MainWindow_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                var zipFiles = files.Where(f => f.EndsWith(".zip", StringComparison.OrdinalIgnoreCase)).ToList();

                if (zipFiles.Any())
                {
                    if (string.IsNullOrEmpty(ViewModel.LibraryPath))
                    {
                        MessageBox.Show("Please select a library folder first.", "No Library Selected",
                                      MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var fileService = new FileService();
                    ViewModel.IsLoading = true;

                    try
                    {
                        foreach (var zipFile in zipFiles)
                        {
                            await fileService.ExtractZipToLibraryAsync(zipFile, ViewModel.LibraryPath);
                        }

                        // Refresh the library
                        ViewModel.RefreshCommand.Execute(null);

                        MessageBox.Show($"Successfully imported {zipFiles.Count} ZIP file(s).", "Import Complete",
                                      MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error importing ZIP files: {ex.Message}", "Import Error",
                                      MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    finally
                    {
                        ViewModel.IsLoading = false;
                    }
                }
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}