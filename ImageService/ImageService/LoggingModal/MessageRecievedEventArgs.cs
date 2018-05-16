﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.LoggingModal
{
    public class MessageRecievedEventArgs : EventArgs
    {
        public MessageTypeEnum Status { get; set; }
        public string Message { get; set; }

        /*
         * Construct MessageRecievedEventArgs by massage and status
         */
        public MessageRecievedEventArgs(MessageTypeEnum status, string message)
        {
            this.Status = status;
            this.Message = message;
        }
    }
}
