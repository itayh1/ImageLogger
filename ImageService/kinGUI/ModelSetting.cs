using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;

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
            this.client = new VClient("127.0.0.1", 8888);
            this.outputDir = "asd";
        }
        public void NotifyPropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public void SetSettings()
        {

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
        public ObservableCollection<string> Handlers {
            get { return this.handlers; }
            set { this.handlers = value;
                NotifyPropertyChanged("Handlers");  }
            }
    }
}
