using ImageService.Modal;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using ImageService.Infrastructure;
//using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Logging.Modal;
using System.Text.RegularExpressions;

namespace ImageService.Controller.Handlers
{
    enum CommandID { addFile = 0 }

    public class DirectoyHandler : IDirectoryHandler
    {
        #region Members
        private IImageController m_controller;              // The Image Processing Controller
        private ILoggingService m_logging;
        private FileSystemWatcher m_dirWatcher;             // The Watcher of the Dir
        private string m_path;                              // The Path of directory
        private readonly string[] extentions = {".jpg", ".png", ".gif", ".bmp"};
        #endregion

        // The Event That Notifies that the Directory is being closed
        public event EventHandler<DirectoryCloseEventArgs> DirectoryClose;             

        /*
         * Construct Directory handler 
         */
        public DirectoyHandler(IImageController ic, ILoggingService ls, string path) 
        {
            this.m_controller = ic;
            this.m_logging = ls;
            this.m_path = path;
            this.m_dirWatcher = new FileSystemWatcher(path);
        
        }                                                                 

        /*
         * The function getting  a directory path and start handeling the directory by watching its actions
         */
        public void StartHandleDirectory(string dirPath) 
        {
            this.m_logging.Log("initiate directory handler from:" + dirPath, MessageTypeEnum.INFO);
            this.m_dirWatcher.Created += new FileSystemEventHandler(this.OnChanged);
            this.m_dirWatcher.Changed += new FileSystemEventHandler(this.OnChanged);
            this.m_dirWatcher.EnableRaisingEvents = true;
            this.m_logging.Log("begin watching " + dirPath, MessageTypeEnum.INFO);
        }


        /*
         * The Event that will be activated upon new Command
         */
        public void OnCommandRecieved(object sender, CommandRecievedEventArgs e) {
		    bool res;
            string msg = this.m_controller.ExecuteCommand(e.CommandID, e.Args, out res);
            if (res) {
                this.m_logging.Log(msg, MessageTypeEnum.INFO);
            }
            else {
                this.m_logging.Log(msg, MessageTypeEnum.FAIL);
            }
        }


        /*
         * The function updates the logger new file added and deals with file's extension
         */
        private void OnChanged(object sender, FileSystemEventArgs e)
         {
            this.m_logging.Log("New file added " + e.FullPath, MessageTypeEnum.INFO);
            string ext = Path.GetExtension(e.FullPath);
            if (this.extentions.Contains(ext)) {
               string[] parameters = { e.FullPath };
               this.OnCommandRecieved(this, new CommandRecievedEventArgs((int)CommandID.addFile, parameters, ""));
            }                                              
         }


        /*
         * The function updates the logger of close action and stops watcher
         */
        public void OnClosed(object sender, DirectoryCloseEventArgs e) {
            
            try {
                this.m_dirWatcher.EnableRaisingEvents = false;
                ((Server.ImageServer) sender).CommandRecieved -= this.OnCommandRecieved;
                this.m_logging.Log("handler closed" + this.m_path, MessageTypeEnum.INFO);
            }
            catch (Exception ex) {
                this.m_logging.Log(String.Format("cannot close handler of dir {0}, msg: {1}",
                    this.m_path, ex.Message.ToString()), MessageTypeEnum.FAIL);
            }

        }
    }
}
