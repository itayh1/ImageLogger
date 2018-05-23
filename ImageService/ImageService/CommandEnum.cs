using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService
{
    public enum CommandEnum : int
    {
        NewFileCommand, // =0
        GetConfigCommand,
        LogCommand,
        CloseCommand,
        GetListLogCommand,
        ExitCommand
    }
}
