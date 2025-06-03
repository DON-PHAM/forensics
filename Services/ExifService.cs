using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageForensics.Models;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Jpeg;

namespace ImageForensics.Services
{
    public class ExifService : IExifService
    {
        public async Task<List<ExifGroup>> GetExifInfoAsync(string imagePath)
        {
            return await Task.Run(() =>
            {
                var exifGroups = new List<ExifGroup>();
                var exifInfo = new Dictionary<string, List<ExifInfo>>();

                try
                {
                    var directories = ImageMetadataReader.ReadMetadata(imagePath);

                    foreach (var directory in directories)
                    {
                        var groupName = directory.Name;
                        if (!exifInfo.ContainsKey(groupName))
                        {
                            exifInfo[groupName] = new List<ExifInfo>();
                        }

                        foreach (var tag in directory.Tags)
                        {
                            exifInfo[groupName].Add(new ExifInfo
                            {
                                Category = groupName,
                                Name = tag.Name,
                                Value = tag.Description
                            });
                        }
                    }

                    // Thêm thông tin cơ bản về file
                    var fileInfo = new FileInfo(imagePath);
                    if (!exifInfo.ContainsKey("File Information"))
                    {
                        exifInfo["File Information"] = new List<ExifInfo>();
                    }
                    exifInfo["File Information"].AddRange(new[]
                    {
                        new ExifInfo { Category = "File Information", Name = "Name", Value = fileInfo.Name },
                        new ExifInfo { Category = "File Information", Name = "Size", Value = $"{fileInfo.Length / 1024.0:F2} KB" },
                        new ExifInfo { Category = "File Information", Name = "Created", Value = fileInfo.CreationTime.ToString() },
                        new ExifInfo { Category = "File Information", Name = "Modified", Value = fileInfo.LastWriteTime.ToString() }
                    });

                    // Thêm thông tin ảnh
                    using (var image = Image.FromFile(imagePath))
                    {
                        if (!exifInfo.ContainsKey("Image Information"))
                        {
                            exifInfo["Image Information"] = new List<ExifInfo>();
                        }
                        exifInfo["Image Information"].AddRange(new[]
                        {
                            new ExifInfo { Category = "Image Information", Name = "Width", Value = $"{image.Width} pixels" },
                            new ExifInfo { Category = "Image Information", Name = "Height", Value = $"{image.Height} pixels" },
                            new ExifInfo { Category = "Image Information", Name = "Format", Value = image.RawFormat.ToString() },
                            new ExifInfo { Category = "Image Information", Name = "Resolution", Value = $"{image.HorizontalResolution} x {image.VerticalResolution} DPI" }
                        });
                    }

                    // Chuyển đổi thành ExifGroup
                    foreach (var group in exifInfo)
                    {
                        exifGroups.Add(new ExifGroup
                        {
                            GroupName = group.Key,
                            Properties = group.Value.OrderBy(x => x.Name).ToList()
                        });
                    }
                }
                catch (Exception ex)
                {
                    exifGroups.Add(new ExifGroup
                    {
                        GroupName = "Error",
                        Properties = new List<ExifInfo>
                        {
                            new ExifInfo { Category = "Error", Name = "Error", Value = $"Không thể đọc EXIF: {ex.Message}" }
                        }
                    });
                }

                return exifGroups.OrderBy(x => x.GroupName).ToList();
            });
        }
    }
} 