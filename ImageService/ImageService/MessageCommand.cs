using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService
{
    public class MessageCommand
    {
        public int cmd {get;}
        public string[] args { get;}

        public MessageCommand(int cmd, string[] args)
        {
            this.cmd = cmd;
            this.args = args;
        }
    } 
}
