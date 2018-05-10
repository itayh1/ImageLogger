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
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message.ToString());
            }
        }

        public void sendMessage(string msg)
        {
            new Task(() =>
            {
                using (NetworkStream stream = client.GetStream())
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    //string args = JsonConvert.SerializeObject(e);
                    writer.Write(msg);
                }
            }).Start();
        }

        public void ReadMesagge()
        {
            new Task(() =>
            {
                while (client.Connected)
                {
                    using (NetworkStream stream = this.client.GetStream())
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string args = "";
                        while (reader.Peek() > 0)
                        {
                            args = args + reader.Read();
                        }
                        var serializer = new JavaScriptSerializer();
                        CommandRecievedEventArgs e = serializer.Deserialize<CommandRecievedEventArgs>(args);

                        OnCommandRecieved?.Invoke(this, e);
                    }
                }
            }).Start();
        }

        public void close()
        {
            //this.client.Shutdown(SocketShutdown.Both);
            this.client.Close();
        }
    }
}
