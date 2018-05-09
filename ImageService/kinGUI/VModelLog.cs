﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace kinGUI
{
    class VModelLog : INotifyPropertyChanged
    {
        private ModelLog model;
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public VModelLog()
        {
            this.model = new ModelLog();
        }

        public List<LogObject> LogsList
        {
            get { return this.model.Logs; }
            set { this.model.Logs = value; }
        }


    }
}
