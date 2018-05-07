using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Web.Script.Serialization;


namespace ImageService
{
    public class Communicator
    {
        private int server_port = 5555;
        private TcpListener listener;
        private ConfigurationData data;

        public Communicator(string od, string sn, string ln, int ts, string[] dirs)
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), this.server_port);
            this.listener = new TcpListener(ep);
            this.data = new ConfigurationData();
            data.outputDir = od;
            data.sourceName = sn;
            data.logName = ln;
            data.thumbnailSize = ts;
            data.handlers = dirs;
        }

        public void Start()
        {
            this.listener.Start();
            Console.WriteLine("Waiting for connections...");
            Task task = new Task(() => {
                while (true)
                {
                    try
                    {
                        TcpClient client = this.listener.AcceptTcpClient();
                        Console.WriteLine("New connection");
                        HandleClient(client);
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
                 Modal.CommandRecievedEventArgs mc = (Modal.CommandRecievedEventArgs)
                    new JavaScriptSerializer().DeserializeObject(msg);

            }
        }

        public void Stop()
        {

        }
    }
}
