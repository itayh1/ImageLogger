using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Web.Script.Serialization;
using Microsoft.Practices.Prism.Commands;

namespace kinGUI
{
    class ModelSetting : INotifyPropertyChanged
    {
        private ClientConn client;
        private string outputDir;
        private string logName;
        private string sourceName;
        private int thumbnailSize;
        private ObservableCollection<string> handlers;
        private string selectedPath;
        public event PropertyChangedEventHandler PropertyChanged;

        public ModelSetting()
        {
            Console.WriteLine("Ctor Model");
            this.client = ClientConn.Instance;
            this.client.OnCommandRecieved += this.OnCommandRecieved;
        }
        public void OnPropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public void SetSettings(CommandRecievedEventArgs e)
        {
            var serializer = new JavaScriptSerializer();
            ConfigurationData cd = serializer.Deserialize
                <ConfigurationData>(e.Args[0]);
            this.outputDir = cd.outputDir;
            this.logName = cd.logName;
            this.sourceName = cd.sourceName;
            this.thumbnailSize = cd.thumbnailSize;
            this.handlers = new ObservableCollection<string>(cd.handlers);
        }

        public void Removehandler(string handler)
        {
            CommandRecievedEventArgs command = new CommandRecievedEventArgs(
                (int)CommandEnum.CloseCommand, new string[] { handler }, string.Empty);
            var serializer = new JavaScriptSerializer();
            var serializedData = serializer.Serialize(command);
            this.client.sendMessage(serializedData);
        }

        public void OnCommandRecieved(object sender, CommandRecievedEventArgs e)
        {
            if (e.CommandID == (int)CommandEnum.GetConfigCommand)
            {
                this.SetSettings(e);
            }
            else if (e.CommandID == (int)CommandEnum.CloseCommand)
            {
                this.handlers.Remove(e.Args[0]);
            }
        }

        public string OutputDir
        {
            get { return this.outputDir; }
            set { this.outputDir = value;
                OnPropertyChanged("OutputDir");  }
        }
        public string SourceName {
            get { return this.sourceName; }
            set { this.sourceName = value;
                OnPropertyChanged("SourceName"); }
        }
        public string LogName {
            get { return this.logName; }
            set { this.logName = value;
                OnPropertyChanged("LogName"); }
            }
        public int ThumbnailSize {
            get { return this.thumbnailSize; }
            set { this.thumbnailSize = value;
                OnPropertyChanged("ThumbnailSize"); }
            }
        public ObservableCollection<String> Handlers {
            get { return this.handlers; }
            set { this.handlers = value;
                OnPropertyChanged("Handlers");  }
            }
        public string SelectedPath
        {
            get { return this.selectedPath; }
            set { this.selectedPath = value;
                OnPropertyChanged("SelectedPath");
            }
        }
    }
}
