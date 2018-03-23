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
using System.Windows.Shapes;

using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace digitalSignageClient
{
    /// <summary>
    /// Interaction logic for timeTable.xaml
    /// </summary>
    public partial class timeTable : Window
    {
        public IPAddress ipAddress;
        public timeTable(IPAddress ip)
        {
            ipAddress = ip;
            InitializeComponent();
        }
        private char comboBoxReturn(System.Windows.Controls.ComboBox comboBox)
        {
            char selectedChar = 'A';
            int index = comboBox.SelectedIndex;
            switch (index)
            {
                case 0:
                    {
                        selectedChar = 'A';
                        break;
                    }
                case 1:
                    {
                        selectedChar = 'B';
                        break;
                    }
                case 2:
                    {
                        selectedChar = 'C';
                        break;

                    }
                case 3:
                    {
                        selectedChar = 'D';
                        break;
                    }
                case 4:
                    {
                        selectedChar = 'E';
                        break;
                    }
                case 5:
                    {
                        selectedChar = 'F';
                        break;
                    }
            }
            return selectedChar;
        }
        private void submitButton_Click(object sender, RoutedEventArgs e)
        {
            // Open up db and close form
            if ((lne1mon1.SelectedIndex == -1) || (lne2mon1.SelectedIndex == -1) || (lne3mon1.SelectedIndex == -1) || (lne4mon1.SelectedIndex == -1) || (lne5mon1.SelectedIndex == -1) || (lne6mon1.SelectedIndex == -1) || 
                (lne1tue1.SelectedIndex == -1) || (lne2tue1.SelectedIndex == -1) || (lne3tue1.SelectedIndex == -1) || (lne4tue1.SelectedIndex == -1) || (lne5tue1.SelectedIndex == -1) || (lne6tue1.SelectedIndex == -1) || 
                (lne1wed1.SelectedIndex == -1) || (lne2wed1.SelectedIndex == -1) || (lne3wed1.SelectedIndex == -1) || (lne4wed1.SelectedIndex == -1) || (lne5wed1.SelectedIndex == -1) || (lne6wed1.SelectedIndex == -1) || 
                (lne1thu1.SelectedIndex == -1) || (lne2thu1.SelectedIndex == -1) || (lne3thu1.SelectedIndex == -1) || (lne4thu1.SelectedIndex == -1) || (lne5thu1.SelectedIndex == -1) || (lne6thu1.SelectedIndex == -1) || 
                (lne1fri1.SelectedIndex == -1) || (lne2fri1.SelectedIndex == -1) || (lne3fri1.SelectedIndex == -1) || (lne4fri1.SelectedIndex == -1) || (lne5fri1.SelectedIndex == -1) || (lne6fri1.SelectedIndex == -1) ||
                (lne1mon2.SelectedIndex == -1) || (lne2mon2.SelectedIndex == -1) || (lne3mon2.SelectedIndex == -1) || (lne4mon2.SelectedIndex == -1) || (lne5mon2.SelectedIndex == -1) || (lne6mon2.SelectedIndex == -1) || 
                (lne1tue2.SelectedIndex == -1) || (lne2tue2.SelectedIndex == -1) || (lne3tue2.SelectedIndex == -1) || (lne4tue2.SelectedIndex == -1) || (lne5tue2.SelectedIndex == -1) || (lne6tue2.SelectedIndex == -1) || 
                (lne1wed2.SelectedIndex == -1) || (lne2wed2.SelectedIndex == -1) || (lne3wed2.SelectedIndex == -1) || (lne4wed2.SelectedIndex == -1) || (lne5wed2.SelectedIndex == -1) || (lne6wed2.SelectedIndex == -1) || 
                (lne1thu2.SelectedIndex == -1) || (lne2thu2.SelectedIndex == -1) || (lne3thu2.SelectedIndex == -1) || (lne4thu2.SelectedIndex == -1) || (lne5thu2.SelectedIndex == -1) || (lne6thu2.SelectedIndex == -1) || 
                (lne1fri2.SelectedIndex == -1) || (lne2fri2.SelectedIndex == -1) || (lne3fri2.SelectedIndex == -1) || (lne4fri2.SelectedIndex == -1) || (lne5fri2.SelectedIndex == -1) || (lne6fri2.SelectedIndex == -1)
                )
            {
                MessageBox.Show(yrLevel.SelectedValue.ToString());
                MessageBox.Show("One class line isn't selected, Please select everything to continue.");
            } else if (yrLevel.SelectedIndex == -1) {
                MessageBox.Show("Please Selected a Year Level");
            } else {
                int yr = 0;
                // Generate SQL Query to send
                if (yrLevel.SelectedIndex == 0) {
                    // Grade 11 selected
                    yr = 11;
                } else {
                    // Grade 12 selected
                    yr = 12;
                }

                string sql = String.Format(  @"UPDATE timetable SET mon='{1}' WHERE yr={0} AND period=1 AND wk=1;
                                               UPDATE timetable SET mon='{2}' WHERE yr={0} AND period=2 AND wk=1;
                                               UPDATE timetable SET mon='{3}' WHERE yr={0} AND period=3 AND wk=1;
                                               UPDATE timetable SET mon='{4}' WHERE yr={0} AND period=4 AND wk=1;
                                               UPDATE timetable SET mon='{5}' WHERE yr={0} AND period=5 AND wk=1;
                                               UPDATE timetable SET mon='{6}' WHERE yr={0} AND period=6 AND wk=1;
                                               UPDATE timetable SET tue='{7}' WHERE yr={0} AND period=1 AND wk=1;
                                               UPDATE timetable SET tue='{8}' WHERE yr={0} AND period=2 AND wk=1;
                                               UPDATE timetable SET tue='{9}' WHERE yr={0} AND period=3 AND wk=1;
                                               UPDATE timetable SET tue='{10}' WHERE yr={0} AND period=4 AND wk=1;
                                               UPDATE timetable SET tue='{11}' WHERE yr={0} AND period=5 AND wk=1;
                                               UPDATE timetable SET tue='{12}' WHERE yr={0} AND period=6 AND wk=1;
                                               UPDATE timetable SET wed='{13}' WHERE yr={0} AND period=1 AND wk=1;
                                               UPDATE timetable SET wed='{14}' WHERE yr={0} AND period=2 AND wk=1;
                                               UPDATE timetable SET wed='{15}' WHERE yr={0} AND period=3 AND wk=1;
                                               UPDATE timetable SET wed='{16}' WHERE yr={0} AND period=4 AND wk=1;
                                               UPDATE timetable SET wed='{17}' WHERE yr={0} AND period=5 AND wk=1;
                                               UPDATE timetable SET wed='{18}' WHERE yr={0} AND period=6 AND wk=1;
                                               UPDATE timetable SET thu='{19}' WHERE yr={0} AND period=1 AND wk=1;
                                               UPDATE timetable SET thu='{20}' WHERE yr={0} AND period=2 AND wk=1;
                                               UPDATE timetable SET thu='{21}' WHERE yr={0} AND period=3 AND wk=1;
                                               UPDATE timetable SET thu='{22}' WHERE yr={0} AND period=4 AND wk=1;
                                               UPDATE timetable SET thu='{23}' WHERE yr={0} AND period=5 AND wk=1;
                                               UPDATE timetable SET thu='{24}' WHERE yr={0} AND period=6 AND wk=1;
                                               UPDATE timetable SET fri='{25}' WHERE yr={0} AND period=1 AND wk=1;
                                               UPDATE timetable SET fri='{26}' WHERE yr={0} AND period=2 AND wk=1;
                                               UPDATE timetable SET fri='{27}' WHERE yr={0} AND period=3 AND wk=1;
                                               UPDATE timetable SET fri='{28}' WHERE yr={0} AND period=4 AND wk=1;
                                               UPDATE timetable SET fri='{29}' WHERE yr={0} AND period=5 AND wk=1;
                                               UPDATE timetable SET fri='{30}' WHERE yr={0} AND period=6 AND wk=1;
                                               UPDATE timetable SET mon='{31}' WHERE yr={0} AND period=1 AND wk=2;
                                               UPDATE timetable SET mon='{32}' WHERE yr={0} AND period=2 AND wk=2;
                                               UPDATE timetable SET mon='{33}' WHERE yr={0} AND period=3 AND wk=2;
                                               UPDATE timetable SET mon='{34}' WHERE yr={0} AND period=4 AND wk=2;
                                               UPDATE timetable SET mon='{35}' WHERE yr={0} AND period=5 AND wk=2;
                                               UPDATE timetable SET mon='{36}' WHERE yr={0} AND period=6 AND wk=2;
                                               UPDATE timetable SET tue='{37}' WHERE yr={0} AND period=1 AND wk=2;
                                               UPDATE timetable SET tue='{33}' WHERE yr={0} AND period=2 AND wk=2;
                                               UPDATE timetable SET tue='{34}' WHERE yr={0} AND period=3 AND wk=2;
                                               UPDATE timetable SET tue='{35}' WHERE yr={0} AND period=4 AND wk=2;
                                               UPDATE timetable SET tue='{36}' WHERE yr={0} AND period=5 AND wk=2;
                                               UPDATE timetable SET tue='{37}' WHERE yr={0} AND period=6 AND wk=2;
                                               UPDATE timetable SET wed='{38}' WHERE yr={0} AND period=1 AND wk=2;
                                               UPDATE timetable SET wed='{39}' WHERE yr={0} AND period=2 AND wk=2;
                                               UPDATE timetable SET wed='{40}' WHERE yr={0} AND period=3 AND wk=2;
                                               UPDATE timetable SET wed='{41}' WHERE yr={0} AND period=4 AND wk=2;
                                               UPDATE timetable SET wed='{42}' WHERE yr={0} AND period=5 AND wk=2;
                                               UPDATE timetable SET wed='{43}' WHERE yr={0} AND period=6 AND wk=2;
                                               UPDATE timetable SET thu='{44}' WHERE yr={0} AND period=1 AND wk=2;
                                               UPDATE timetable SET thu='{45}' WHERE yr={0} AND period=2 AND wk=2;
                                               UPDATE timetable SET thu='{46}' WHERE yr={0} AND period=3 AND wk=2;
                                               UPDATE timetable SET thu='{47}' WHERE yr={0} AND period=4 AND wk=2;
                                               UPDATE timetable SET thu='{48}' WHERE yr={0} AND period=5 AND wk=2;
                                               UPDATE timetable SET thu='{49}' WHERE yr={0} AND period=6 AND wk=2;
                                               UPDATE timetable SET fri='{50}' WHERE yr={0} AND period=1 AND wk=2;
                                               UPDATE timetable SET fri='{51}' WHERE yr={0} AND period=2 AND wk=2;
                                               UPDATE timetable SET fri='{52}' WHERE yr={0} AND period=3 AND wk=2;
                                               UPDATE timetable SET fri='{53}' WHERE yr={0} AND period=4 AND wk=2;
                                               UPDATE timetable SET fri='{54}' WHERE yr={0} AND period=5 AND wk=2;
                                               UPDATE timetable SET fri='{55}' WHERE yr={0} AND period=6 AND wk=2;", yr,
                                               comboBoxReturn(lne1mon1), comboBoxReturn(lne2mon1), comboBoxReturn(lne3mon1), comboBoxReturn(lne4mon1), comboBoxReturn(lne5mon1), comboBoxReturn(lne6mon1), 
                                               comboBoxReturn(lne1tue1), comboBoxReturn(lne2tue1), comboBoxReturn(lne3tue1), comboBoxReturn(lne4tue1), comboBoxReturn(lne5tue1), comboBoxReturn(lne6tue1),
                                               comboBoxReturn(lne1wed1), comboBoxReturn(lne2wed1), comboBoxReturn(lne3wed1), comboBoxReturn(lne4wed1), comboBoxReturn(lne5wed1), comboBoxReturn(lne6wed1),
                                               comboBoxReturn(lne1thu1), comboBoxReturn(lne2thu1), comboBoxReturn(lne3thu1), comboBoxReturn(lne4thu1), comboBoxReturn(lne5thu1), comboBoxReturn(lne6thu1),
                                               comboBoxReturn(lne1fri1), comboBoxReturn(lne2fri1), comboBoxReturn(lne3fri1), comboBoxReturn(lne4fri1), comboBoxReturn(lne5fri1), comboBoxReturn(lne6fri1),
                                               comboBoxReturn(lne1mon2), comboBoxReturn(lne2mon2), comboBoxReturn(lne3mon2), comboBoxReturn(lne4mon2), comboBoxReturn(lne5mon2), comboBoxReturn(lne6mon2),
                                               comboBoxReturn(lne1tue2), comboBoxReturn(lne2tue2), comboBoxReturn(lne3tue2), comboBoxReturn(lne4tue2), comboBoxReturn(lne5tue2), comboBoxReturn(lne6tue2),
                                               comboBoxReturn(lne1wed2), comboBoxReturn(lne2wed2), comboBoxReturn(lne3wed2), comboBoxReturn(lne4wed2), comboBoxReturn(lne5wed2), comboBoxReturn(lne6wed2),
                                               comboBoxReturn(lne1thu2), comboBoxReturn(lne2thu2), comboBoxReturn(lne3thu2), comboBoxReturn(lne4thu2), comboBoxReturn(lne5thu2), comboBoxReturn(lne6thu2),
                                               comboBoxReturn(lne1fri2), comboBoxReturn(lne2fri2), comboBoxReturn(lne3fri2), comboBoxReturn(lne4fri2), comboBoxReturn(lne5fri2), comboBoxReturn(lne6fri2));
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
                    byte[] msg = Encoding.ASCII.GetBytes("timetableEdit~" + sql + "<EOF>");

                    // Send the data through the socket.
                    int bytesSent = sendRequest.Send(msg);
                    // Receive the response from the remote device.
                    int bytesRec = sendRequest.Receive(bytes);

                    String response = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    if (response == "timetableInit")
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
