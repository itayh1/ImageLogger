using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kinGUI
{
    class VModelMainWin : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool connected;

        protected void NotifyPropertyChanged(string name)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public VModelMainWin()
        {
            Console.WriteLine("Ctor VModelMainWindow");
            ClientConn conn = ClientConn.Instance;
            VM_Connecetd = conn.Connected;
        }

        public bool VM_Connecetd
        {
            get { return this.connected; }
            set { this.connected = value;
                NotifyPropertyChanged("VM_Connecetd");
            }
        }
    }
}
