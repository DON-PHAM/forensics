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
using System.Drawing;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Jpeg;
using System.Windows.Documents;
using SD = System.Drawing;
using WM = System.Windows.Media;

namespace ImageForensics
{
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }
} 