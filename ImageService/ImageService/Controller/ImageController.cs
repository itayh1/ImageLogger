using ImageService.Commands;
using ImageService.Infrastructure;
using ImageService.Infrastructure.Enums;
using ImageService.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Controller
{
    public class ImageController : IImageController
    {
        private IImageServiceModal m_modal;                      // The Modal Object
        private Dictionary<int, ICommand> commands;

        public ImageController(IImageServiceModal modal)
        {
            m_modal = modal;                    // Storing the Modal Of The System
            commands = new Dictionary<int, ICommand>();
            commands[0] = new Commands.NewFileCommand(this.m_modal);
        }
        public string ExecuteCommand(int commandID, string[] args, out bool resultSuccesful)
        {
            if (this.commands.ContainsKey(commandID))
            {
                return this.commands[commandID].Execute(args, out resultSuccesful);
            }
            resultSuccesful = false;
            string message = "Invalid command";
            return message;
        }
    }
}
