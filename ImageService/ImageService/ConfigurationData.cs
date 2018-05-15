using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService
{
    public class ConfigurationData
    {
        public String SourceName { get; set; }
        public String OutputDir { get; set; }
        public String LogName { get; set; }
        public int ThumbnailSize { get; set; }
        public List<String> Handlers { get; set; }
    }
}
