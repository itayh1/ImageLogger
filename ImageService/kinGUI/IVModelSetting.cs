using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace kinGUI
{
    interface IVModelSetting : INotifyPropertyChanged
    {
        string OutputDir { get; }
        string SourceName { get; }
        string LogName { get; }
        int ThumbnailSize { get; }
        ObservableCollection<string> Handles { get; }
    }
}
