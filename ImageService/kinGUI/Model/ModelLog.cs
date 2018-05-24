using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace kinGUI
{
    class ModelLog : INotifyPropertyChanged
    {
        private ClientConn client;
        private List<LogObject> logs;
        public event PropertyChangedEventHandler PropertyChanged;

        public ModelLog()
        {
            this.client = ClientConn.Instance;
            this.client.OnCommandRecieved += this.OnCommandRecieved;
            
            this.logs = new List<LogObject>();
        }

        public void NotifyPropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public void OnCommandRecieved(object sender, CommandRecievedEventArgs e)
        {
            if (e.CommandID == (int)CommandEnum.GetListLogCommand)
            {
                App.Current.Dispatcher.Invoke(new Action(() =>
                {
                    List<LogObject> temp = JsonConvert.DeserializeObject<List<LogObject>>(e.Args[0]);
                    this.Logs.AddRange(temp);
                }));
            }
            else if (e.CommandID == (int)CommandEnum.LogCommand)
            {
                App.Current.Dispatcher.Invoke(new Action(() =>
                {
                    LogObject newLog = JsonConvert.DeserializeObject<LogObject>(e.Args[0]);
                    this.Logs.Add(newLog);
                }));
                
            }
        }

        public List<LogObject> Logs
        {
            get { return this.logs; }
            set { this.logs = value;
                NotifyPropertyChanged("LogsList");
            }
        }
    }
}
