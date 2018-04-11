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
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                    new ImageService(args)
            };
            ServiceBase.Run(ServicesToRun);
            //bool flag;
            //Modal.ImageServiceModal image = new Modal.ImageServiceModal(@"C:\Users\Itay\Pictures\target", 120);
            //image.AddFile(@"C:\Users\Itay\Pictures\sampl - Copy.png", out flag);

        }
    }
}
