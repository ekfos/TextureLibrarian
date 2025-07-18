using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using TextureLibrarian.Models;

namespace TextureLibrarian.Services
{
    public class DragDropService
    {
        [DllImport("shell32.dll")]
        private static extern IntPtr SHCreateItemFromParsingName(
            [MarshalAs(UnmanagedType.LPWStr)] string pszPath,
            IntPtr pbc,
            [MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            out IntPtr ppv);

        public void StartDragDrop(TextureItem texture, TexturePassType passType = TexturePassType.BaseColor)
        {
            try
            {
                var targetPass = texture.Passes.FirstOrDefault(p => p.Type == passType);
                if (targetPass == null)
                    targetPass = texture.Passes.FirstOrDefault();

                if (targetPass != null && File.Exists(targetPass.FilePath))
                {
                    var dataObject = new DataObject();
                    var fileList = new List<string> { targetPass.FilePath };
                    dataObject.SetData(DataFormats.FileDrop, fileList.ToArray());

                    DoDragDrop(dataObject, DragDropEffects.Copy);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during drag operation: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DoDragDrop(DataObject dataObject, DragDropEffects allowedEffects)
        {
            // This is a simplified version - in a full implementation you'd want to use
            // the Windows Shell API to properly simulate file drag from the application
            System.Windows.DragDrop.DoDragDrop(
                (DependencyObject)System.Windows.Application.Current.MainWindow,
                dataObject,
                allowedEffects);
        }
    }
}