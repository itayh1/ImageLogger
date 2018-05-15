using ImageService.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Controller.Handlers
{
    public interface IDirectoryHandler
    {
        event EventHandler<CommandRecievedEventArgs> CommandRecieved;

        String DPath { get; set; }

        void StartHandleDirectory(string dirPath);             // The Function Recieves the directory to Handle
        void OnClose();
    }
}
