using ImageService.Controller;
using ImageService.Controller.Handlers;
using ImageService.LoggingModal;
using ImageService.Modal;
using System;
using System.Configuration;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ImageService.Server
{
    public class ImageServer
    {
        #region Members
        private IImageController m_controller;
        private ILoggingService m_logging;
        private List<IDirectoryHandler> handlers;
        private Communicator communicator;
        #endregion

        /*
         * Construct ImageServer by imageController and loggingService
         */
        public ImageServer(IImageController iic, ILoggingService ils, Communicator com)
        {
            this.m_controller = iic;
            this.m_logging = ils;
            this.handlers = new List<IDirectoryHandler>();
            this.communicator = com;
            this.communicator.OnCommandRecieved += this.OnCommandRecieved;
            this.setHandlers(com.Configurations.Handlers.ToArray<string>());
        }

        public void setHandlers(string[] dirs)
        {
            foreach (string handler in dirs)
            {
                try
                {
                    if (Directory.Exists(handler))
                    {
                        IDirectoryHandler dir = new DirectoyHandler(this.m_controller, this.m_logging, handler);
                        dir.CommandRecieved += OnCommandRecieved;
                        
                        dir.StartHandleDirectory(handler);
                        this.handlers.Add(dir);
                        this.m_logging.Log("An handler has been initialized, " + handler, MessageTypeEnum.INFO);
                    }
                }
                catch (Exception e)
                {
                    this.m_logging.Log("Failed initiating handler, " + handler + ", " + e.Message.ToString(),MessageTypeEnum.FAIL);
                }
            }
        }

        /*
         * The Event that will be activated upon new Command
         */
        public void OnCommandRecieved(object sender, CommandRecievedEventArgs e)
        {
            bool res;
            // remove ommand handler
            if (e.CommandID == (int)CommandEnum.CloseCommand)
            {
                this.OnClosed(e.Args[0]);

            }
            // addFile command
            else
            {
                string msg = this.m_controller.ExecuteCommand(e.CommandID, e.Args, out res);
                MessageTypeEnum typeEnum;
                if (res)
                {
                    typeEnum = MessageTypeEnum.INFO;
                    this.m_logging.Log(msg, typeEnum);
                }
                else
                {
                    typeEnum = MessageTypeEnum.FAIL;
                    this.m_logging.Log(msg, typeEnum);
                }
                this.BuildLogAndSendCommand(msg, typeEnum.ToString());
            }
        }

        /*
        * The function updates the logger of close action and stops watcher
        */
        public void OnClosed(string path)
        {
            string message = String.Empty;
            try
            {
                foreach (IDirectoryHandler handler in handlers)
                {
                    // finds handler from list and remove it
                    if (handler.DPath.CompareTo(path) == 0)
                    {
                        // log creation and update all clients
                        message += "handler: " + path + " was closed";
                        string type = MessageTypeEnum.INFO.ToString();
                        BuildLogAndSendCommand(message, type);
                        this.m_logging.Log(message, MessageTypeEnum.INFO);

                        // remove handler
                        handler.OnClose();
                        this.handlers.Remove(handler);
                        this.communicator.Configurations.Handlers.Remove(path);
                        handler.CommandRecieved -= this.OnCommandRecieved;
                        break;                      
                    }
                }
                // update all clients of which handler closed
                communicator.SendCommandBroadCast(new CommandRecievedEventArgs((int)CommandEnum.CloseCommand, 
                    new string[] { path }, String.Empty));
            }
            catch (Exception ex)
            {
                this.m_logging.Log(String.Format("cannot close handler of dir {0}, msg: {1}",
                    path, ex.Message.ToString()), MessageTypeEnum.FAIL);
            }
        }


        private void BuildLogAndSendCommand(string message, string type)
        {
            LogObject newLog = new LogObject(type, message);
            var serializedLog = JsonConvert.SerializeObject(newLog);
            CommandRecievedEventArgs cmd = new CommandRecievedEventArgs((int)CommandEnum.LogCommand, new string[] { serializedLog }, String.Empty);
            this.communicator.SendCommandBroadCast(cmd);
        }

        /*
         * The function close the server
         */
        public void ServerClose() {
            try
            {
                foreach (IDirectoryHandler handler in handlers)
                {
                    handler.OnClose();
                }
                this.communicator.Close();
                this.m_logging.Log("Server closed", MessageTypeEnum.INFO);
            }
            catch (Exception e)
            {
                this.m_logging.Log("Failed closing the server " + e.Message.ToString(), MessageTypeEnum.FAIL);
            }
        }   
    }
}
