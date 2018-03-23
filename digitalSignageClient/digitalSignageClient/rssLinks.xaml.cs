using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace digitalSignageClient
{
    /// <summary>
    /// Interaction logic for rssLinks.xaml
    /// </summary>
    public partial class rssLinks : Window
    {
        public IPAddress ipAddress;
        public rssLinks(IPAddress ip)
        {
            ipAddress = ip;
            InitializeComponent();
        }

        private void submitButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if everything has value in it
            if((firstTitle.Text.Length <= 0) || (firstLink.Text.Length <= 0) ||
               (secondTitle.Text.Length <= 0) || (secondLink.Text.Length <= 0) ||
               (thirdTitle.Text.Length <= 0) || (thirdLink.Text.Length <= 0) ||
               (fourthTitle.Text.Length <= 0) || (fourthLink.Text.Length <= 0) ||
               (fifthTitle.Text.Length <= 0) || (fifthLink.Text.Length <= 0))
            {
                MessageBox.Show("Please ensure there is values in each box");
            } else {
                // Send request to server (generate sql)
                // Originally used Truncate, but sqlite doesnt have that implemented; so need to drop and vacuum respectively
                string sql = String.Format( @"DELETE FROM rssFeed; VACUUM;
                                              INSERT INTO rssFeed (name, link) VALUES ('{0}', '{1}');
                                              INSERT INTO rssFeed (name, link) VALUES ('{2}', '{3}');
                                              INSERT INTO rssFeed (name, link) VALUES ('{4}', '{5}');
                                              INSERT INTO rssFeed (name, link) VALUES ('{6}', '{7}');
                                              INSERT INTO rssFeed (name, link) VALUES ('{8}', '{9}');"
                                              ,firstTitle.Text.ToString(), firstLink.Text.ToString(),
                                               secondTitle.Text.ToString(), secondLink.Text.ToString(),
                                               thirdTitle.Text.ToString(), thirdLink.Text.ToString(),
                                               fourthTitle.Text.ToString(), fourthLink.Text.ToString(),
                                               fifthTitle.Text.ToString(), fifthLink.Text.ToString());

                // Remove uneeded whitespace and newlines and carriage returns
                sql = sql.Replace(System.Environment.NewLine, string.Empty);
                sql = System.Text.RegularExpressions.Regex.Replace(sql, @"\s+", " ");

                // Now sql is completed, need to send off to server to accept and update respectively
                byte[] bytes = new byte[1024];

                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 10042);

                // Create socket
                Socket sendRequest = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // Connect socket to remote endpoint. try and catch for errors
                try
                {
                    sendRequest.Connect(remoteEP);

                    // Encode data to byte array
                    byte[] msg = Encoding.ASCII.GetBytes("rssEdit~" + sql + "<EOF>");

                    // Send the data through the socket.
                    int bytesSent = sendRequest.Send(msg);
                    // Receive the response from the remote device.
                    int bytesRec = sendRequest.Receive(bytes);

                    String response = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    if (response == "rssEditInit")
                    {
                        // Connection success, alert user
                        MessageBox.Show("Successfully updated changes");
                    }
                    else
                    {
                        MessageBox.Show("Error seems to have occured, try again");
                    }
                    // Close socket
                    sendRequest.Shutdown(SocketShutdown.Both);
                    sendRequest.Close();
                }
                catch (SocketException se)
                {
                    MessageBox.Show("Cant find server, try a different address?");
                }
                this.Close();

            }
        }
    }
}
