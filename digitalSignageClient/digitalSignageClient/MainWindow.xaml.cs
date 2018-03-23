using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Net;
using System.Net.Sockets;
using System.Diagnostics;


namespace digitalSignageClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        public IPAddress ipAddress;
        private void Window_Initialized(object windowSender, EventArgs eDef) { }

        private void connectButton_Click(object sender, RoutedEventArgs e)
        {

            // Check status is currently not connected, if false do nothing
            if ((string)currentStatus.Content == "Disconnected")
            {
                byte[] bytes = new byte[1024];

                // Establish the remote endpoint for the socket.
                // This example uses port 11000 on the local computer.
                
                /*IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
                IPAddress ipAddress1 = ipHostInfo.AddressList[0];*/
                
                
                if (IPAddress.TryParse(txtBoxIPAddress.Text, out ipAddress))
                {
                    // Valid ip continue.
                    IPEndPoint remoteEP = new IPEndPoint(ipAddress, 10042);

                    // Create socket
                    Socket sendRequest = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    // Connect socket to remote endpoint. try and catch for errors
                    try
                    {
                        sendRequest.Connect(remoteEP);

                        // Encode data to byte array
                        byte[] msg = Encoding.ASCII.GetBytes("connect~success?<EOF>");

                        // Send the data through the socket.
                        int bytesSent = sendRequest.Send(msg);
                        // Receive the response from the remote device.
                        int bytesRec = sendRequest.Receive(bytes);

                        String response = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        if (response == "connectionSuccess")
                        {
                            // Connection success, change status
                            currentStatus.Content = "Connected";
                            MessageBox.Show("Connection success");
                        }
                        else
                        {
                            MessageBox.Show("Error seems to have occured, try again");
                        }
                        // Close socket
                        sendRequest.Shutdown(SocketShutdown.Both);
                        sendRequest.Close();
                    } catch (SocketException se) {
                        MessageBox.Show("Cant find server, try a different address?");
                    }
                } else {
                    // Invalid ip throw error message
                    MessageBox.Show("Cant find server, try a different address?");
                }
            }
        }

        private void noticesButton_Click(object sender, RoutedEventArgs e)
        {
            // Check status is currently connected, if false do nothing
            if ((string)currentStatus.Content == "Connected")
            {
                new notices(ipAddress).Show();
            }
        }

        private void rssButton_Click(object sender, RoutedEventArgs e)
        {
            // Check status is currently connected, if false do nothing
            if ((string)currentStatus.Content == "Connected")
            {
                new rssLinks(ipAddress).Show();
            }
        }

        private void timetableButton_Click(object sender, RoutedEventArgs e)
        {
            // Check status is currently connected, if false do nothing
            if ((string)currentStatus.Content == "Connected")
            {
                new timeTable(ipAddress).Show();
            }
        }
    }
}
