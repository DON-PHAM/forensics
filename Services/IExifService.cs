using System.Collections.Generic;
using System.Threading.Tasks;
using ImageForensics.Models;

namespace ImageForensics.Services
{
    public interface IExifService
    {
        Task<List<ExifGroup>> GetExifInfoAsync(string imagePath);
    }
} 