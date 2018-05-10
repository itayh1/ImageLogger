using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace kinGUI
{
    class ModelLog : INotifyPropertyChanged
    {
        private VClient client;
        private List<LogObject> logs;
        public event PropertyChangedEventHandler PropertyChanged;

        public ModelLog()
        {
           // this.client = VClient.Instance;
            this.logs = new List<LogObject>();
            logs.Add(new LogObject() { Type = "Info", Message = "Hello world!" });
            logs.Add(new LogObject() { Type = "Error", Message = "Hello world2!" });
        }

        public void NotifyPropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public List<LogObject> Logs
        {
            get { return this.logs; }
            set { this.logs = value; }
        }
    }
}
