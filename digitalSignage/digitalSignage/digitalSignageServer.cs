using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using System.Diagnostics;
using System.Data.SQLite;

namespace digitalSignage
{
    class digitalSignageServer
    {
        digitalSignageServer() { }

        // Incoming data from client (gives commands)
        public static string data = null;

        public static string processRequest(string[] requestArray)
        {
            string command = requestArray[0];
            string data = requestArray[1];

            switch (command)
            {
                case "connect":
                    {
                        // verify succesful connection, complete socket
                        return "connectionSuccess";
                    }
                case "timetableEdit":
                    {
                        // update time table for classes
                        /*Debug.WriteLine("TIMETABLEs");
                        Debug.WriteLine(data);*/
                        // place data into db

                        // Connect to db, open to start taking queries
                        SQLiteConnection dbConnection = new SQLiteConnection("Data Source=mainDatabase.db;Version=3;");
                        string sql = "";
                        SQLiteCommand dbCommand = new SQLiteCommand(sql, dbConnection);

                        // by the time this code is executed it is known database exists.
                        dbConnection.Open();

                        // Data contains the sql query

                        // Construct command variable to execute commands with SQLite
                        dbCommand = new SQLiteCommand(data, dbConnection);
                        // Create table (execute command)
                        dbCommand.ExecuteNonQuery();

                        return "timetableInit";
                    }
                case "rssEdit":
                    {
                        // update RSS tables
                        /*Debug.WriteLine("rssEditting");
                        Debug.WriteLine(data);*/

                        // Connect to db, open to start taking queries
                        SQLiteConnection dbConnection = new SQLiteConnection("Data Source=mainDatabase.db;Version=3;");
                        string sql = "";
                        SQLiteCommand dbCommand = new SQLiteCommand(sql, dbConnection);

                        // by the time this code is executed it is known database exists.
                        dbConnection.Open();

                        // Data contains the sql query

                        // Construct command variable to execute commands with SQLite
                        dbCommand = new SQLiteCommand(data, dbConnection);
                        // Create table (execute command)
                        dbCommand.ExecuteNonQuery();

                        return "rssEditInit";
                    }
                case "noticesChanges":
                    {
                        // update Notices tables
                        /*Debug.WriteLine("testing");
                        Debug.WriteLine(data); */
                        // Connect to db, open to start taking queries
                        SQLiteConnection dbConnection = new SQLiteConnection("Data Source=mainDatabase.db;Version=3;");
                        string sql = "";
                        SQLiteCommand dbCommand = new SQLiteCommand(sql, dbConnection);

                        // by the time this code is executed it is known database exists.
                        dbConnection.Open();

                        // Data contains the sql query

                        // Construct command variable to execute commands with SQLite
                        dbCommand = new SQLiteCommand(data, dbConnection);
                        // Create table (execute command)
                        dbCommand.ExecuteNonQuery();

                        return "noticesChangeInit";
                    }
                default:
                    {
                        // Unrecognised command alert user
                        return "unrecognizedError";
                    }
            }
        }
        public static void startServer()
        {
            // Data buffer for incoming data
            byte[] bytes = new Byte[1024];

            // Establish local endpoint for the socket.
            // DNS name of computer
            // listen on local network (obviously static ip, as defined in subnet)
            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            // port 11000 used randomly
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 10042);

            // Create socket
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Bind socket to local address
            try
            {
                listener.Bind(localEndPoint);
                // 100ms refresh fairly std.
                listener.Listen(100);

                // socket created, wait for connection
                while (true)
                {
                    // Waiting for connection
                    // Thread suspended while waiting but that doesnt matter, because in own thread
                    Socket handler = listener.Accept();
                    data = null;

                    // Incoming connection being processed in own loop
                    while (true)
                    {
                        bytes = new byte[1024];
                        int bytesRec = handler.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        if (data.IndexOf("<EOF>") > -1)
                        {
                            break;
                        }
                    }
                    // Cleanse string of '<EOF>' declarations
                    data = data.ToString().Remove(data.Length - 5);
                    string[] requestArray = data.Split('~');

                    string[] revisedArray = new string[2];
                    if (requestArray.Length == 1)
                    {
                        revisedArray[0] = requestArray[0];
                        revisedArray[1] = "";
                    }
                    else
                    {
                        revisedArray[0] = requestArray[0];
                        revisedArray[1] = requestArray[1];
                    }

                    // Got data, send to function to proces request
                    byte[] msg = Encoding.ASCII.GetBytes(processRequest(revisedArray));
                    handler.Send(msg);
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }

            
        }
    }
}
