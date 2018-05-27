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

        public async Task StartAsync()
        {
            this.listener.Start();
            Console.WriteLine("Waiting for connections...");

            while (true)
            {
                try
                {
                    var tcpClient = await this.listener.AcceptTcpClientAsync();
                    this.clients.Add(tcpClient);
                    this.loggingService.Logs.Add(new LogObject(MessageTypeEnum.INFO.ToString(),
                     string.Format("Client with socket {0} connected", tcpClient.ToString())));
                    Console.WriteLine("[Server] Client has connected");
                    var task = StartHandleConnectionAsync(tcpClient);
                    // if already faulted, re-throw any error on the calling context
                    if (task.IsFaulted)
                        task.Wait();
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                    break;
                }
            }
            Console.WriteLine("Server stopped");
        }

        object _lock = new Object(); // sync lock 
        List<Task> _connections = new List<Task>(); // pending connections

        private async Task StartHandleConnectionAsync(TcpClient tcpClient)
        {
            // start the new connection task
            var connectionTask = HandleConnectionAsync(tcpClient);

            // add it to the list of pending task 
            lock (_lock)
                _connections.Add(connectionTask);

            // catch all errors of HandleConnectionAsync
            try
            {
                await connectionTask;
                // we may be on another thread after "await"
            }
            catch (Exception ex)
            {
                // log the error
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                // remove pending task
                lock (_lock)
                    _connections.Remove(connectionTask);
            }
        }

        private Task HandleConnectionAsync(TcpClient client)
        {
            return Task.Run(async () =>
            {
                using (NetworkStream stream = client.GetStream())
                {
                    bool running = true;
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
                }
            });
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
