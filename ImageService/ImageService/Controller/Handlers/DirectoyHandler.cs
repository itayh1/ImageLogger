using ImageService.Modal;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.LoggingModal;
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
        private readonly string[] extentions = {".jpg", ".png", ".gif", ".bmp"};
        #endregion

        public event EventHandler<CommandRecievedEventArgs> CommandRecieved;

        public String DPath { get; set; }

        /*
         * Construct Directory handler 
         */
        public DirectoyHandler(IImageController ic, ILoggingService ls, string path) 
        {
            this.m_controller = ic;
            this.m_logging = ls;
            this.DPath = path;
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
         * The function updates the logger new file added and deals with file's extension
         */
        private void OnChanged(object sender, FileSystemEventArgs e)
         {
            this.m_logging.Log("New file added " + e.FullPath, MessageTypeEnum.INFO);
            string ext = Path.GetExtension(e.FullPath);
            if (this.extentions.Contains(ext)) {
               string[] parameters = { e.FullPath };
               CommandRecieved(this, new CommandRecievedEventArgs((int)CommandID.addFile, parameters, ""));
            }                                              
         }

        public void OnClose()
        {
            this.m_dirWatcher.EnableRaisingEvents = false;
            this.m_dirWatcher.Dispose();
        }





}
}
