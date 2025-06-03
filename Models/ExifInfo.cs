using System.Collections.Generic;

namespace ImageForensics.Models
{
    public class ExifInfo
    {
        public string Category { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class ExifGroup
    {
        public string GroupName { get; set; }
        public List<ExifInfo> Properties { get; set; } = new List<ExifInfo>();
    }
} 