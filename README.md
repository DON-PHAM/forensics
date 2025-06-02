# Image Forensics Tool

A powerful image forensics tool built with C# and OpenCV that helps detect image manipulation and forgery.

## Features

- **ELA (Error Level Analysis)**: Detects areas of an image that have been modified by analyzing compression artifacts
- **JPEG Ghost**: Identifies areas that have been saved multiple times as JPEG
- **Noise Analysis**: Analyzes noise patterns to detect inconsistencies
- **DCT Analysis**: Examines Discrete Cosine Transform coefficients to find manipulation
- **Zoom Test**: Detects resampling artifacts from image resizing
- **CFA Analysis**: Analyzes Color Filter Array patterns
- **ADJPEG Analysis**: Advanced JPEG artifact analysis
- **Clone Detection**: Detects copy-move forgery by identifying duplicated regions

## Requirements

- .NET 6.0 or later
- OpenCvSharp4
- Windows OS

## Installation

1. Clone the repository:
```bash
git clone https://github.com/yourusername/ImageForensics.git
```

2. Open the solution in Visual Studio 2022 or later

3. Build and run the project

## Usage

1. Launch the application
2. Click "Load Image" to select an image for analysis
3. Choose one of the analysis methods:
   - ELA Analysis
   - JPEG Ghost Analysis
   - Noise Analysis
   - DCT Analysis
   - Zoom Test
   - CFA Analysis
   - ADJPEG Analysis
   - Clone Detection

## Clone Detection Algorithm

The Clone Detection feature uses a sophisticated algorithm to detect copy-move forgery:

1. Divides the image into small blocks
2. Calculates hash for each block
3. Identifies blocks with matching hashes
4. Groups matching blocks by displacement vectors
5. Marks original regions (green) and cloned regions (red)
6. Draws connection lines between matching regions

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the LICENSE file for details. 