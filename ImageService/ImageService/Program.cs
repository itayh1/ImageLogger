using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ImageService
{
    static class Program
    {
        // The main entry point for the application.
        static void Main(string[] args)
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] {
                    new ImageService(args)
            };

            ServiceBase.Run(ServicesToRun);
            //Modal.ImageServiceModal m = new Modal.ImageServiceModal(@"C:\Users\Itay\Pictures\target", 120);
            //bool res;
            //string str = m.AddFile(@"C:\Users\Itay\Pictures\sampl.png", out res);
        }
    }
}
