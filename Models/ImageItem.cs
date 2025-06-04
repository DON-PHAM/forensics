using System.Windows.Media.Imaging;

namespace ImageForensics.Models
{
    public class ImageItem
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public BitmapImage Thumbnail { get; set; }
    }
} 