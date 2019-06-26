using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Net;
using System.Net.Sockets;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MulticastUDPClient
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        UDPClient udpClientWrapper;
        int i = 1;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            // Create address objects
            int port = Int32.Parse(txtPort.Text);
            IPAddress multicastIPaddress = IPAddress.Parse(txtRemoteIP.Text);
            IPAddress localIPaddress = IPAddress.Any;

            // Create MulticastUdpClient
            udpClientWrapper = new UDPClient(multicastIPaddress, port, localIPaddress);
            udpClientWrapper.UdpMessageReceived += OnUdpMessageReceived;

            AddToLog("UDP Client started");
        }

        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            // Generate some message bytes
            string msgString = String.Format("Message from {0} pid {1} #{2}",
                GetLocalIPAddress(),
                System.Diagnostics.Process.GetCurrentProcess().Id,
                i.ToString());
            i++;
            byte[] buffer = Encoding.Unicode.GetBytes(msgString);

            // Send
            udpClientWrapper.SendMulticast(buffer);
            AddToLog("Sent message: " + msgString);
        }

        private void OnUdpMessageReceived(object sender, UDPClient.UdpMessageReceivedEventArgs e)
        {
            string receivedText = ASCIIEncoding.Unicode.GetString(e.Buffer);
            AddToLog("Received message: " + receivedText);
        }

        private void AddToLog(string s)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                txtLog.Text += Environment.NewLine;
                txtLog.Text += s;
            }), null);
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }
    }
}
