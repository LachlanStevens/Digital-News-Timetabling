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
    /// Interaction logic for notices.xaml
    /// </summary>
    public partial class notices : Window
    {
        public IPAddress ipAddress;
        public notices(IPAddress ip)
        {
            ipAddress = ip;
            InitializeComponent();
        }

        private void noticesButton_Click(object sender, RoutedEventArgs e)
        {
            if ((classChangesTxtBox.Text.Length <= 0) || (meetingChangesTxtBox.Text.Length <= 0) ||
               (specEventsTxtBox.Text.Length <= 0))
            {
                MessageBox.Show("Please ensure there is values in each box");
            }
            else
            {
                // For a final production project this would not be acceptable, but due to simplification having to be made
                // Trust user has placed correct values in box <-- very VERY unsafe
                string classRoomChanges = classChangesTxtBox.Text.Replace(System.Environment.NewLine, " \n");
                string meetingChanges = meetingChangesTxtBox.Text.Replace(System.Environment.NewLine, " \n");
                string specEvents = specEventsTxtBox.Text.Replace(System.Environment.NewLine, "\n");

                // Construct SQL for classRoomChanges
                string sql = String.Format(@"DELETE FROM classchange; VACUUM; INSERT INTO classchange (date, teacher) VALUES ('{0}', '{1}');"
                                              , DateTime.Now.Date, classRoomChanges);
                
                // Construct SQL for meetingChanges
                sql = sql + String.Format(@"DELETE FROM meetingreminder; VACUUM; INSERT INTO meetingreminder (date, name) VALUES ('{0}', '{1}');"
                                              , DateTime.Now.Date, meetingChanges);

                // Construct SQL for specEvents
                sql = sql + String.Format(@"DELETE FROM specevents; VACUUM; INSERT INTO specevents (date, event) VALUES ('{0}', '{1}');"
                                              , DateTime.Now.Date, specEvents);

                // Deal with apostrophes (they mess up sql) Simplifications will just tell user to not use apostrophes

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
                    byte[] msg = Encoding.ASCII.GetBytes("noticesChanges~" + sql + "<EOF>");

                    // Send the data through the socket.
                    int bytesSent = sendRequest.Send(msg);
                    // Receive the response from the remote device.
                    int bytesRec = sendRequest.Receive(bytes);

                    String response = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    if (response == "noticesChangeInit")
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
