using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Runtime.InteropServices;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace ImageService
{    
    public partial class ImageService : ServiceBase
    {
        public enum ServiceState
        {
            SERVICE_STOPPED = 0x00000001,
            SERVICE_START_PENDING = 0x00000002,
            SERVICE_STOP_PENDING = 0x00000003,
            SERVICE_RUNNING = 0x00000004,
            SERVICE_CONTINUE_PENDING = 0x00000005,
            SERVICE_PAUSE_PENDING = 0x00000006,
            SERVICE_PAUSED = 0x00000007,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ServiceStatus
        {
            public int dwServiceType;
            public ServiceState dwCurrentState;
            public int dwControlsAccepted;
            public int dwWin32ExitCode;
            public int dwServiceSpecificExitCode;
            public int dwCheckPoint;
            public int dwWaitHint;
        };

        // code from the service guide.
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);
        // class members
        private int eventId = 1;
        private Server.ImageServer m_imageServer; 
        private Modal.IImageServiceModal modal;
        private Controller.IImageController controller;
        private ILoggingService logging;
        // tcp connection
        private int server_port = 5555;
        private TcpListener listener;


        
        /*
         * Construct ImageServic by app configurations, modal, controller and logger
         */
        public ImageService(string[] args) {

            InitializeComponent();

            // updates data from configurations manager
            string targetPath = ConfigurationManager.AppSettings["OutputDir"];  // destenation dir
            targetPath = targetPath.Replace(";", "");
            string eventSourceName = ConfigurationManager.AppSettings["SourceName"];   //  "MySource";
            string logName = ConfigurationManager.AppSettings["LogName"];    //  "MyNewLog";
            int thumbnail = int.Parse(ConfigurationManager.AppSettings["ThumbnailSize"]);

            // init log and src
            if (args.Count() > 0)
            {
                eventSourceName = args[0];
            }
            if (args.Count() > 1)
            {
                logName = args[1];
            }

            // create new eventLog by src
            eventLog1 = new EventLog();
            if (!EventLog.SourceExists(eventSourceName))
            {
                EventLog.CreateEventSource(eventSourceName, logName);
            }

            eventLog1.Source = eventSourceName;
            eventLog1.Log = logName;

            // construct all members
            this.modal = new Modal.ImageServiceModal(targetPath, thumbnail);
            this.controller = new Controller.ImageController(this.modal);
            this.logging = new LoggingService();
            this.logging.MessageRecieved += updateLog;
            this.m_imageServer = new Server.ImageServer(this.controller, this.logging);
        }

        /*
         * The function updates logger by a massage
         */
        private void updateLog(object sender, Logging.Modal.MessageRecievedEventArgs e)
        {
            this.eventLog1.WriteEntry(e.Message.ToString());
        }


        /*
         * The function stars the service process and update the logger
         */
        protected override void OnStart(string[] args) {

            // Update the service state to Start Pending.  
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            this.eventLog1.WriteEntry("In OnStart");
            // Set up a timer to trigger every minute.  
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 60000; // 60 seconds  
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            timer.Start();

            // Update the service state to Running.  
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }


        /*
         * The function stops the service and update logger
         */
        protected override void OnStop()
        {
            this.eventLog1.WriteEntry("In onStop.");
            this.m_imageServer.ServerClose();// OnCloseServer();
            this.eventLog1.WriteEntry("stop service.");
        }

        /*
         * The function update logger of countinuios activation
         */
        protected override void OnContinue()
        {
            eventLog1.WriteEntry("In OnContinue.");
        }


        /*
         * The function update logger on monitoring by system's timer
         */
        public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
        {
            eventLog1.WriteEntry("Monitoring the System", EventLogEntryType.Information, eventId++);
        }

        public void Start()
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), this.server_port);
            this.listener = new TcpListener(ep);
            this.listener.Start();
            Console.WriteLine("Waiting for connections...");

            Task task = new Task(() => {
                while (true)
                {
                    try
                    {
                        TcpClient client = this.listener.AcceptTcpClient();
                        Console.WriteLine("New connection");
                        // handle client ...
                    }
                    catch (Exception)
                    {
                        break;
                    }
                }
                Console.WriteLine("Server stopped");
            });
            task.Start();
        }

    }
}
