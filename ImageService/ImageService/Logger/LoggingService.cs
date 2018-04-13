
using ImageService.Logging.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService
{
    public class LoggingService : ILoggingService
    {
        // get massage and operate eventHandler
        public event EventHandler<MessageRecievedEventArgs> MessageRecieved;
        
        /*
         * The function loggs recieved massage
         */ 
        public void Log(string message, MessageTypeEnum type)
        {
            MessageRecieved?.Invoke(this, new MessageRecievedEventArgs(type, message));
        }
    }
}
