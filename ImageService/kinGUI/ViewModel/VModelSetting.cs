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
    class VModelSetting : INotifyPropertyChanged
    {
        ModelSetting model;

        public ICommand RemoveCommand { get; private set; }
        private string selectedItem;
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
            this.model.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                this.OnPropertyChanged(e.PropertyName);
            };
            this.RemoveCommand = new DelegateCommand<object>(this.Submit, this.CanSubmit);
            this.PropertyChanged += RemovePropertyChanged;

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
            get { return this.selectedItem; }
            set { this.selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
            //add property changed
        }

        private void RemovePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var command = this.RemoveCommand as DelegateCommand<object>;
            command.RaiseCanExecuteChanged();
        }

        public void Submit(object o)
        {
            this.model.Removehandler(this.SelectedItem);
        }

        public bool CanSubmit(object o)
        {
            Console.WriteLine("CanSubmit");
            if (string.IsNullOrEmpty(this.SelectedItem))
                return false;
            return true;
        }
    }
}
