using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.IO;
using System.Windows.Media;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using OpenCvSharp;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ImageForensics
{
    public partial class MainWindow : System.Windows.Window
    {
        private string currentImagePath;
        private Mat originalImage;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnLoadImage_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                currentImagePath = openFileDialog.FileName;
                originalImage = Cv2.ImRead(currentImagePath);
                if (originalImage.Empty())
                {
                    MessageBox.Show("Không thể đọc ảnh. Vui lòng thử lại với ảnh khác.");
                    return;
                }

                // Hiển thị ảnh gốc
                using (var ms = new MemoryStream())
                {
                    Cv2.ImEncode(".png", originalImage, out byte[] buffer);
                    ms.Write(buffer, 0, buffer.Length);
                    ms.Position = 0;

                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = ms;
                    bitmap.EndInit();
                    bitmap.Freeze();

                    imgOriginal.Source = bitmap;
                }

                // Enable tất cả các nút phân tích
                btnAnalyzeELA.IsEnabled = true;
                btnAnalyzeJPEGGhost.IsEnabled = true;
                btnAnalyzeNoise.IsEnabled = true;
                btnAnalyzeDCT.IsEnabled = true;
                btnAnalyzeZoom.IsEnabled = true;
                btnAnalyzeCFA.IsEnabled = true;
                btnAnalyzeADJPEG.IsEnabled = true;
                btnAnalyzeClone.IsEnabled = true;
            }
        }

        private async void btnAnalyzeELA_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(currentImagePath))
            {
                MessageBox.Show("Vui lòng chọn ảnh trước khi phân tích.");
                return;
            }

            try
            {
                btnAnalyzeELA.IsEnabled = false;
                txtLoading.Text = "Đang phân tích ELA...";
                txtLoading.Visibility = Visibility.Visible;

                var result = await Task.Run(() => PerformELAAnalysisOpenCV(currentImagePath));

                imgAnalyzed.Source = result;
                txtLoading.Text = "Phân tích ELA hoàn tất.";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi phân tích ELA: {ex.Message}");
                txtLoading.Text = "Lỗi khi phân tích.";
            }
            finally
            {
                btnAnalyzeELA.IsEnabled = true;
                txtLoading.Visibility = Visibility.Collapsed;
            }
        }

        private async void btnAnalyzeJPEGGhost_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(currentImagePath))
            {
                MessageBox.Show("Vui lòng chọn ảnh trước khi phân tích.");
                return;
            }

            try
            {
                btnAnalyzeJPEGGhost.IsEnabled = false;
                txtLoading.Text = "Đang phân tích JPEG Ghost...";
                txtLoading.Visibility = Visibility.Visible;

                var result = await Task.Run(() => PerformJPEGGhostAnalysisOpenCV(currentImagePath));

                imgAnalyzed.Source = result;
                txtLoading.Text = "Phân tích JPEG Ghost hoàn tất.";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi phân tích JPEG Ghost: {ex.Message}");
                txtLoading.Text = "Lỗi khi phân tích.";
            }
            finally
            {
                btnAnalyzeJPEGGhost.IsEnabled = true;
                txtLoading.Visibility = Visibility.Collapsed;
            }
        }

        private async void btnAnalyzeNoise_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(currentImagePath))
            {
                MessageBox.Show("Vui lòng chọn ảnh trước khi phân tích.");
                return;
            }

            try
            {
                btnAnalyzeNoise.IsEnabled = false;
                txtLoading.Text = "Đang phân tích nhiễu...";
                txtLoading.Visibility = Visibility.Visible;

                var (result, analysis) = await Task.Run(() => PerformNoiseAnalysisOpenCV(currentImagePath));

                imgAnalyzed.Source = result;
                txtNoiseAnalysis.Text = analysis;
                txtLoading.Text = "Phân tích nhiễu hoàn tất.";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi phân tích nhiễu: {ex.Message}");
                txtLoading.Text = "Lỗi khi phân tích.";
            }
            finally
            {
                btnAnalyzeNoise.IsEnabled = true;
                txtLoading.Visibility = Visibility.Collapsed;
            }
        }

        private async void btnAnalyzeDCT_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(currentImagePath))
            {
                MessageBox.Show("Vui lòng chọn ảnh trước khi phân tích.");
                return;
            }

            try
            {
                btnAnalyzeDCT.IsEnabled = false;
                txtLoading.Text = "Đang phân tích DCT...";
                txtLoading.Visibility = Visibility.Visible;

                var result = await Task.Run(() => PerformDCTAnalysisOpenCV(currentImagePath));

                imgAnalyzed.Source = result;
                txtLoading.Text = "Phân tích DCT hoàn tất.";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi phân tích DCT: {ex.Message}");
                txtLoading.Text = "Lỗi khi phân tích.";
            }
            finally
            {
                btnAnalyzeDCT.IsEnabled = true;
                txtLoading.Visibility = Visibility.Collapsed;
            }
        }

        private async void btnAnalyzeZoom_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(currentImagePath))
            {
                MessageBox.Show("Vui lòng chọn ảnh trước khi phân tích.");
                return;
            }

            try
            {
                btnAnalyzeZoom.IsEnabled = false;
                txtLoading.Text = "Đang phân tích Zoom Test...";
                txtLoading.Visibility = Visibility.Visible;

                var result = await Task.Run(() => PerformZoomTestAnalysisOpenCV(currentImagePath));

                // Cập nhật giao diện trên luồng UI chính
                Dispatcher.Invoke(() =>
                {
                    imgAnalyzed.Source = result;
                    txtLoading.Text = "Phân tích Zoom Test hoàn tất.";
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi phân tích Zoom Test: {ex.Message}");
                txtLoading.Text = "Lỗi khi phân tích.";
            }
            finally
            {
                // Quay trở lại luồng UI để cập nhật giao diện
                Dispatcher.Invoke(() =>
                {
                    btnAnalyzeZoom.IsEnabled = true;
                    txtLoading.Visibility = Visibility.Collapsed;
                });
            }
        }

        private async void btnAnalyzeCFA_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(currentImagePath))
            {
                MessageBox.Show("Vui lòng chọn ảnh trước khi phân tích.");
                return;
            }

            try
            {
                btnAnalyzeCFA.IsEnabled = false;
                txtLoading.Text = "Đang phân tích CFA...";
                txtLoading.Visibility = Visibility.Visible;

                var result = await Task.Run(() => PerformCFAAnalysisOpenCV(currentImagePath));

                // Cập nhật giao diện trên luồng UI chính
                Dispatcher.Invoke(() =>
                {
                    imgAnalyzed.Source = result;
                    txtLoading.Text = "Phân tích CFA hoàn tất.";
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi phân tích CFA: {ex.Message}");
                txtLoading.Text = "Lỗi khi phân tích.";
            }
            finally
            {
                // Quay trở lại luồng UI để cập nhật giao diện
                Dispatcher.Invoke(() =>
                {
                    btnAnalyzeCFA.IsEnabled = true;
                    txtLoading.Visibility = Visibility.Collapsed;
                });
            }
        }

        private void btnAnalyzeADJPEG_Click(object sender, RoutedEventArgs e)
        {
            PerformADJPEGAnalysis();
        }

        private void btnAnalyzeClone_Click(object sender, RoutedEventArgs e)
        {
            if (originalImage == null)
            {
                MessageBox.Show("Please load an image first.");
                return;
            }

            try
            {
                PerformCloneDetection();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error performing Clone Detection: {ex.Message}");
            }
        }

        private void PerformCloneDetection()
        {
            using (var src = originalImage.Clone())
            {
                // Resize ảnh về tối đa 512x512 để tăng tốc
                int maxDim = 512;
                double scale = 1.0;
                if (src.Width > maxDim || src.Height > maxDim)
                {
                    scale = Math.Min((double)maxDim / src.Width, (double)maxDim / src.Height);
                    Cv2.Resize(src, src, new OpenCvSharp.Size(), scale, scale);
                }

                // Chuyển đổi ảnh sang grayscale
                using (var gray = new Mat())
                {
                    Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);

                    int blockSize = 16;
                    int stepSize = 8;
                    int minDistance = 32; // loại trừ các khối quá gần

                    // 1. Tính hash cho từng khối và lưu vị trí
                    var hashDict = new Dictionary<string, List<OpenCvSharp.Point>>();
                    int width = gray.Cols;
                    int height = gray.Rows;

                    for (int y = 0; y <= height - blockSize; y += stepSize)
                    {
                        for (int x = 0; x <= width - blockSize; x += stepSize)
                        {
                            using (var block = new Mat(gray, new OpenCvSharp.Rect(x, y, blockSize, blockSize)))
                            {
                                string hash = CalculateBlockHash(block);
                                if (!hashDict.ContainsKey(hash))
                                    hashDict[hash] = new List<OpenCvSharp.Point>();
                                hashDict[hash].Add(new OpenCvSharp.Point(x, y));
                            }
                        }
                    }

                    // 2. Tìm các cặp khối có hash giống nhau và vector dịch chuyển
                    var vectorDict = new Dictionary<(int dx, int dy), List<(OpenCvSharp.Point, OpenCvSharp.Point)>>();
                    foreach (var entry in hashDict)
                    {
                        var points = entry.Value;
                        if (points.Count < 2) continue;
                        for (int i = 0; i < points.Count; i++)
                        {
                            for (int j = i + 1; j < points.Count; j++)
                            {
                                var p1 = points[i];
                                var p2 = points[j];
                                int dx = p2.X - p1.X;
                                int dy = p2.Y - p1.Y;
                                // Loại trừ các khối quá gần nhau
                                if (Math.Abs(dx) < minDistance && Math.Abs(dy) < minDistance)
                                    continue;
                                var key = (dx, dy);
                                if (!vectorDict.ContainsKey(key))
                                    vectorDict[key] = new List<(OpenCvSharp.Point, OpenCvSharp.Point)>();
                                vectorDict[key].Add((p1, p2));
                            }
                        }
                    }

                    // 3. Chỉ giữ lại các vector xuất hiện nhiều lần (clone thực sự)
                    int minPairs = 3; // ít nhất 3 cặp vùng giống nhau mới coi là clone thực sự
                    var clonePairs = new List<(OpenCvSharp.Point, OpenCvSharp.Point)>();
                    foreach (var entry in vectorDict)
                    {
                        if (entry.Value.Count >= minPairs)
                        {
                            clonePairs.AddRange(entry.Value);
                        }
                    }

                    // 4. Đánh dấu các vùng clone và vùng gốc
                    using (var result = src.Clone())
                    {
                        foreach (var (p1, p2) in clonePairs)
                        {
                            // Đánh dấu vùng gốc (xanh)
                            Cv2.Rectangle(result, p1, new OpenCvSharp.Point(p1.X + blockSize, p1.Y + blockSize), new Scalar(0, 255, 0), 2);
                            // Đánh dấu vùng clone (đỏ)
                            Cv2.Rectangle(result, p2, new OpenCvSharp.Point(p2.X + blockSize, p2.Y + blockSize), new Scalar(0, 0, 255), 2);
                            // Vẽ đường nối
                            Cv2.Line(result, new OpenCvSharp.Point(p1.X + blockSize/2, p1.Y + blockSize/2), new OpenCvSharp.Point(p2.X + blockSize/2, p2.Y + blockSize/2), new Scalar(255, 255, 0), 1);
                        }

                        // Thêm chú thích
                        Cv2.PutText(result, "Clone Detection (Green: Original, Red: Cloned)", new OpenCvSharp.Point(10, 30), HersheyFonts.HersheyComplexSmall, 1.0, Scalar.White, 2);
                        // Hiển thị kết quả
                        DisplayImage(result, "Clone Detection");
                    }
                }
            }
        }

        private string CalculateBlockHash(Mat block)
        {
            int byteLen = (int)(block.Total() * block.ElemSize());
            byte[] blockData = new byte[byteLen];
            Marshal.Copy(block.Ptr(0), blockData, 0, byteLen);
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(blockData);
                return BitConverter.ToString(hashBytes).Replace("-", "");
            }
        }

        private BitmapImage PerformELAAnalysisOpenCV(string imagePath)
        {
            using (var original = Cv2.ImRead(imagePath))
            {
                // Tạo ảnh tạm thời với chất lượng thấp
                var tempPath = Path.GetTempFileName() + ".jpg";
                Cv2.ImWrite(tempPath, original, new int[] { (int)ImwriteFlags.JpegQuality, 90 });

                // Đọc lại ảnh đã nén
                using (var compressed = Cv2.ImRead(tempPath))
                {
                    // Tính toán sự khác biệt
                    using (var diff = new Mat())
                    {
                        Cv2.Absdiff(original, compressed, diff);

                        // Chuyển đổi sang grayscale
                        using (var gray = new Mat())
                        {
                            Cv2.CvtColor(diff, gray, ColorConversionCodes.BGR2GRAY);

                            // Áp dụng Gaussian blur để giảm nhiễu
                            using (var blurred = new Mat())
                            {
                                Cv2.GaussianBlur(gray, blurred, new OpenCvSharp.Size(3, 3), 0);

                                // Chuẩn hóa kết quả
                                using (var normalized = new Mat())
                                {
                                    Cv2.Normalize(blurred, normalized, 0, 255, NormTypes.MinMax);

                                    // Áp dụng grayscale thay vì colormap
                                    using (var colored = new Mat())
                                    {
                                        Cv2.CvtColor(normalized, colored, ColorConversionCodes.GRAY2BGR);

                                        // Chuyển đổi kết quả thành BitmapImage
                                        Cv2.ImEncode(".png", colored, out byte[] buffer);
                                        using (var ms = new MemoryStream(buffer))
                                        {
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
                            }
                        }
                    }
                }
            }
        }

        private BitmapImage PerformJPEGGhostAnalysisOpenCV(string imagePath)
        {
            using (var original = Cv2.ImRead(imagePath))
            {
                var qualityLevels = new[] { 30, 40, 50, 60, 70, 80, 90 };
                var maxDiff = new Mat(original.Size(), MatType.CV_8UC3, Scalar.All(0));

                foreach (var quality in qualityLevels)
                {
                    // Tạo ảnh tạm thời với chất lượng cụ thể
                    var tempPath = Path.GetTempFileName() + ".jpg";
                    Cv2.ImWrite(tempPath, original, new int[] { (int)ImwriteFlags.JpegQuality, quality });

                    // Đọc lại ảnh đã nén
                    using (var compressed = Cv2.ImRead(tempPath))
                    {
                        // Tính toán sự khác biệt
                        using (var diff = new Mat())
                        {
                            Cv2.Absdiff(original, compressed, diff);
                            Cv2.Max(maxDiff, diff, maxDiff);
                        }
                    }

                    // Xóa file tạm
                    try { File.Delete(tempPath); } catch { }
                }

                // Chuyển đổi sang grayscale
                using (var gray = new Mat())
                {
                    Cv2.CvtColor(maxDiff, gray, ColorConversionCodes.BGR2GRAY);

                    // Áp dụng Gaussian blur
                    using (var blurred = new Mat())
                    {
                        Cv2.GaussianBlur(gray, blurred, new OpenCvSharp.Size(3, 3), 0);

                        // Chuẩn hóa kết quả
                        using (var normalized = new Mat())
                        {
                            Cv2.Normalize(blurred, normalized, 0, 255, NormTypes.MinMax);

                            // Áp dụng grayscale thay vì colormap
                            using (var colored = new Mat())
                            {
                                Cv2.CvtColor(normalized, colored, ColorConversionCodes.GRAY2BGR);

                                // Chuyển đổi kết quả thành BitmapImage
                                Cv2.ImEncode(".png", colored, out byte[] buffer);
                                using (var ms = new MemoryStream(buffer))
                                {
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
                    }
                }
            }
        }

        private (BitmapImage result, string analysis) PerformNoiseAnalysisOpenCV(string imagePath)
        {
            using (var original = Cv2.ImRead(imagePath))
            {
                // Làm mượt ảnh để lấy phần nền (Gaussian blur)
                using (var blurred = new Mat())
                using (var noise = new Mat())
                {
                    Cv2.GaussianBlur(original, blurred, new OpenCvSharp.Size(3, 3), 0);
                    Cv2.Absdiff(original, blurred, noise);

                    // Tính mức nhiễu tổng thể (stddev trên toàn bộ ảnh)
                    Cv2.MeanStdDev(noise, out _, out Scalar stddevScalar);
                    double stddev = (stddevScalar.Val0 + stddevScalar.Val1 + stddevScalar.Val2) / 3.0;

                    // Tạo mask cho các vùng có nhiễu cao
                    using (var mask = new Mat())
                    {
                        // Chuyển sang grayscale để dễ xử lý
                        using (var gray = new Mat())
                        {
                            Cv2.CvtColor(noise, gray, ColorConversionCodes.BGR2GRAY);
                            
                            // Ngưỡng để xác định vùng có nhiễu cao (2 lần stddev)
                            double threshold = stddev * 2;
                            Cv2.Threshold(gray, mask, threshold, 255, ThresholdTypes.Binary);
                        }

                        // Tính toán tỷ lệ vùng nhiễu cao
                        double noiseArea = Cv2.CountNonZero(mask);
                        double totalArea = mask.Rows * mask.Cols;
                        double noiseRatio = (noiseArea / totalArea) * 100;

                        // Tạo ảnh kết quả với đánh dấu vùng nhiễu cao
                        using (var display = original.Clone())
                        {
                            // Đánh dấu vùng nhiễu cao bằng màu đỏ
                            display.SetTo(new Scalar(0, 0, 255), mask);

                            // Đánh giá mức độ nhiễu
                            string noiseLevel;
                            if (stddev < 8)
                                noiseLevel = "Low (Natural)";
                            else if (stddev < 15)
                                noiseLevel = "Medium (Possible editing)";
                            else
                                noiseLevel = "High (Likely edited)";

                            // Đánh giá phân bố nhiễu
                            string distribution;
                            if (noiseRatio < 5)
                                distribution = "Even (Natural)";
                            else if (noiseRatio < 15)
                                distribution = "Moderate (Possible editing)";
                            else
                                distribution = "Uneven (Likely edited)";

                            // Hiển thị thông tin lên ảnh
                            int y = 30;
                            HersheyFonts fontFace = HersheyFonts.HersheySimplex;
                            double fontScale = 0.7;
                            int thickness = 2;
                            Scalar color = new Scalar(0, 0, 255); // Red
                            int lineHeight = 25;

                            Cv2.PutText(display, $"Noise Level (Std): {stddev:F2}", new OpenCvSharp.Point(10, y), fontFace, fontScale, color, thickness, LineTypes.AntiAlias);
                            y += lineHeight;
                            Cv2.PutText(display, $"Noise Level: {noiseLevel}", new OpenCvSharp.Point(10, y), fontFace, fontScale, color, thickness, LineTypes.AntiAlias);
                            y += lineHeight;
                            Cv2.PutText(display, $"Noise Distribution: {distribution}", new OpenCvSharp.Point(10, y), fontFace, fontScale, color, thickness, LineTypes.AntiAlias);
                            y += lineHeight;
                            Cv2.PutText(display, $"High Noise Area: {noiseRatio:F1}%", new OpenCvSharp.Point(10, y), fontFace, fontScale, color, thickness, LineTypes.AntiAlias);

                            // Chuyển đổi kết quả thành BitmapImage
                            Cv2.ImEncode(".png", display, out byte[] buffer);
                            using (var ms = new MemoryStream(buffer))
                            {
                                var bitmap = new BitmapImage();
                                bitmap.BeginInit();
                                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                                bitmap.StreamSource = ms;
                                bitmap.EndInit();
                                bitmap.Freeze();
                                return (bitmap, "");
                            }
                        }
                    }
                }
            }
        }

        private BitmapImage PerformDCTAnalysisOpenCV(string imagePath)
        {
            using (var original = Cv2.ImRead(imagePath))
            {
                // Chuyển ảnh sang grayscale
                using (var gray = new Mat())
                {
                    Cv2.CvtColor(original, gray, ColorConversionCodes.BGR2GRAY);

                    // Đảm bảo kích thước ảnh là bội số của 8
                    int rows = (gray.Rows / 8) * 8;
                    int cols = (gray.Cols / 8) * 8;
                    using (var resized = new Mat(gray, new OpenCvSharp.Rect(0, 0, cols, rows)))
                    {
                        // Chuyển đổi sang float32 để thực hiện DCT
                        using (var floatImg = new Mat())
                        {
                            resized.ConvertTo(floatImg, MatType.CV_32F);

                            // Tạo ma trận kết quả cho độ lệch chuẩn
                            using (var stddevResult = new Mat(rows, cols, MatType.CV_32F))
                            {
                                // Thực hiện DCT trên từng khối 8x8 và tính độ lệch chuẩn AC
                                for (int i = 0; i < rows; i += 8)
                                {
                                    for (int j = 0; j < cols; j += 8)
                                    {
                                        using (var block = new Mat(floatImg, new OpenCvSharp.Rect(j, i, 8, 8)))
                                        using (var dctBlock = new Mat())
                                        {
                                            // Thực hiện DCT trên khối hiện tại
                                            Cv2.Dct(block, dctBlock);

                                            // Lấy 63 hệ số AC (bỏ qua DC component)
                                            List<float> acValues = new List<float>();
                                            for (int y = 0; y < 8; y++)
                                            {
                                                for (int x = 0; x < 8; x++)
                                                {
                                                    if (x != 0 || y != 0) // Bỏ qua DC component
                                                    {
                                                        acValues.Add(Math.Abs(dctBlock.At<float>(y, x)));
                                                    }
                                                }
                                            }

                                            // Tính độ lệch chuẩn của các hệ số AC
                                            double mean = acValues.Average();
                                            double variance = acValues.Sum(v => Math.Pow(v - mean, 2)) / acValues.Count;
                                            float stddev = (float)Math.Sqrt(variance);

                                            // Lưu độ lệch chuẩn vào ma trận kết quả
                                            for (int y = 0; y < 8; y++)
                                            {
                                                for (int x = 0; x < 8; x++)
                                                {
                                                    stddevResult.Set(i + y, j + x, stddev);
                                                }
                                            }
                                        }
                                    }
                                }

                                // Chuẩn hóa kết quả
                                using (var normalized = new Mat())
                                {
                                    Cv2.Normalize(stddevResult, normalized, 0, 255, NormTypes.MinMax);
                                    normalized.ConvertTo(normalized, MatType.CV_8U);

                                    // Tạo ảnh màu từ DCT
                                    using (var colored = new Mat())
                                    {
                                        // Sử dụng grayscale
                                        Cv2.CvtColor(normalized, colored, ColorConversionCodes.GRAY2BGR);

                                        // Thêm thông tin phân tích
                                        string text = "DCT Analysis - Forensic";
                                        HersheyFonts fontFace = HersheyFonts.HersheySimplex;
                                        double fontScale = 1.2;
                                        int thickness = 3;
                                        Scalar color = new Scalar(0, 0, 0); // Black
                                        int y = 40;
                                        int lineHeight = 40;

                                        // Tiêu đề
                                        Cv2.PutText(colored, text, new OpenCvSharp.Point(20, y), fontFace, fontScale, color, thickness, LineTypes.AntiAlias);
                                        y += lineHeight;

                                        // Hướng dẫn đọc kết quả
                                        string[] guides = new string[]
                                        {
                                            "White/Bright: High StdDev of AC (possible editing)",
                                            "Dark/Gray: Low StdDev of AC (likely original)",
                                            "Bright Areas: Modified DCT patterns",
                                            "Dark Areas: Original DCT patterns"
                                        };

                                        foreach (var guide in guides)
                                        {
                                            Cv2.PutText(colored, guide, new OpenCvSharp.Point(20, y), fontFace, fontScale, color, thickness, LineTypes.AntiAlias);
                                            y += lineHeight;
                                        }

                                        // Thêm thông tin về phương pháp
                                        string[] methodInfo = new string[]
                                        {
                                            "Method: JPEG DCT Analysis",
                                            "Analysis: StdDev of AC coefficients",
                                            "Purpose: Detect variations in DCT patterns"
                                        };

                                        foreach (var info in methodInfo)
                                        {
                                            Cv2.PutText(colored, info, new OpenCvSharp.Point(20, y), fontFace, fontScale, color, thickness, LineTypes.AntiAlias);
                                            y += lineHeight;
                                        }

                                        // Chuyển đổi kết quả thành BitmapImage
                                        Cv2.ImEncode(".png", colored, out byte[] buffer);
                                        using (var ms = new MemoryStream(buffer))
                                        {
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
                            }
                        }
                    }
                }
            }
        }

        private BitmapImage PerformZoomTestAnalysisOpenCV(string imagePath)
        {
            using (var original = Cv2.ImRead(imagePath))
            {
                if (original.Empty())
                {
                     System.Diagnostics.Debug.WriteLine($"[DEBUG] ZoomTest: Không thể đọc ảnh từ {imagePath}");
                     return null; // Xử lý trường hợp không đọc được ảnh
                }
                 System.Diagnostics.Debug.WriteLine($"[DEBUG] ZoomTest: Đọc ảnh gốc thành công, kích thước: {original.Size()}");

                // Chuyển ảnh sang grayscale
                using (var gray = new Mat())
                {
                    Cv2.CvtColor(original, gray, ColorConversionCodes.BGR2GRAY);

                    // Tính toán bản đồ tương quan cục bộ
                    using (var correlationMap = new Mat(gray.Size(), MatType.CV_32F, Scalar.All(0)))
                    {
                        // Tính toán sự khác biệt bình phương với pixel lân cận (ngang và dọc)
                        for (int i = 0; i < gray.Rows - 1; i++)
                        {
                            for (int j = 0; j < gray.Cols - 1; j++)
                            {
                                float diffX = gray.At<byte>(i, j + 1) - gray.At<byte>(i, j);
                                float diffY = gray.At<byte>(i + 1, j) - gray.At<byte>(i, j);
                                correlationMap.Set(i, j, diffX * diffX + diffY * diffY); // Tổng bình phương sai lệch
                            }
                        }

                        // Áp dụng Gaussian blur để làm mượt kết quả
                        using (var blurredCorrelation = new Mat())
                        {
                            Cv2.GaussianBlur(correlationMap, blurredCorrelation, new OpenCvSharp.Size(5, 5), 0);

                            // Chuẩn hóa kết quả
                            using (var normalized = new Mat())
                            {
                                // Chuẩn hóa về dải 0-255
                                Cv2.Normalize(blurredCorrelation, normalized, 0, 255, NormTypes.MinMax);
                                normalized.ConvertTo(normalized, MatType.CV_8U);

                                // Tạo ảnh màu từ kết quả
                                using (var colored = new Mat())
                                {
                                    // Sử dụng grayscale
                                    Cv2.CvtColor(normalized, colored, ColorConversionCodes.GRAY2BGR);

                                    // Thêm thông tin phân tích
                                    string text = "Zoom Test Analysis - Local Correlation";
                                    HersheyFonts fontFace = HersheyFonts.HersheySimplex;
                                    double fontScale = 1.2;
                                    int thickness = 3;
                                    Scalar color = new Scalar(0, 0, 0); // Black
                                    int y = 40;
                                    int lineHeight = 40;

                                    // Tiêu đề
                                    Cv2.PutText(colored, text, new OpenCvSharp.Point(20, y), fontFace, fontScale, color, thickness, LineTypes.AntiAlias);
                                    y += lineHeight;

                                    // Hướng dẫn đọc kết quả
                                    string[] guides = new string[]
                                    {
                                        "White/Bright: High Local Correlation (possible resampling/editing)",
                                        "Dark/Gray: Low Local Correlation (likely original)",
                                        "Look for uniform or unnatural patterns"
                                    };

                                    foreach (var guide in guides)
                                    {
                                        Cv2.PutText(colored, guide, new OpenCvSharp.Point(20, y), fontFace, fontScale, color, thickness, LineTypes.AntiAlias);
                                        y += lineHeight;
                                    }

                                    // Thêm thông tin về phương pháp
                                    string[] methodInfo = new string[]
                                    {
                                        "Method: Local Pixel Correlation Analysis",
                                        "Analysis: Detects altered relationships between neighboring pixels",
                                        "Purpose: Identify resampling and interpolation artifacts"
                                    };

                                    foreach (var info in methodInfo)
                                    {
                                        Cv2.PutText(colored, info, new OpenCvSharp.Point(20, y), fontFace, fontScale, color, thickness, LineTypes.AntiAlias);
                                        y += lineHeight;
                                    }

                                    // Chuyển đổi kết quả thành BitmapImage
                                    Cv2.ImEncode(".png", colored, out byte[] buffer);
                                    using (var ms = new MemoryStream(buffer))
                                    {
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
                        }
                    }
                }
            }
        }

        private BitmapImage PerformCFAAnalysisOpenCV(string imagePath)
        {
            using (var original = Cv2.ImRead(imagePath, ImreadModes.Color))
            {
                if (original.Empty())
                {
                    System.Diagnostics.Debug.WriteLine($"[DEBUG] CFAAnalysis: Không thể đọc ảnh từ {imagePath}");
                    return null; // Xử lý trường hợp không đọc được ảnh
                }
                System.Diagnostics.Debug.WriteLine($"[DEBUG] CFAAnalysis: Đọc ảnh gốc thành công, kích thước: {original.Size()}");

                // Tách các kênh màu (B, G, R)
                Mat[] channels = original.Split();
                using (Mat b = channels[0])
                using (Mat g = channels[1])
                using (Mat r = channels[2])
                {
                    // Tạo ảnh kết quả cho bản đồ CFA
                    using (var cfaMap = new Mat(original.Size(), MatType.CV_32F, Scalar.All(0)))
                    {
                        // Kích thước cửa sổ phân tích
                        int windowSize = 3;
                        int halfWindow = windowSize / 2;

                        // Duyệt qua ảnh, bỏ qua viền
                        for (int i = halfWindow; i < original.Rows - halfWindow; i++)
                        {
                            for (int j = halfWindow; j < original.Cols - halfWindow; j++)
                            {
                                // Xác định vị trí pixel trong mẫu Bayer (ví dụ: RGGB)
                                bool isRed = (i % 2 == 0 && j % 2 == 0);
                                bool isGreenR = (i % 2 == 0 && j % 2 == 1);
                                bool isGreenB = (i % 2 == 1 && j % 2 == 0);
                                bool isBlue = (i % 2 == 1 && j % 2 == 1);

                                float residual;

                                // Tính phần dư tùy thuộc vào vị trí pixel và kênh màu
                                if (isRed)
                                {
                                    // Tại vị trí R, dự đoán R từ G lân cận
                                    float predictedR = (g.At<byte>(i, j - 1) + g.At<byte>(i, j + 1) + g.At<byte>(i - 1, j) + g.At<byte>(i + 1, j)) / 4.0f;
                                    residual = Math.Abs(r.At<byte>(i, j) - predictedR);
                                }
                                else if (isGreenR || isGreenB)
                                {
                                    // Tại vị trí G (có 2 loại), dự đoán G từ R và B lân cận
                                    float predictedG_R = (r.At<byte>(i, j - 1) + r.At<byte>(i, j + 1)) / 2.0f; // Nếu G nằm ngang với R
                                    float predictedG_B = (b.At<byte>(i - 1, j) + b.At<byte>(i + 1, j)) / 2.0f; // Nếu G nằm dọc với B

                                    if (isGreenR) // G nằm trên hàng R
                                    {
                                        predictedG_B = (b.At<byte>(i - 1, j) + b.At<byte>(i + 1, j)) / 2.0f; // Thêm dự đoán theo chiều dọc từ B
                                         // Dự đoán G từ R và B lân cận (có trọng số có thể cần)
                                        residual = Math.Abs(g.At<byte>(i, j) - (predictedG_R + predictedG_B)/2.0f);
                                    }
                                    else // isGreenB - G nằm trên hàng B
                                    {
                                        predictedG_R = (r.At<byte>(i, j - 1) + r.At<byte>(i, j + 1)) / 2.0f; // Thêm dự đoán theo chiều ngang từ R
                                         // Dự đoán G từ R và B lân cận
                                        residual = Math.Abs(g.At<byte>(i, j) - (predictedG_R + predictedG_B) / 2.0f);
                                    }

                                }
                                else // isBlue
                                {
                                    // Tại vị trí B, dự đoán B từ G lân cận
                                    float predictedB = (g.At<byte>(i, j - 1) + g.At<byte>(i, j + 1) + g.At<byte>(i - 1, j) + g.At<byte>(i + 1, j)) / 4.0f;
                                    residual = Math.Abs(b.At<byte>(i, j) - predictedB);
                                }

                                // Lưu giá trị phần dư vào bản đồ CFA
                                cfaMap.Set(i, j, residual);
                            }
                        }

                        // Chuẩn hóa kết quả
                        using (var normalized = new Mat())
                        {
                            Cv2.Normalize(cfaMap, normalized, 0, 255, NormTypes.MinMax);
                            normalized.ConvertTo(normalized, MatType.CV_8U);

                            // Sử dụng ảnh màu kết quả để thêm text
                            using (var colored = normalized.Clone())
                            {
                                // Thêm thông tin phân tích
                                string text = "CFA Analysis - Residuals";
                                HersheyFonts fontFace = HersheyFonts.HersheySimplex;
                                double fontScale = 1.2;
                                int thickness = 3;
                                Scalar color = new Scalar(0, 0, 0); // Black
                                int y = 40;
                                int lineHeight = 40;

                                // Tiêu đề
                                Cv2.PutText(colored, text, new OpenCvSharp.Point(20, y), fontFace, fontScale, color, thickness, LineTypes.AntiAlias);
                                y += lineHeight;

                                // Hướng dẫn đọc kết quả
                                string[] guides = new string[]
                                {
                                    "White/Bright: High Residuals (possible CFA artifacts/editing)",
                                    "Dark/Gray: Low Residuals (likely original)",
                                    "Look for unnatural patterns or uniform areas in the presence of texture"
                                };

                                foreach (var guide in guides)
                                {
                                    Cv2.PutText(colored, guide, new OpenCvSharp.Point(20, y), fontFace, fontScale, color, thickness, LineTypes.AntiAlias);
                                    y += lineHeight;
                                }

                                // Thêm thông tin về phương pháp
                                string[] methodInfo = new string[]
                                {
                                    "Method: CFA Residual Analysis",
                                    "Analysis: Detects deviations from expected CFA pattern",
                                    "Purpose: Identify areas potentially not processed by standard demosaicing"
                                };

                                foreach (var info in methodInfo)
                                {
                                    Cv2.PutText(colored, info, new OpenCvSharp.Point(20, y), fontFace, fontScale, color, thickness, LineTypes.AntiAlias);
                                    y += lineHeight;
                                }

                                // Chuyển đổi kết quả thành BitmapImage
                                Cv2.ImEncode(".png", colored, out byte[] buffer);
                                using (var ms = new MemoryStream(buffer))
                                {
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
                    }
                }
            }
        }

        private void DisplayImage(Mat image, string title)
        {
            using (var ms = new MemoryStream())
            {
                Cv2.ImEncode(".png", image, out byte[] buffer);
                ms.Write(buffer, 0, buffer.Length);
                ms.Position = 0;

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = ms;
                bitmap.EndInit();
                bitmap.Freeze();

                imgAnalyzed.Source = bitmap;
            }
        }

        private void PerformADJPEGAnalysis()
        {
            if (originalImage == null)
            {
                MessageBox.Show("Please load an image first.");
                return;
            }

            try
            {
                using (var src = originalImage.Clone())
                {
                    // Chuyển đổi ảnh sang grayscale
                    using (var gray = new Mat())
                    {
                        Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);

                        // Tạo ma trận kết quả
                        using (var adjpegMap = new Mat(gray.Size(), MatType.CV_32F))
                        {
                            // Kích thước khối DCT
                            int blockSize = 8;
                            int width = gray.Cols;
                            int height = gray.Rows;

                            // Duyệt qua từng khối 8x8
                            for (int y = 0; y < height - blockSize; y += blockSize)
                            {
                                for (int x = 0; x < width - blockSize; x += blockSize)
                                {
                                    // Lấy khối 8x8
                                    using (var block = new Mat(gray, new OpenCvSharp.Rect(x, y, blockSize, blockSize)))
                                    {
                                        // Chuyển đổi sang float
                                        using (var blockFloat = new Mat())
                                        {
                                            block.ConvertTo(blockFloat, MatType.CV_32F);

                                            // Áp dụng DCT
                                            using (var dct = new Mat())
                                            {
                                                Cv2.Dct(blockFloat, dct);

                                                // Tính toán các đặc trưng ADJPEG
                                                float sumAC = 0;
                                                float sumAC2 = 0;
                                                int acCount = 0;

                                                // Bỏ qua hệ số DC (0,0)
                                                for (int i = 0; i < blockSize; i++)
                                                {
                                                    for (int j = 0; j < blockSize; j++)
                                                    {
                                                        if (i == 0 && j == 0) continue;

                                                        float coeff = dct.Get<float>(i, j);
                                                        sumAC += Math.Abs(coeff);
                                                        sumAC2 += coeff * coeff;
                                                        acCount++;
                                                    }
                                                }

                                                // Tính toán các đặc trưng
                                                float meanAC = sumAC / acCount;
                                                float varAC = (sumAC2 / acCount) - (meanAC * meanAC);
                                                float skewness = 0;

                                                // Tính skewness
                                                for (int i = 0; i < blockSize; i++)
                                                {
                                                    for (int j = 0; j < blockSize; j++)
                                                    {
                                                        if (i == 0 && j == 0) continue;
                                                        float coeff = dct.Get<float>(i, j);
                                                        skewness += (float)Math.Pow(coeff - meanAC, 3);
                                                    }
                                                }
                                                skewness = skewness / (acCount * (float)Math.Pow(varAC, 1.5f));

                                                // Tính điểm ADJPEG
                                                float adjpegScore = (float)(Math.Abs(skewness) * Math.Sqrt(varAC));

                                                // Lưu kết quả vào bản đồ
                                                for (int i = 0; i < blockSize; i++)
                                                {
                                                    for (int j = 0; j < blockSize; j++)
                                                    {
                                                        adjpegMap.Set(i + y, j + x, adjpegScore);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            // Chuẩn hóa kết quả
                            using (var normalized = new Mat())
                            {
                                Cv2.Normalize(adjpegMap, normalized, 0, 255, NormTypes.MinMax);
                                normalized.ConvertTo(normalized, MatType.CV_8U);

                                // Tạo ảnh màu từ kết quả grayscale để thêm text
                                using (var colored = new Mat())
                                {
                                    Cv2.CvtColor(normalized, colored, ColorConversionCodes.GRAY2BGR);

                                    // Thêm thông tin phân tích
                                    string text = "ADJPEG Analysis";
                                    Cv2.PutText(colored, text, new OpenCvSharp.Point(10, 30), HersheyFonts.HersheyComplexSmall, 1.0, Scalar.White, 1);
                                    Cv2.PutText(colored, "Bright areas: High JPEG artifacts", new OpenCvSharp.Point(10, 60), HersheyFonts.HersheyComplexSmall, 0.8, Scalar.White, 1);
                                    Cv2.PutText(colored, "Dark areas: Low JPEG artifacts", new OpenCvSharp.Point(10, 90), HersheyFonts.HersheyComplexSmall, 0.8, Scalar.White, 1);

                                    // Hiển thị kết quả
                                    DisplayImage(colored, "ADJPEG Analysis");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error performing ADJPEG analysis: {ex.Message}");
            }
        }
    }
} 