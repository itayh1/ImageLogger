using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace kinGUI
{
    class VModelSetting : IVModelSetting
    {
        ModelSetting model;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public VModelSetting()
        {
            Console.WriteLine("Ctor VModelSetting");
            this.model = new ModelSetting();

        }
        public string OutputDir
        {
            get { return this.model.OutputDir; }
            set { this.model.OutputDir = value; }
        }

        public string SourceName
        {
            get { return this.model.SourceName; }
            set { this.model.SourceName = value; }
        }

        public string LogName
        {
            get { return this.model.LogName; }
            set { this.model.LogName = value; }
        }

        public int ThumbnailSize
        {
            get { return this.model.ThumbnailSize; }
            set { this.model.ThumbnailSize = value; }
        }

        public ObservableCollection<string> Handlers
        {
            get { return this.model.Handlers; }
        }
    }
}
