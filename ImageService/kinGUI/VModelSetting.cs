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

        public string SourceName => throw new NotImplementedException();

        public string LogName => throw new NotImplementedException();

        public int ThumbnailSize => throw new NotImplementedException();

        public ObservableCollection<string> Handles => throw new NotImplementedException();
    }
}
