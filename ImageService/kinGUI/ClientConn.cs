using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Web.Script.Serialization;

namespace kinGUI
{
    class ClientConn
    {
        private static ClientConn singletonClient;

        private TcpClient client;

        public event EventHandler<CommandRecievedEventArgs> OnCommandRecieved;

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
                this.ReadMesagge();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message.ToString());
            }
        }

        public void sendMessage(string msg)
        {
            NetworkStream stream = client.GetStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.WriteLine(msg);
        }

        public void ReadMesagge()
        {
            new Task(() =>
            {
                try
                {
                    string arg;
                    NetworkStream stream = this.client.GetStream();
                    StreamReader reader = new StreamReader(stream);

                    while (client.Connected)
                    {
                        arg = reader.ReadLine();
                        var serializer = new JavaScriptSerializer();
                        CommandRecievedEventArgs e = serializer.Deserialize<CommandRecievedEventArgs>(arg);
                        //if (e.CommandID == (int)CommandEnum.ExitCommand)
                        //{
                        //    // Client want to exit.
                        //    break;
                        //}
                        OnCommandRecieved?.Invoke(this, e);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    client.Close();

                }
               
            }).Start();
        }

        public void close()
        {
            this.client.Close();
        }
    }
}
