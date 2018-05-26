using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Newtonsoft.Json;
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
            ConfigurationData cd = JsonConvert.DeserializeObject<ConfigurationData>(e.Args[0]);
            this.OutputDir = cd.outputDir;
            this.LogName = cd.logName;
            this.SourceName = cd.sourceName;
            this.ThumbnailSize = cd.thumbnailSize;
            this.Handlers = new ObservableCollection<string>(cd.handlers);
        }

        public void Removehandler(string handler)
        {
            try
            {
                // remove from handlers
                //if (this.handlers.Count > 0 && this.handlers.Contains(handler) && handler != null)
                //{
                //    this.handlers.Remove(handler);
                //}

                // update server handler was removed
                CommandRecievedEventArgs command = new CommandRecievedEventArgs(
                    (int)CommandEnum.CloseCommand, new string[] { handler }, string.Empty);
                var serializedData = JsonConvert.SerializeObject(command);
                this.client.sendMessage(serializedData);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ", cannot remove this handler");
            }
        }

        public void OnCommandRecieved(object sender, CommandRecievedEventArgs e)
        {
            if (e.CommandID == (int)CommandEnum.GetConfigCommand)
            {
                this.SetSettings(e);
            }
            else if (e.CommandID == (int)CommandEnum.CloseCommand)
            {
                //this.Removehandler(e.Args[0]);
                try
                {
                    string handler = e.Args[0];
                    if (this.handlers.Contains(handler))
                    {
                        App.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            this.Handlers.Remove(handler);
                        }));
                    }
                } catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
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
