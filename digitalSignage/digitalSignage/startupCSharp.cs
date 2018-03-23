using System.Windows; // Application, StartupEventArgs, WindowState (lets us listen to xaml requests)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Sqlite references
using System.Data.SQLite;
using System.IO;


using System.Diagnostics;
using System.Threading;


namespace digitalSignage
{
    class startupCSharp
    {
        
    }

    public partial class App : Application
    {
        void App_Startup(object sender, StartupEventArgs e)
        {
            // Start db connection
            SQLiteConnection dbConnection = new SQLiteConnection("Data Source=mainDatabase.db;Version=3;");
            string sql = "";
            SQLiteCommand dbCommand = new SQLiteCommand(sql, dbConnection);


            // Check if db exists (if not, create it)
            if (!(File.Exists("mainDatabase.db")))
            {
                // Database doesnt exist, create and input default values
                SQLiteConnection.CreateFile("mainDatabase.db");

                // Connection to db open, start executing queries
                dbConnection.Open();

                // SQL to create timetable table
                sql = @"CREATE TABLE timetable (
                          id int(11),
                          yr varchar(45) DEFAULT NULL,
                          wk varchar(45) DEFAULT NULL,
                          period varchar(45) DEFAULT NULL,
                          mon varchar(45) DEFAULT NULL,
                          tue varchar(45) DEFAULT NULL,
                          wed varchar(45) DEFAULT NULL,
                          thu varchar(45) DEFAULT NULL,
                          fri varchar(45) DEFAULT NULL,
                          PRIMARY KEY (id)
                        )";

                // Construct command variable to execute commands with SQLite
                dbCommand = new SQLiteCommand(sql, dbConnection);
                // Create table (execute command)
                dbCommand.ExecuteNonQuery();

                // fill table with default timetable values (will be changed later by user)
                sql = @"INSERT INTO timetable (yr, wk, period, mon, tue, wed, thu, fri) VALUES ('11', '1', '1', 'A', 'B', 'C', 'D', 'E');
                        INSERT INTO timetable (yr, wk, period, mon, tue, wed, thu, fri) VALUES ('11', '1', '2', 'A', 'B', 'C', 'D', 'E');
                        INSERT INTO timetable (yr, wk, period, mon, tue, wed, thu, fri) VALUES ('11', '1', '3', 'A', 'B', 'C', 'D', 'E');
                        INSERT INTO timetable (yr, wk, period, mon, tue, wed, thu, fri) VALUES ('11', '1', '4', 'A', 'B', 'C', 'D', 'E');
                        INSERT INTO timetable (yr, wk, period, mon, tue, wed, thu, fri) VALUES ('11', '1', '5', 'A', 'B', 'C', 'D', 'E');
                        INSERT INTO timetable (yr, wk, period, mon, tue, wed, thu, fri) VALUES ('11', '1', '6', 'A', 'B', 'C', 'D', 'E');
                        INSERT INTO timetable (yr, wk, period, mon, tue, wed, thu, fri) VALUES ('11', '2', '1', 'A', 'B', 'C', 'D', 'E');
                        INSERT INTO timetable (yr, wk, period, mon, tue, wed, thu, fri) VALUES ('11', '2', '2', 'A', 'B', 'C', 'D', 'E');
                        INSERT INTO timetable (yr, wk, period, mon, tue, wed, thu, fri) VALUES ('11', '2', '3', 'A', 'B', 'C', 'D', 'E');
                        INSERT INTO timetable (yr, wk, period, mon, tue, wed, thu, fri) VALUES ('11', '2', '4', 'A', 'B', 'C', 'D', 'E');
                        INSERT INTO timetable (yr, wk, period, mon, tue, wed, thu, fri) VALUES ('11', '2', '5', 'A', 'B', 'C', 'D', 'E');
                        INSERT INTO timetable (yr, wk, period, mon, tue, wed, thu, fri) VALUES ('11', '2', '6', 'A', 'B', 'C', 'D', 'E');
                        INSERT INTO timetable (yr, wk, period, mon, tue, wed, thu, fri) VALUES ('12', '1', '1', 'A', 'B', 'C', 'D', 'E');
                        INSERT INTO timetable (yr, wk, period, mon, tue, wed, thu, fri) VALUES ('12', '1', '2', 'A', 'B', 'C', 'D', 'E');
                        INSERT INTO timetable (yr, wk, period, mon, tue, wed, thu, fri) VALUES ('12', '1', '3', 'A', 'B', 'C', 'D', 'E');
                        INSERT INTO timetable (yr, wk, period, mon, tue, wed, thu, fri) VALUES ('12', '1', '4', 'A', 'B', 'C', 'D', 'E');
                        INSERT INTO timetable (yr, wk, period, mon, tue, wed, thu, fri) VALUES ('12', '1', '5', 'A', 'B', 'C', 'D', 'E');
                        INSERT INTO timetable (yr, wk, period, mon, tue, wed, thu, fri) VALUES ('12', '1', '6', 'A', 'B', 'C', 'D', 'E');
                        INSERT INTO timetable (yr, wk, period, mon, tue, wed, thu, fri) VALUES ('12', '2', '1', 'A', 'B', 'C', 'D', 'E');
                        INSERT INTO timetable (yr, wk, period, mon, tue, wed, thu, fri) VALUES ('12', '2', '2', 'A', 'B', 'C', 'D', 'E');
                        INSERT INTO timetable (yr, wk, period, mon, tue, wed, thu, fri) VALUES ('12', '2', '3', 'A', 'B', 'C', 'D', 'E');
                        INSERT INTO timetable (yr, wk, period, mon, tue, wed, thu, fri) VALUES ('12', '2', '4', 'A', 'B', 'C', 'D', 'E');
                        INSERT INTO timetable (yr, wk, period, mon, tue, wed, thu, fri) VALUES ('12', '2', '5', 'A', 'B', 'C', 'D', 'E');
                        INSERT INTO timetable (yr, wk, period, mon, tue, wed, thu, fri) VALUES ('12', '2', '6', 'A', 'B', 'C', 'D', 'E');";

                // Construct command variable to execute commands with SQLite
                dbCommand = new SQLiteCommand(sql, dbConnection);
                // Fill table (execute command)
                dbCommand.ExecuteNonQuery();

                // Create rssFeed table
                sql = @"CREATE TABLE rssFeed (
                          id int(11),
                          name varchar(45) DEFAULT NULL,
                          link varchar(200) DEFAULT NULL,
                          PRIMARY KEY (id)
                        )";

