using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kinGUI
{
    public class Controler
    {
        public event EventHandler<CommandRecievedEventArgs> commandHandler;

        //public void OnCommandRecieved(object sender, CommandRecievedEventArgs e)
        //{
        //    bool res;
        //    string msg = this.m_controller.ExecuteCommand(e.CommandID, e.Args, out res);
        //    if (res)
        //    {
        //        this.m_logging.Log(msg, MessageTypeEnum.INFO);
        //    }
        //    else
        //    {
        //        this.m_logging.Log(msg, MessageTypeEnum.FAIL);
        //    }
        //}

        //public string ExecuteCommand(int commandID, string[] args, out bool resultSuccesful)
        //{
        //    if (this.commands.ContainsKey(commandID))
        //    {
        //        return this.commands[commandID].Execute(args, out resultSuccesful);
        //    }
        //    resultSuccesful = false;
        //    string message = "Invalid command";
        //    return message;
        //}
    }
}
