using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;
using ImageService.LoggingModal;
using Newtonsoft.Json;

namespace ImageService
{
    public class Communicator
    {
        private static Mutex mutex = new Mutex();

        private readonly int server_port = 8888;
        private TcpListener listener;
        private List<TcpClient> clients;
        private LoggingService loggingService;
        public event EventHandler<CommandRecievedEventArgs> OnCommandRecieved;


        public Communicator(ConfigurationData configData, LoggingService loggingS)
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), this.server_port);
            this.listener = new TcpListener(ep);
            this.loggingService = loggingS;
            this.Configurations = configData;
            this.clients = new List<TcpClient>();
        }

        public ConfigurationData Configurations;
       
        public void Start()
        {
            this.listener.Start();
            Console.WriteLine("Waiting for connections...");

            //Task task = new Task(() =>
            //{
                while (true)
                {
                    try
                    {
                        TcpClient client = this.listener.AcceptTcpClient();
                        this.clients.Add(client);
                        Console.WriteLine("New client connection");
                        this.loggingService.Logs.Add(new LogObject(MessageTypeEnum.INFO.ToString(),
                            string.Format("Client with socket {0} connected", client.ToString())));
                        HandleClient(client);
                    }
                    catch (Exception e)
                    {
                        Console.Write(e.Message);
                        break;
                    }
                }
                Console.WriteLine("Server stopped");
           // });
           // task.Start();
        }

        public void HandleClient(TcpClient client)
        {

            //Task task = new Task(() =>
            //{

                bool running = true;
                NetworkStream stream = client.GetStream();
                BinaryReader reader = new BinaryReader(stream);
                string msg = string.Empty;
                SetConfigsAndLogs(client);

                while (running)
                {
                    try
                    {
                        msg = reader.ReadString();
                        Console.WriteLine("msg: {0}", msg);
                        CommandRecievedEventArgs cmd = JsonConvert.DeserializeObject<CommandRecievedEventArgs>(msg);
                        Console.WriteLine("command is: {0}", cmd);
                        // client exit
                        if (cmd.CommandID == (int)CommandEnum.ExitCommand)
                        {
                            client.Close();
                            clients.Remove(client);
                            Console.WriteLine("Client removed");
                            break;
                        }
                        OnCommandRecieved?.Invoke(this, cmd);
                    }
                    catch (Exception ex)
                    {
                        running = false;
                        clients.Remove(client);
                        Console.WriteLine(ex.Message);
                    }
                }
                Console.WriteLine("closing client");
            //});
            //task.Start();
        }

        public void SendMessage(string msg, TcpClient client)
        {
                NetworkStream stream = client.GetStream();
                BinaryWriter writer = new BinaryWriter(stream);
                mutex.WaitOne();
            
                writer.Write(msg);
                writer.Flush();
              
                mutex.ReleaseMutex();

        }


        public void SetConfigsAndLogs(TcpClient client)
        {
            // serialize command for settings
            var serializedConfig = JsonConvert.SerializeObject(Configurations);
            CommandRecievedEventArgs command = new CommandRecievedEventArgs((int)CommandEnum.GetConfigCommand, new string[] { serializedConfig }, string.Empty);

            // send appconfig
            var serializedCmd = JsonConvert.SerializeObject(command);
            this.SendMessage(serializedCmd, client);

            // serialize command for logs
            var serializedLogs = JsonConvert.SerializeObject(this.loggingService.Logs);
            command = new CommandRecievedEventArgs((int)CommandEnum.GetListLogCommand, new string[] { serializedLogs }, string.Empty);

            // send logs
            serializedCmd = JsonConvert.SerializeObject(command);
            this.SendMessage(serializedCmd, client);
        }

        public void SendCommandBroadCast(CommandRecievedEventArgs cmd)
        {
            var serializedCmd = JsonConvert.SerializeObject(cmd);

            foreach (TcpClient client in clients)
            {
                    this.SendMessage(serializedCmd, client);
            }

        }

        public void Close()
        {
            CommandRecievedEventArgs command = new CommandRecievedEventArgs((int)CommandEnum.ExitCommand, null, string.Empty);
            // Send for every client to exit.
            listener.Stop();
        }
    }
}
