using Microsoft.EntityFrameworkCore;
using RecipesProject.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace RecipesProject
{
    public static class ImageMethods
    {
        //-- Чтение фото
        public static BitmapImage? readImage(string path)
        {
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                var bitmap = new BitmapImage();
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = stream;
                    bitmap.EndInit();
                }
                bitmap.Freeze();

                return bitmap;
            }
            return null;
        }
    }
}
