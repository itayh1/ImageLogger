using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.LoggingModal
{
    public class LogObject
    {

        public LogObject(string type, string msg)
        {
            this.Type = type;
            this.Message = msg;
        }

        public string Type { get; set; }
        public string Message { get; set; }
    }
}
