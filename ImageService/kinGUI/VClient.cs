using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace kinGUI
{
    class VClient
    {
        private Socket sender;

        public VClient(string ip, int port)
        {
            try
            {
                //IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = IPAddress.Parse(ip);//ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
                this.sender = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message.ToString());
            }
        }

        public void sendMessage(string msg)
        {
            byte[] message = Encoding.ASCII.GetBytes(msg);
            sender.Send(message);
        }


        public string getMessage()
        {
            byte[] bytes = new byte[1024];
            int bytesRec = sender.Receive(bytes);
            return Encoding.ASCII.GetString(bytes, 0, bytesRec);
        }

        public void close()
        {
            this.sender.Shutdown(SocketShutdown.Both);
            this.sender.Close();
        }
    }
}