                // Construct command variable to execute commands with SQLite
                dbCommand = new SQLiteCommand(sql, dbConnection);
                // Create table (execute command)
                dbCommand.ExecuteNonQuery();

                // Create specialArrangements table 
                /**commented because might not be needed (replaced by yr 11 line)*
                sql = @"CREATE TABLE specarrangement (
                          id int(11),
                          date date DEFAULT NULL,
                          note varchar(255) DEFAULT NULL,
                          PRIMARY KEY (id)
                        )";
                */
                // Construct command variable to execute commands with SQLite
                //dbCommand = new SQLiteCommand(sql, dbConnection);
                // Create table (execute command)
                //dbCommand.ExecuteNonQuery();

                // Create classroomChanges table
                sql = @"CREATE TABLE classchange (
                          id int(11),
                          date date DEFAULT NULL,
                          teacher varchar(100) DEFAULT NULL,
                          line varchar(1) DEFAULT NULL,
                          period varchar(1) DEFAULT NULL,
                          room varchar(45) DEFAULT NULL,
                          PRIMARY KEY (id)
                        )";

                // Construct command variable to execute commands with SQLite
                dbCommand = new SQLiteCommand(sql, dbConnection);
                // Create table (execute command)
                dbCommand.ExecuteNonQuery();

                // Create meetingReminders table
                sql = @"CREATE TABLE meetingreminder (
                          id int(11),
                          date date DEFAULT NULL,
                          name varchar(500) DEFAULT NULL,
                          room varchar(45) DEFAULT NULL,
                          PRIMARY KEY (id)
                        )";

                // Construct command variable to execute commands with SQLite
                dbCommand = new SQLiteCommand(sql, dbConnection);
                // Create table (execute command)
                dbCommand.ExecuteNonQuery();

                // Create specialEvents table
                sql = @"CREATE TABLE specevents (
                          id int(11),
                          date date DEFAULT NULL,
                          event varchar(500) DEFAULT NULL,
                          PRIMARY KEY (id)
                        )";

                // Construct command variable to execute commands with SQLite
                dbCommand = new SQLiteCommand(sql, dbConnection);
                // Create table (execute command)
                dbCommand.ExecuteNonQuery();
            }
            else
            {
                // Database already created, simply connect (asumed default stuff already implemented within db
                dbConnection.Open();
            }

            /*
             * By this point it is known for the fact that the database is open and is working properly, this means that all new threads
             * can safely call database
             */

            // Database made,start server (own thread -- ensures no interuptions [does increase cpu load; but negligible considering size of program])
            Thread serverThread = new Thread(new ThreadStart(digitalSignageServer.startServer));
            serverThread.IsBackground = true;
            serverThread.Start();

            // Create window
            MainWindow mainWindow = new MainWindow();

            // Start program maximised & full screen
            mainWindow.WindowState = WindowState.Maximized;
            mainWindow.WindowStyle = WindowStyle.None;
            
            // Display window
            MainWindow.Show();
        }
    }
}
