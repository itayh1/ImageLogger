using ImageService.Commands;
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
        private IImageServiceModal m_modal;
        private Dictionary<int, ICommand> commands;

        /*
         * Construct ImageController
         */  
        public ImageController(IImageServiceModal modal)
        {
            m_modal = modal;
            commands = new Dictionary<int, ICommand>();
            commands = new Dictionary<int, ICommand>()
            {
                { (int) CommandEnum.NewFileCommand, new NewFileCommand(m_modal) }
            };

        }

        /*
         * The function execute a command by getting its id and args 
         */ 
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
