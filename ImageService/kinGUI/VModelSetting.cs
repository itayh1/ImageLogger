using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;

namespace kinGUI
{
    class VModelSetting : IVModelSetting
    {
        ModelSetting model;

        private ICommand RemoveCommand { get; set; }

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

            this.RemoveCommand = new DelegateCommand(this.Submit, this.CanSubmit);

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

        public ObservableCollection<String> Handlers
        {
            get { return this.model.Handlers; }
        }

        public string SelectedItem
        {
            get; set;
        }

        public void Submit()
        {
            this.model.Removehandler(this.SelectedItem);
        }

        public bool CanSubmit()
        {
            if (this.SelectedItem != null)
                return true;
            return false;
        }
    }
}
