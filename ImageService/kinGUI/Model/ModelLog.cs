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
        private ObservableCollection<LogObject> logs;
        public event PropertyChangedEventHandler PropertyChanged;

        public ModelLog()
        {
            this.client = ClientConn.Instance;
            this.client.OnCommandRecieved += this.OnCommandRecieved;
            
            this.logs = new ObservableCollection<LogObject>();
        }

        public void NotifyPropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public void OnCommandRecieved(object sender, CommandRecievedEventArgs e)
        {
            Console.WriteLine(e.CommandID + e.Args[0]);
            if (e.CommandID == (int)CommandEnum.GetListLogCommand)
            {
                App.Current.Dispatcher.Invoke(new Action(() =>
                {
                    List<LogObject> temp = JsonConvert.DeserializeObject<List<LogObject>>(e.Args[0]);
                    foreach (LogObject lo in temp)
                    {
                        this.LogsList.Add(lo);
                    }                    
                }));
            }
            else if (e.CommandID == (int)CommandEnum.LogCommand)
            {
                App.Current.Dispatcher.Invoke(new Action(() =>
                {
                    LogObject newLog = JsonConvert.DeserializeObject<LogObject>(e.Args[0]);
                    this.LogsList.Add(newLog);
                }));

            }
        }

        public ObservableCollection<LogObject> LogsList
        {
            get { return this.logs; }
            set { this.logs = value;
                NotifyPropertyChanged("LogsList");
            }
        }
    }
}
