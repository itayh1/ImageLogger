using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using Newtonsoft.Json;

namespace kinGUI
{
    class ClientConn
    {
        private static ClientConn singletonClient;

        private TcpClient client;

        public event EventHandler<CommandRecievedEventArgs> OnCommandRecieved;
        NetworkStream stream;
        private static Mutex mtx = new Mutex();
        private bool connected;

        private readonly string ip = "127.0.0.1";
        private readonly int port = 8888;

        public static ClientConn Instance
        {
            get
            {
                if (singletonClient == null)
                {
                    singletonClient = new ClientConn();
                }
                return singletonClient;
            }
        }

        private ClientConn()
        {
            try
            {
                IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ip), port);
                this.client = new TcpClient();  
                this.client.Connect(ep);
                this.Connected = true;
                this.ReadMesagge();
            }
            catch (Exception e)
            {
                this.connected = false;
                Console.WriteLine(e.Message.ToString());
            }
        }

        public void sendMessage(string msg)
        {
            Task task = new Task(() =>
            {
                try
                {
                    Console.WriteLine("Sending message " + msg);
                    stream = client.GetStream();
                    BinaryWriter writer = new BinaryWriter(stream);
                    mtx.WaitOne();
                    writer.Write(msg);
                    writer.Flush();
                    mtx.ReleaseMutex();
                } catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            });
            task.Start();
        }

        object o = new object();
        public void ReadMesagge()
        {
            new Task(() =>
            {
                try
                {
                    string arg;
                    stream = this.client.GetStream();
                    BinaryReader reader = new BinaryReader(stream);

                    while (client.Connected)
                    {
                        lock (o)
                        {
                            arg = reader.ReadString();
                            Console.WriteLine("recieved " + arg);
                            CommandRecievedEventArgs e = JsonConvert.DeserializeObject<CommandRecievedEventArgs>(arg);
                            //if (e.CommandID == (int)CommandEnum.ExitCommand)
                            //{
                            //    // Client want to exit.
                            //    break;
                            //}
                            //Thread.Sleep(1000);
                            OnCommandRecieved?.Invoke(this, e);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                //finally
                //{
                //    client.Close();

                //}
               
            }).Start();
        }

        public bool Connected
        {
            get; set;
        }

        public void close()
        {
            this.client.Close();
        }
    }
}
