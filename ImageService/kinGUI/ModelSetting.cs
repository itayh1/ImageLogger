using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Web.Script.Serialization;

namespace kinGUI
{
    class ModelSetting : INotifyPropertyChanged
    {
        private VClient client;
        private string outputDir;
        private string logName;
        private string sourceName;
        private int thumbnailSize;
        private ObservableCollection<string> handlers;

        public event PropertyChangedEventHandler PropertyChanged;
        public ModelSetting()
        {
            Console.WriteLine("Ctor Model");
            //this.client = new VClient("127.0.0.1", 8888);

            this.outputDir = "asd";
            this.sourceName = "asfasgfs";
            this.thumbnailSize = 324235;
            this.handlers = new ObservableCollection<string>();
            this.handlers.Add("hello");
            this.handlers.Add("hellosdas");

        }
        public void NotifyPropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public void SetSettings()
        {
            string message;
            CommandRecievedEventArgs command = new CommandRecievedEventArgs(
                (int)CommandEnum.GetConfigCommand, null, string.Empty);
            var serializer = new JavaScriptSerializer();
            var serializedData = serializer.Serialize(command);
            //send request for appconfig
            this.client.sendMessage(serializedData.ToString());
            // get appconfig
            message = this.client.getMessage();
            command = serializer.Deserialize<CommandRecievedEventArgs>(message);
            ConfigurationData cd = serializer.Deserialize
                <ConfigurationData>(command.Args[0]);
            this.outputDir = cd.outputDir;
            this.logName = cd.sourceName;
            this.sourceName = cd.sourceName;
            this.thumbnailSize = cd.thumbnailSize;
            this.handlers = new ObservableCollection<string>(cd.handlers);
        }

        public void Removehandler(string handler)
        {
            string message;
            CommandRecievedEventArgs command = new CommandRecievedEventArgs(
                (int)CommandEnum.CloseCommand, new string[] { handler }, string.Empty);
            var serializer = new JavaScriptSerializer();
            var serializedData = serializer.Serialize(command);
            //send request for appconfig
            this.client.sendMessage(serializedData.ToString());
            
            //message = this.client.getMessage();
            //command = serializer.Deserialize<CommandRecievedEventArgs>(message);
            //ConfigurationData cd = serializer.Deserialize
            //    <ConfigurationData>(command.Args[0]);
            //this.client.sendMessage
        }

        public string OutputDir
        {
            get { return this.outputDir; }
            set { this.outputDir = value;
                NotifyPropertyChanged("OutputDir");  }
        }
        public string SourceName {
            get { return this.sourceName; }
            set { this.sourceName = value;
                NotifyPropertyChanged("SourceName"); }
        }
        public string LogName {
            get { return this.logName; }
            set { this.logName = value;
                NotifyPropertyChanged("LogName"); }
            }
        public int ThumbnailSize {
            get { return this.thumbnailSize; }
            set { this.thumbnailSize = value;
                NotifyPropertyChanged("ThumbnailSize"); }
            }
        public ObservableCollection<String> Handlers {
            get { return this.handlers; }
            set { this.handlers = value;
                NotifyPropertyChanged("Handlers");  }
            }
    }
}
