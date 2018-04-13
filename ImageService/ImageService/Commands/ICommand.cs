using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Commands
{
    public interface ICommand
    {
        /*
         * A generic function which executes commands  
         */
        string Execute(string[] args, out bool result);
    }
}
