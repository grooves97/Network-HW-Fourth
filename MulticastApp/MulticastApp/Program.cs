using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace MulticastApp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                UdpClient sock = new UdpClient(9999);
                Console.WriteLine("Ready to receive...");

                sock.JoinMulticastGroup(IPAddress.Parse("127.0.0.1"), 50);

                IPEndPoint iep = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = sock.Receive(ref iep);
                string stringData = Encoding.ASCII.GetString(data, 0, data.Length);

                Console.WriteLine("received: {0}  from: {1}", stringData, iep.ToString());

                sock.Close();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }
    }
}
