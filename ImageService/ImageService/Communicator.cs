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
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("192.168.5.180"), this.server_port);
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

           // Task task = new Task(() => {

                TcpClient client = this.listener.AcceptTcpClient();
                Console.WriteLine("New client connection");
                this.loggingService.Log(string.Format("Client with socket {0} connected",client.ToString()), Logging.Modal.MessageTypeEnum.INFO);
                
                // serialize command for settings
                var serializer = new JavaScriptSerializer();
                var serializedConfig = serializer.Serialize(this.data);
                CommandRecievedEventArgs command = new CommandRecievedEventArgs((int)CommandEnum.GetConfigCommand,
                    new string[] { serializedConfig }, string.Empty);
                
                // send appconfig
                var serializedCmd = serializer.Serialize(command);
                this.sendMessage(serializedCmd, client);
                
                // serialize command for logs
                var serializedLogs = serializer.Serialize(this.loggingService.Logs);
                command = new CommandRecievedEventArgs((int)CommandEnum.GetListLogCommand, 
                    new string[] { serializedLogs }, string.Empty);

                // send logs
                serializedCmd = serializer.Serialize(command);
                this.sendMessage(serializedLogs, client);
                try
                {
                    HandleClient(client);
                } catch (Exception e) {
                    Console.Write(e.Message);
                }

                Console.WriteLine("Server stopped");
          //  });
          //  task.Start();
        }

        public void HandleClient(TcpClient client)
        {
            bool running = true;
            while (running)
            {
                Byte[] data = new Byte[1024];
                string msg = string.Empty;
                NetworkStream stream = client.GetStream();
                Int32 bytes = stream.Read(data, 0, data.Length);
                msg = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine("msg: {0}",msg);
                CommandRecievedEventArgs e = (CommandRecievedEventArgs) new JavaScriptSerializer().DeserializeObject(msg);
                OnCommandRecieved?.Invoke(this, e);

            }
        }

        public void sendMessage(string msg, TcpClient client)
        {
            //new Task(() =>
            //{
            NetworkStream stream = client.GetStream();
            StreamWriter writer = new StreamWriter(stream)
            {
                AutoFlush = true
            };
 

            writer.WriteLine(msg);
                
                
                
            //}).Start();
        }


        public void Stop()
        {

        }
    }
}
