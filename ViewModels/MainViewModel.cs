using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ImageForensics.Commands;
using ImageForensics.Models;
using ImageForensics.Services;
using Microsoft.Win32;
using OpenCvSharp;

namespace ImageForensics.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IExifService _exifService;
        private string _currentImagePath;
        private Mat _originalImage;
        private BitmapImage _originalImageSource;
        private BitmapImage _analyzedImageSource;
        private string _loadingText;
        private bool _isLoading;
        private ObservableCollection<ExifGroup> _exifGroups;

        public MainViewModel(IExifService exifService)
        {
            _exifService = exifService;
            ExifGroups = new ObservableCollection<ExifGroup>();
            
            LoadImageCommand = new RelayCommand(ExecuteLoadImage);
            AnalyzeExifCommand = new RelayCommand(ExecuteAnalyzeExif, CanAnalyzeExif);
        }

        public ICommand LoadImageCommand { get; }
        public ICommand AnalyzeExifCommand { get; }

        public string CurrentImagePath
        {
            get => _currentImagePath;
            set => SetProperty(ref _currentImagePath, value);
        }

        public BitmapImage OriginalImageSource
        {
            get => _originalImageSource;
            set => SetProperty(ref _originalImageSource, value);
        }

        public BitmapImage AnalyzedImageSource
        {
            get => _analyzedImageSource;
            set => SetProperty(ref _analyzedImageSource, value);
        }

        public string LoadingText
        {
            get => _loadingText;
            set => SetProperty(ref _loadingText, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ObservableCollection<ExifGroup> ExifGroups
        {
            get => _exifGroups;
            set => SetProperty(ref _exifGroups, value);
        }

        private void ExecuteLoadImage(object parameter)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                CurrentImagePath = openFileDialog.FileName;
                _originalImage = Cv2.ImRead(CurrentImagePath);
                if (_originalImage.Empty())
                {
                    System.Windows.MessageBox.Show("Không thể đọc ảnh. Vui lòng thử lại với ảnh khác.");
                    return;
                }

                // Hiển thị ảnh gốc
                using (var ms = new System.IO.MemoryStream())
                {
                    Cv2.ImEncode(".png", _originalImage, out byte[] buffer);
                    ms.Write(buffer, 0, buffer.Length);
                    ms.Position = 0;

                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = ms;
                    bitmap.EndInit();
                    bitmap.Freeze();

                    OriginalImageSource = bitmap;
                }
            }
        }

        private bool CanAnalyzeExif(object parameter)
        {
            return !string.IsNullOrEmpty(CurrentImagePath);
        }

        private async void ExecuteAnalyzeExif(object parameter)
        {
            if (string.IsNullOrEmpty(CurrentImagePath))
            {
                System.Windows.MessageBox.Show("Vui lòng chọn ảnh trước khi phân tích.");
                return;
            }

            try
            {
                IsLoading = true;
                LoadingText = "Đang phân tích EXIF...";

                var exifGroups = await _exifService.GetExifInfoAsync(CurrentImagePath);
                ExifGroups.Clear();
                foreach (var group in exifGroups)
                {
                    ExifGroups.Add(group);
                }

                LoadingText = "Phân tích EXIF hoàn tất.";
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Lỗi khi phân tích EXIF: {ex.Message}");
                LoadingText = "Lỗi khi phân tích.";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
} 