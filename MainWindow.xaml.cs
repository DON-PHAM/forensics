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

                    int blockSize = 8; // Kích thước khối DCT
                    int stepSize = 1; // Bước nhảy
                    int vectorSize = 16; // Số hệ số DCT đầu tiên theo zigzag
                    double oklidThreshold = 3.0; // Ngưỡng Euclidean
                    int correlationThreshold = 20; // Số lượng vector so sánh
                    double vecLenThreshold = 16.0; // Ngưỡng độ dài vector
                    double numOfVectorThreshold = 0.1; // Ngưỡng số lượng vector (10%)

                    // Ma trận quantization cho DCT
                    float[,] QUANTIZATION_MAT_90 = new float[8,8] {
                        {3, 2, 2, 3, 5, 8, 10, 12},
                        {2, 2, 3, 4, 5, 12, 12, 11},
                        {3, 3, 3, 5, 8, 11, 14, 11},
                        {3, 3, 4, 6, 10, 17, 16, 12},
                        {4, 4, 7, 11, 14, 22, 21, 15},
                        {5, 7, 11, 13, 16, 12, 23, 18},
                        {10, 13, 16, 17, 21, 24, 24, 21},
                        {14, 18, 19, 20, 22, 20, 20, 20}
                    };

                    // 1. Chia ảnh thành các khối và lấy đặc trưng DCT
                    var blockVectors = new List<double[]>();
                    int width = gray.Cols;
                    int height = gray.Rows;

                    for (int y = 0; y <= height - blockSize; y += stepSize)
                    {
                        for (int x = 0; x <= width - blockSize; x += stepSize)
                        {
                            using (var block = new Mat(gray, new OpenCvSharp.Rect(x, y, blockSize, blockSize)))
                            {
                                // Lấy đặc trưng DCT
                                var blockFloat = new Mat();
                                block.ConvertTo(blockFloat, MatType.CV_32F);
                                var dct = new Mat();
                                Cv2.Dct(blockFloat, dct);

                                // Quantization - giống Python hơn
                                var dctArray = new float[blockSize, blockSize];
                                for (int i = 0; i < blockSize; i++)
                                {
                                    for (int j = 0; j < blockSize; j++)
                                    {
                                        dctArray[i,j] = dct.At<float>(i, j);
                                    }
                                }

                                // Áp dụng quantization matrix
                                for (int i = 0; i < blockSize; i++)
                                {
                                    for (int j = 0; j < blockSize; j++)
                                    {
                                        dctArray[i,j] = (float)Math.Round(dctArray[i,j] / QUANTIZATION_MAT_90[i,j]);
                                        dctArray[i,j] = (float)Math.Round(dctArray[i,j] / 4.0);
                                        dct.Set(i, j, dctArray[i,j]);
                                    }
                                }

                                // Lấy vector zigzag
                                double[] zigzag = GetZigzagDCT(dct, vectorSize);
                                
                                // Thêm tọa độ vào vector
                                var vector = new double[vectorSize + 2];
                                Array.Copy(zigzag, vector, vectorSize);
                                vector[vectorSize] = x;
                                vector[vectorSize + 1] = y;
                                blockVectors.Add(vector);

                                blockFloat.Dispose();
                                dct.Dispose();
                            }
                        }
                    }

                    // 2. Sắp xếp vector theo thứ tự từ điển - giống Python hơn
                    var blockVectorsArray = blockVectors.ToArray();
                    Array.Sort(blockVectorsArray, (a, b) =>
                    {
                        // So sánh từng phần tử của vector
                        for (int i = 0; i < vectorSize; i++)
                        {
                            int cmp = a[i].CompareTo(b[i]);
                            if (cmp != 0) return cmp;
                        }
                        return 0;
                    });
                    blockVectors = blockVectorsArray.ToList();

                    // 3. So sánh các vector và tìm các cặp tương đồng
                    var similarPairs = new List<(OpenCvSharp.Point, OpenCvSharp.Point, double)>();
                    var houghSpace = new double[height, width, 2];
                    var shiftVectors = new List<(int, int, int, int, int, int, int)>();

                    for (int i = 0; i < blockVectors.Count; i++)
                    {
                        int end = Math.Min(i + correlationThreshold, blockVectors.Count);
                        for (int j = i + 1; j < end; j++)
                        {
                            double dist = EuclideanDistance(blockVectors[i], blockVectors[j], vectorSize);
                            if (dist <= oklidThreshold)
                            {
                                int x1 = (int)blockVectors[i][vectorSize];
                                int y1 = (int)blockVectors[i][vectorSize + 1];
                                int x2 = (int)blockVectors[j][vectorSize];
                                int y2 = (int)blockVectors[j][vectorSize + 1];

                                // Tính vector dịch chuyển
                                int dx = Math.Abs(x2 - x1);
                                int dy = Math.Abs(y2 - y1);
                                if (EuclideanDistance(new[] { (double)x1, (double)y1 }, new[] { (double)x2, (double)y2 }, 2) >= vecLenThreshold)
                                {
                                    int z = (x2 >= x1) ? ((y2 >= y1) ? 0 : 1) : ((y1 >= y2) ? 0 : 1);
                                    houghSpace[dy, dx, z]++;
                                    shiftVectors.Add((dx, dy, z, x1, y1, x2, y2));
                                }
                            }
                        }
                    }

                    // 4. Tìm giá trị lớn nhất trong không gian Hough
                    double max = 0;
                    for (int i = 0; i < height; i++)
                        for (int j = 0; j < width; j++)
                            for (int h = 0; h < 2; h++)
                                if (houghSpace[i, j, h] > max)
                                    max = houghSpace[i, j, h];

                    // 5. Đánh dấu các vùng copy-move
                    using (var result = src.Clone())
                    {
                        double threshold = max * (1 - numOfVectorThreshold);
                        foreach (var (dx, dy, z, x1, y1, x2, y2) in shiftVectors)
                        {
                            if (houghSpace[dy, dx, z] >= threshold)
                            {
                                Cv2.Rectangle(result, new OpenCvSharp.Point(x1, y1), 
                                            new OpenCvSharp.Point(x1 + blockSize, y1 + blockSize), 
                                            new Scalar(0, 255, 0), 2);
                                Cv2.Rectangle(result, new OpenCvSharp.Point(x2, y2), 
                                            new OpenCvSharp.Point(x2 + blockSize, y2 + blockSize), 
                                            new Scalar(0, 0, 255), 2);
                                Cv2.Line(result, 
                                        new OpenCvSharp.Point(x1 + blockSize/2, y1 + blockSize/2),
                                        new OpenCvSharp.Point(x2 + blockSize/2, y2 + blockSize/2),
                                        new Scalar(255, 255, 0), 1);
                            }
                        }

                        // Thêm thông tin về Accuracy và Coverage
                        string info = $"CMFD (DCT + Vector Quantization) - Accuracy: {CalculateAccuracy(similarPairs):F2}, Coverage: {CalculateCoverage(similarPairs, width, height):F2}";
                        Cv2.PutText(result, info, new OpenCvSharp.Point(10, 30), HersheyFonts.HersheyComplexSmall, 1.0, Scalar.White, 2);
                        DisplayImage(result, "Copy-Move Forgery Detection");
                    }
                }
            }
        }

        // Lấy numDCT hệ số DCT đầu tiên theo zigzag
        private double[] GetZigzagDCT(Mat dct, int numDCT)
        {
            var vector = new List<double>();
            int n = dct.Rows - 1;
            int i = 0;
            int j = 0;

            // Thêm phần tử đầu tiên
            vector.Add(dct.At<float>(i, j));

            // Quét ma trận theo zigzag
            for (int k = 0; k < n * 2; k++)
            {
                if (j == n)   // Biên phải
                {
                    i++;     // Dịch xuống
                    while (i != n)   // Đường chéo
                    {
                        vector.Add(dct.At<float>(i, j));
                        i++;
                        j--;
                    }
                }
                else if (i == 0)  // Biên trên
                {
                    j++;
                    while (j != 0)
                    {
                        vector.Add(dct.At<float>(i, j));
                        i++;
                        j--;
                    }
                }
                else if (i == n)   // Biên dưới
                {
                    j++;
                    while (j != n)
                    {
                        vector.Add(dct.At<float>(i, j));
                        i--;
                        j++;
                    }
                }
                else if (j == 0)   // Biên trái
                {
                    i++;
                    while (i != 0)
                    {
                        vector.Add(dct.At<float>(i, j));
                        i--;
                        j++;
                    }
                }

                vector.Add(dct.At<float>(i, j));
            }

            // Chỉ lấy numDCT phần tử đầu tiên
            return vector.Take(numDCT).ToArray();
        }

        // Tính khoảng cách Euclidean giữa 2 vector
        private double EuclideanDistance(double[] v1, double[] v2, int size)
        {
            double sum = 0;
            for (int i = 0; i < size; i++)
                sum += (v2[i] - v1[i]) * (v2[i] - v1[i]);
            return Math.Sqrt(sum);
        }

        // Tính Accuracy
        private double CalculateAccuracy(List<(OpenCvSharp.Point, OpenCvSharp.Point, double)> pairs)
        {
            if (pairs.Count == 0)
                return 0.0;

            int truePositives = pairs.Count(p => p.Item3 > 0.7); // Giả sử điểm > 0.7 là true positive
            return (double)truePositives / pairs.Count;
        }

        // Tính Coverage
        private double CalculateCoverage(List<(OpenCvSharp.Point, OpenCvSharp.Point, double)> pairs, int width, int height)
        {
            if (pairs.Count == 0)
                return 0.0;

            int totalBlocks = (width / 4) * (height / 4); // Tổng số khối có thể
            int detectedBlocks = pairs.SelectMany(p => new[] { p.Item1, p.Item2 }).Distinct().Count();
            return (double)detectedBlocks / totalBlocks;
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