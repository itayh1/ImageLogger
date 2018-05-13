using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;

namespace ImageService
{
    public class Communicator
    {
        private readonly int server_port = 8888;
        private TcpListener listener;
        //private List<TcpListener> clients;
        private LoggingService loggingService;
        private ConfigurationData data;
        public event EventHandler<CommandRecievedEventArgs> OnCommandRecieved;


        public Communicator(ConfigurationData configData, LoggingService loggingS)
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), this.server_port);
            this.listener = new TcpListener(ep);
            this.loggingService = loggingS;
            this.data = new ConfigurationData();
            data.outputDir = configData.outputDir;
            data.sourceName = configData.sourceName;
            data.logName = configData.logName;
            data.thumbnailSize = configData.thumbnailSize;
            data.handlers = configData.handlers;
        }

        public void Start()
        {
            this.listener.Start();
            Console.WriteLine("Waiting for connections...");

            while (true)
            {
                try
                {
                    TcpClient client = this.listener.AcceptTcpClient();
                    Console.WriteLine("New client connection");
                    this.loggingService.Log(string.Format("Client with socket {0} connected", client.ToString()), Logging.Modal.MessageTypeEnum.INFO);
                    HandleClient(client);
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                    break;
                }
            }
            Console.WriteLine("Server stopped");
        }

        public void HandleClient(TcpClient client)
        {


         //Task task = new Task(() => {

            // serialize command for settings
            var serializer = new JavaScriptSerializer();
            var serializedConfig = serializer.Serialize(this.data);
            CommandRecievedEventArgs command = new CommandRecievedEventArgs((int)CommandEnum.GetConfigCommand, new string[] { serializedConfig }, string.Empty);

            // send appconfig
            var serializedCmd = serializer.Serialize(command);
            this.sendMessage(serializedCmd, client);

            // serialize command for logs
            var serializedLogs = serializer.Serialize(this.loggingService.Logs);
            command = new CommandRecievedEventArgs((int)CommandEnum.GetListLogCommand, new string[] { serializedLogs }, string.Empty);

            // send logs
            serializedCmd = serializer.Serialize(command);
            this.sendMessage(serializedCmd, client);
            
            bool running = true;
            NetworkStream stream = client.GetStream();
            StreamReader reader = new StreamReader(stream);
            string msg = string.Empty;

            while (running)
            {
                try
                {
                    msg = reader.ReadLine();
                    Console.WriteLine("msg: {0}", msg);
                    CommandRecievedEventArgs e = new JavaScriptSerializer().Deserialize<CommandRecievedEventArgs>(msg);
                    OnCommandRecieved?.Invoke(this, e);
                }
                catch (Exception ex)
                {
                    running = false;
                    Console.WriteLine(ex.Message);
                }
            }
            client.Close();

            //  });
            //  task.Start();
        }

        public void sendMessage(string msg, TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            StreamWriter writer = new StreamWriter(stream) {
                AutoFlush = true
            };
            writer.WriteLine(msg);
        }


        public void Stop()
        {

        }
    }
}
