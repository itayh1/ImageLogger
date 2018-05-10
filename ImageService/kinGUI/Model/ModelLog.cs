using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Web.Script.Serialization;

namespace kinGUI
{
    class ModelLog : INotifyPropertyChanged
    {
        private ClientConn client;
        private List<LogObject> logs;
        public event PropertyChangedEventHandler PropertyChanged;

        public ModelLog()
        {
            // this.client = VClient.Instance;
            //this.client.OnCommandRecieved += this.OnCommandRecieved;
            this.logs = new List<LogObject>();
            logs.Add(new LogObject() { Type = "Info", Message = "Hello world!" });
            logs.Add(new LogObject() { Type = "fail", Message = "Hello world2!" });
            logs.Add(new LogObject() { Type = "warning", Message = "Hello world2!" });
        }

        public void NotifyPropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public void OnCommandRecieved(object sender, CommandRecievedEventArgs e)
        {
            if (e.CommandID == (int)CommandEnum.GetListLogCommand)
            {
                var serializer = new JavaScriptSerializer();
                List<LogObject> temp = serializer.Deserialize<List<LogObject>>(e.Args[0]);
                this.logs.AddRange(temp);
            }
            else if (e.CommandID == (int)CommandEnum.LogCommand)
            {
                this.logs.Add(new LogObject() { Type = e.Args[0], Message = e.Args[1] });
            }
        }

        public List<LogObject> Logs
        {
            get { return this.logs; }
            set { this.logs = value; }
        }
    }
}
