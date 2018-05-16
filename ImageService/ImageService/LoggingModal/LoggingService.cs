﻿
using ImageService.LoggingModal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.LoggingModal
{
    public class LoggingService : ILoggingService
    {

        public List<LogObject> logs;
        // get massage and operate eventHandler
        public event EventHandler<MessageRecievedEventArgs> MessageRecieved;
        
        public LoggingService()
        {
            this.logs = new List<LogObject>();              
        }
   
        /*
         * The function loggs recieved massage
         */
        public void Log(string message, MessageTypeEnum type)
        {
            LogObject newLog = new LogObject(type.ToString(), message);
            Logs.Add(newLog);
            MessageRecieved?.Invoke(this, new MessageRecievedEventArgs(type, message));
        }

        public List<LogObject> Logs
        {
            get { return this.logs; }
            set { this.logs = value; }
        }
    }
}
