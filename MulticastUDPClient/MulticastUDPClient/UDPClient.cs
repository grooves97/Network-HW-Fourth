using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace MulticastUDPClient
{
    public class UDPClient
    {
        UdpClient _udpclient;
        int _port;
        IPAddress _multicastIPaddress;
        IPAddress _localIPaddress;
        IPEndPoint _localEndPoint;
        IPEndPoint _remoteEndPoint;

        public UDPClient(IPAddress multicastIPaddress, int port, IPAddress localIPaddress = null)
        {
            // Store params
            _multicastIPaddress = multicastIPaddress;
            _port = port;
            _localIPaddress = localIPaddress;
            if (localIPaddress == null)
                _localIPaddress = IPAddress.Any;

            // Create endpoints
            _remoteEndPoint = new IPEndPoint(_multicastIPaddress, port);
            _localEndPoint = new IPEndPoint(_localIPaddress, port);

            // Create and configure UdpClient
            _udpclient = new UdpClient();
            // The following three lines allow multiple clients on the same PC
            _udpclient.ExclusiveAddressUse = false;
            _udpclient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            _udpclient.ExclusiveAddressUse = false;
            // Bind, Join
            _udpclient.Client.Bind(_localEndPoint);
            _udpclient.JoinMulticastGroup(_multicastIPaddress, _localIPaddress);

            // Start listening for incoming data
            _udpclient.BeginReceive(new AsyncCallback(ReceivedCallback), null);
        }

        private void ReceivedCallback(IAsyncResult ar)
        {
            // Get received data
            IPEndPoint sender = new IPEndPoint(0, 0);
            Byte[] receivedBytes = _udpclient.EndReceive(ar, ref sender);

            // fire event if defined
            if (UdpMessageReceived != null)
                UdpMessageReceived(this, new UdpMessageReceivedEventArgs() { Buffer = receivedBytes });

            // Restart listening for udp data packages
            _udpclient.BeginReceive(new AsyncCallback(ReceivedCallback), null);
        }

        public void SendMulticast(byte[] bufferToSend)
        {
            _udpclient.Send(bufferToSend, bufferToSend.Length, _remoteEndPoint);
        }

        public event EventHandler<UdpMessageReceivedEventArgs> UdpMessageReceived;

        public class UdpMessageReceivedEventArgs : EventArgs
        {
            public byte[] Buffer { get; set; }
        }
    }
}
