using ImageService.Controller;
using ImageService.Controller.Handlers;
using ImageService.Logging;
using ImageService.Modal;
using System;
using System.Configuration;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Server
{
    public class ImageServer
    {
        #region Members
        private IImageController m_controller;
        private ILoggingService m_logging;
        #endregion
        
        #region Properties
        public event EventHandler<CommandRecievedEventArgs> CommandRecieved;          // The event that notifies about a new Command being recieved
        public event EventHandler<DirectoryCloseEventArgs> closeCommand;

        #endregion
        public ImageServer(IImageController iic, ILoggingService ils)
        {
            this.m_controller = iic;
            this.m_logging = ils;

            string[] dirs = ConfigurationManager.AppSettings["Handler"].Split(';');

            foreach (string s in dirs) {
                try 
                {
                    if (Directory.Exists(s))
                        this.InitHandler(s);
                } catch(Exception e) {
                    this.m_logging.Log("Failed initiating handler, " + s + ", " +e.Message.ToString(), Logging.Modal.MessageTypeEnum.FAIL);
                }
            }
        }

        public void InitHandler(string path) 
        {
            IDirectoryHandler dir = new DirectoyHandler(this.m_controller, this.m_logging, path);
            this.CommandRecieved += dir.OnCommandRecieved;
            this.closeCommand += dir.OnClosed;
            dir.StartHandleDirectory(path);
            this.m_logging.Log("An handler has been initialized, "+path, Logging.Modal.MessageTypeEnum.INFO );
        }
        public void ServerClose() {
            try {
                this.closeCommand?.Invoke(this, null);
                this.m_logging.Log("Server closed", Logging.Modal.MessageTypeEnum.INFO);
            }
            catch(Exception e){
                this.m_logging.Log("Failed closing the server " + e.Message.ToString(), Logging.Modal.MessageTypeEnum.FAIL);
            }
        }   
    }
}
