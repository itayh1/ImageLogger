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
        private ModelSetting model;

        public ICommand RemoveCommand { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string name)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public VModelSetting()
        {
            Console.WriteLine("Ctor VModelSetting");
            this.model = new ModelSetting();
            this.model.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                this.NotifyPropertyChanged("VM_" + e.PropertyName);
            };

            this.RemoveCommand = new DelegateCommand<object>(this.Submit, this.CanSubmit);
        }

        public string VM_OutputDir
        {
            get { return this.model.OutputDir; }
            set { this.model.OutputDir = value; }
        }

        public string VM_SourceName
        {
            get { return this.model.SourceName; }
            set { this.model.SourceName = value; }
        }

        public string VM_LogName
        {
            get { return this.model.LogName; }
            set { this.model.LogName = value; }
        }

        public int VM_ThumbnailSize
        {
            get { return this.model.ThumbnailSize; }
            set { this.model.ThumbnailSize = value; }
        }

        public ObservableCollection<String> VM_Handlers
        {
            get { return this.model.Handlers; }
        }

        public string VM_SelectedPath
        {
            get { return this.model.SelectedPath; }
            set { this.model.SelectedPath = value;
                var command = this.RemoveCommand as DelegateCommand<object>;
                command.RaiseCanExecuteChanged();
            }
        }

        public void Submit(object o)
        {
            Console.WriteLine("Submit");
            this.model.Removehandler(this.VM_SelectedPath);
        }

        public bool CanSubmit(object o)
        {
            Console.WriteLine("CanSubmit");
            if (string.IsNullOrEmpty(this.VM_SelectedPath))
                return false;
            return true;
        }
    }
}
