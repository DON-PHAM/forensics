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
using System.IO;
using System.Threading;

namespace ImageForensics.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IExifService _exifService;
        private string _selectedImagePath;
        private Mat _originalImage;
        private BitmapImage _originalImageSource;
        private BitmapImage _analyzedImageSource;
        private string _loadingText;
        private bool _isLoading;
        private ObservableCollection<ExifGroup> _exifGroups;
        private ObservableCollection<ImageItem> _imageList;
        private ImageItem _selectedImage;
        private CancellationTokenSource _currentLoadCancellation;

        public MainViewModel(IExifService exifService)
        {
            _exifService = exifService;
            ExifGroups = new ObservableCollection<ExifGroup>();
            ImageList = new ObservableCollection<ImageItem>();
            
            LoadImageCommand = new RelayCommand(ExecuteLoadImage);
            AnalyzeExifCommand = new RelayCommand(ExecuteAnalyzeExif, CanAnalyzeExif);
        }

        public ICommand LoadImageCommand { get; }
        public ICommand AnalyzeExifCommand { get; }

        public ObservableCollection<ImageItem> ImageList
        {
            get => _imageList;
            set => SetProperty(ref _imageList, value);
        }

        public ImageItem SelectedImage
        {
            get => _selectedImage;
            set
            {
                if (SetProperty(ref _selectedImage, value))
                {
                    if (value != null)
                    {
                        _ = LoadSelectedImageAsync(value.FilePath);
                    }
                }
            }
        }

        public string SelectedImagePath
        {
            get => _selectedImagePath;
            set => SetProperty(ref _selectedImagePath, value);
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
                Filter = "Image files (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp|All files (*.*)|*.*",
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _ = LoadImagesAsync(openFileDialog.FileNames);
            }
        }

        private async Task LoadImagesAsync(string[] filePaths)
        {
            IsLoading = true;
            LoadingText = "Đang tải ảnh...";

            try
            {
                foreach (string filePath in filePaths)
                {
                    var thumbnail = await Task.Run(() => LoadThumbnail(filePath));
                    var imageItem = new ImageItem
                    {
                        FilePath = filePath,
                        FileName = Path.GetFileName(filePath),
                        Thumbnail = thumbnail
                    };
                    ImageList.Add(imageItem);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Lỗi khi tải ảnh: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private BitmapImage LoadThumbnail(string filePath)
        {
            try
            {
                using (var mat = Cv2.ImRead(filePath))
                {
                    if (mat.Empty()) return null;

                    // Resize to thumbnail size
                    var thumbnail = new Mat();
                    Cv2.Resize(mat, thumbnail, new Size(100, 100));
                    
                    using (var ms = new MemoryStream())
                    {
                        Cv2.ImEncode(".png", thumbnail, out byte[] buffer);
                        ms.Write(buffer, 0, buffer.Length);
                        ms.Position = 0;

                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = ms;
                        bitmap.EndInit();
                        bitmap.Freeze();

                        return bitmap;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        private async Task LoadSelectedImageAsync(string filePath)
        {
            // Cancel previous load if any
            _currentLoadCancellation?.Cancel();
            _currentLoadCancellation = new CancellationTokenSource();
            var token = _currentLoadCancellation.Token;

            try
            {
                IsLoading = true;
                LoadingText = "Đang tải ảnh...";

                // Clear previous analysis
                ExifGroups.Clear();
                AnalyzedImageSource = null;

                SelectedImagePath = filePath;

                // Load image in background
                var (image, bitmap) = await Task.Run(() =>
                {
                    var mat = Cv2.ImRead(filePath);
                    if (mat.Empty()) return (null, null);

                    using (var ms = new MemoryStream())
                    {
                        Cv2.ImEncode(".png", mat, out byte[] buffer);
                        ms.Write(buffer, 0, buffer.Length);
                        ms.Position = 0;

                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = ms;
                        bitmap.EndInit();
                        bitmap.Freeze();

                        return (mat, bitmap);
                    }
                }, token);

                if (token.IsCancellationRequested) return;

                _originalImage = image;
                OriginalImageSource = bitmap;
            }
            catch (OperationCanceledException)
            {
                // Ignore cancellation
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Lỗi khi tải ảnh: {ex.Message}");
            }
            finally
            {
                if (!token.IsCancellationRequested)
                {
                    IsLoading = false;
                }
            }
        }

        private bool CanAnalyzeExif(object parameter)
        {
            return !string.IsNullOrEmpty(SelectedImagePath);
        }

        private async void ExecuteAnalyzeExif(object parameter)
        {
            if (string.IsNullOrEmpty(SelectedImagePath))
            {
                System.Windows.MessageBox.Show("Vui lòng chọn ảnh trước khi phân tích.");
                return;
            }

            try
            {
                IsLoading = true;
                LoadingText = "Đang phân tích EXIF...";

                var exifGroups = await _exifService.GetExifInfoAsync(SelectedImagePath);
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