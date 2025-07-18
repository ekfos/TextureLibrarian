using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace TextureLibrarian.Models
{
    public class TextureItem : INotifyPropertyChanged
    {
        private string _name;
        private string _folderPath;
        private MaterialCategory _category;
        private BitmapImage _thumbnail;
        private List<TexturePass> _passes;
        private bool _isSelected;
        private string _resolution;
        private List<string> _tags;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string FolderPath
        {
            get => _folderPath;
            set => SetProperty(ref _folderPath, value);
        }

        public MaterialCategory Category
        {
            get => _category;
            set => SetProperty(ref _category, value);
        }

        public BitmapImage Thumbnail
        {
            get => _thumbnail;
            set => SetProperty(ref _thumbnail, value);
        }

        public List<TexturePass> Passes
        {
            get => _passes ?? (_passes = new List<TexturePass>());
            set => SetProperty(ref _passes, value);
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public string Resolution
        {
            get => _resolution;
            set => SetProperty(ref _resolution, value);
        }

        public List<string> Tags
        {
            get => _tags ?? (_tags = new List<string>());
            set => SetProperty(ref _tags, value);
        }

        public string Source { get; set; }
        public DateTime ImportDate { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}