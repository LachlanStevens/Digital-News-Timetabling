using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Timers;
using System.Data.SQLite;

namespace digitalSignage {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }
        private void Window_Initialized(object sender, EventArgs e) {
            // Thread to start rssListUpdating
            Thread rssThread = new Thread(new ThreadStart(rssUpdater.updateRss));
            rssThread.IsBackground = true;
            rssThread.Start();

            // Thread to start updating rss visuals (every 15s)
            Thread rssTimerThread = new Thread(new ThreadStart(rssTimer));
            rssTimerThread.IsBackground = true;
            rssTimerThread.Start();

            //Thread to update timetable
            Thread timetableThread = new Thread(new ThreadStart(timetableTimer));
            timetableThread.IsBackground = true;
            timetableThread.Start();

            // Thread to update Class changes, Meetings & Special Events
            Thread noticesThread = new Thread(new ThreadStart(noticesTimer));
            noticesThread.IsBackground = true;
            noticesThread.Start();
        }

        public void rssTimer()
        {
            List<string> rssFeedList = rssUpdater.rssFeedList;

            while (true)
            {
                if (!(rssFeedList.Count == 0)) { break; }
            }

            int i = 0;

            String[] splitString = rssFeedList[i].Split('~');
            String title = splitString[0];
            String desc = splitString[1];
            String img = splitString[2];

            newsTitle.Dispatcher.Invoke(
                new delegates.updateNewsHeadlineCallback(this.updateNewsHeadline),
                new object[] { title }
            );
            newsAbstract.Dispatcher.Invoke(
                new delegates.updateNewsAbstractCallback(this.updateNewsAbstract),
                new object[] { desc }
            );
            newsImage.Dispatcher.Invoke(
                new delegates.updateNewsImageCallback(this.updateNewsImage),
                new object[] { img }
            );


            DateTime runDate = DateTime.Parse("12:00 am");
            while (true)
            {
                DateTime rightNow = DateTime.Now;
                TimeSpan ts = runDate - rightNow;
                //Task.Delay(ts.Milliseconds).Wait();
                //testing();
                if (ts.TotalMilliseconds < 0)
                {
                    // Negative means time has past, add day to time of retrieval
                    runDate = runDate.AddSeconds(15);
                }
                else
                {
                    // execute function after task.delay has finished
                    // task.delay > thread.sleep due to the fact that task.delay doesnt block ui thread
                    // Enable no lack of response within wpf (ui).
                    Task.Delay(Convert.ToInt32(Math.Round(ts.TotalMilliseconds))).Wait();
                    if (i < rssFeedList.Count)
                    {
                        splitString = rssFeedList[i].Split('~');
                        title = splitString[0];
                        desc = splitString[1];
                        img = splitString[2];

                        newsTitle.Dispatcher.Invoke(
                            new delegates.updateNewsHeadlineCallback(this.updateNewsHeadline),
                            new object[] { title }
                        );
                        newsAbstract.Dispatcher.Invoke(
                            new delegates.updateNewsAbstractCallback(this.updateNewsAbstract),
                            new object[] { desc }
                        );
                        newsImage.Dispatcher.Invoke(
                            new delegates.updateNewsImageCallback(this.updateNewsImage),
                            new object[] { img }
                        );
                        i++;
                    }
                    else
                    {
                        i = 0;
                    }
                }
            }
        }

        public void timetableTimer()
        {
            DateTime runDate = DateTime.Parse("12:00 am");
            while (true)
            {
                DateTime rightNow = DateTime.Now;
                TimeSpan ts = runDate - rightNow;
                //Task.Delay(ts.Milliseconds).Wait();
                //testing();
                if (ts.TotalMilliseconds < 0)
                {
                    // Negative means time has past, add day to time of retrieval
                    // Loop once a minute to make sure correct time is found each day (sounds like a lot of loop, but code will only be executed on right time)
                    runDate = runDate.AddSeconds(1);
                }
                else
                {
                    /*
                     * Times to swap over
                     * 12:00AM - 9:50AM <-- Period 1
                     * 09:50AM - 10:33AM <-- Period 2
                     * 10:33AM - 11:36AM <-- Period 3
                     * 11:36AM - 12:49PM <-- Period 4
                     * 12:49PM - 02:17PM <-- Period 5
                     * 02:17PM - 11:59AM <-- Period 6
                     */
                    String yr12CurrentLine = "";
                    String yr12NextLine = "";
                    String yr11CurrentLine = "";
                    String yr11NextLine = "";

                    TimeSpan midNight = new TimeSpan(0,0,0);
                    TimeSpan firstPeriod = new TimeSpan(9,50,0);
                    TimeSpan secondPeriod = new TimeSpan(10,33,0);
                    TimeSpan thirdPeriod = new TimeSpan(11,36,0);
                    TimeSpan fourthPeriod = new TimeSpan(12,49,0);
                    TimeSpan fifthPeriod = new TimeSpan(14,17,0);
                    TimeSpan sixthPeriod = new TimeSpan(23,59,0);

                    TimeSpan now = DateTime.Now.TimeOfDay;

                    // Take day of year to week of year (odd or even)
                    int week = (DateTime.Now.DayOfYear / 7) % 2;
                    string day = DateTime.Now.DayOfWeek.ToString().Substring(0,3).ToLower();

                    if (day == "sat" || day == "sun"){
                        day = "fri";
                    }

                    Task.Delay(Convert.ToInt32(Math.Round(ts.TotalMilliseconds))).Wait();

                    SQLiteConnection dbConnection = new SQLiteConnection("Data Source=mainDatabase.db;Version=3;");
                    string sql = "";
                    SQLiteCommand dbCommand = new SQLiteCommand(sql, dbConnection);
                    

                    dbConnection.Open();
                    SQLiteDataReader reader = dbCommand.ExecuteReader();

                    if ((now > midNight) && (now < firstPeriod))
                    {
                        // Period 1
                        sql = String.Format(@"Select {0} from timetable where yr='{1}' AND period='{2}' AND wk='{3}'", day, 12, 1, (week + 1));
                        dbCommand = new SQLiteCommand(sql, dbConnection);
                        reader = dbCommand.ExecuteReader();

                        while (reader.Read())
                            yr12CurrentLine = "Line " + (string)reader[day];

                        sql = String.Format(@"Select {0} from timetable where yr='{1}' AND period='{2}' AND wk='{3}'", day, 12, 2, (week + 1));
                        dbCommand = new SQLiteCommand(sql, dbConnection);

                        reader = dbCommand.ExecuteReader();
                        while (reader.Read())
                            yr12NextLine = "Line " + (string)reader[day];

                        sql = String.Format(@"Select {0} from timetable where yr='{1}' AND period='{2}' AND wk='{3}'", day, 11, 1, (week + 1));
                        dbCommand = new SQLiteCommand(sql, dbConnection);
                        reader = dbCommand.ExecuteReader();

                        while (reader.Read())
                            yr11CurrentLine = "Line " + (string)reader[day];

                        sql = String.Format(@"Select {0} from timetable where yr='{1}' AND period='{2}' AND wk='{3}'", day, 11, 2, (week + 1));
                        dbCommand = new SQLiteCommand(sql, dbConnection);

                        reader = dbCommand.ExecuteReader();
                        while (reader.Read())
                            yr11NextLine = "Line " + (string)reader[day];

                        yr12FirstLine.Dispatcher.Invoke(
                            new delegates.updateYr12FirstLineCallback(this.updateYr12FirstLine),
                            new object[] { yr12CurrentLine }
                        );
                        yr12FirstLine.Dispatcher.Invoke(
                            new delegates.updateYr12FirstLineActiveCallback(this.updateYr12FirstActiveLine),
                            new object[] { 62.00 }
                        );
                        yr12SecondLine.Dispatcher.Invoke(
                            new delegates.updateYr12SecondLineCallback(this.updateYr12SecondLine),
                            new object[] { yr12NextLine }
                        );
                        yr12SecondLine.Dispatcher.Invoke(
                            new delegates.updateYr12SecondLineActiveCallback(this.updateYr12SecondActiveLine),
                            new object[] { 40.00 }
                        );

                        yr11FirstLine.Dispatcher.Invoke(
                            new delegates.updateYr11FirstLineCallback(this.updateYr11FirstLine),
                            new object[] { yr11CurrentLine }
                        );
                        yr11FirstLine.Dispatcher.Invoke(
                            new delegates.updateYr11FirstLineActiveCallback(this.updateYr11FirstActiveLine),
                            new object[] { 62.00 }
                        );
                        yr11SecondLine.Dispatcher.Invoke(
                            new delegates.updateYr11SecondLineCallback(this.updateYr11SecondLine),
                            new object[] { yr11NextLine }
                        );
                        yr11SecondLine.Dispatcher.Invoke(
                            new delegates.updateYr11SecondLineActiveCallback(this.updateYr11SecondActiveLine),
                            new object[] { 40.00 }
                        );                        
                    }
                    else if ((now > firstPeriod) && (now < secondPeriod))
                    {
                        // Period 2
                        sql = String.Format(@"Select {0} from timetable where yr='{1}' AND period='{2}' AND wk='{3}'", day, 12, 1, (week + 1));
                        dbCommand = new SQLiteCommand(sql, dbConnection);
                        reader = dbCommand.ExecuteReader();

                        while (reader.Read())
                            yr12CurrentLine = "Line " + (string)reader[day];

                        sql = String.Format(@"Select {0} from timetable where yr='{1}' AND period='{2}' AND wk='{3}'", day, 12, 2, (week + 1));
                        dbCommand = new SQLiteCommand(sql, dbConnection);

                        reader = dbCommand.ExecuteReader();
                        while (reader.Read())
                            yr12NextLine = "Line " + (string)reader[day];

                        sql = String.Format(@"Select {0} from timetable where yr='{1}' AND period='{2}' AND wk='{3}'", day, 11, 1, (week + 1));
                        dbCommand = new SQLiteCommand(sql, dbConnection);
                        reader = dbCommand.ExecuteReader();

                        while (reader.Read())
                            yr11CurrentLine = "Line " + (string)reader[day];

                        sql = String.Format(@"Select {0} from timetable where yr='{1}' AND period='{2}' AND wk='{3}'", day, 11, 2, (week + 1));
                        dbCommand = new SQLiteCommand(sql, dbConnection);

                        reader = dbCommand.ExecuteReader();
                        while (reader.Read())
                            yr11NextLine = "Line " + (string)reader[day];

                        yr12FirstLine.Dispatcher.Invoke(
                            new delegates.updateYr12FirstLineCallback(this.updateYr12FirstLine),
                            new object[] { yr12CurrentLine }
                        );
                        yr12FirstLine.Dispatcher.Invoke(
                            new delegates.updateYr12FirstLineActiveCallback(this.updateYr12FirstActiveLine),
                            new object[] { 40.00 }
                        );
                        yr12SecondLine.Dispatcher.Invoke(
                            new delegates.updateYr12SecondLineCallback(this.updateYr12SecondLine),
                            new object[] { yr12NextLine }
                        );
                        yr12SecondLine.Dispatcher.Invoke(
                            new delegates.updateYr12SecondLineActiveCallback(this.updateYr12SecondActiveLine),
                            new object[] { 62.00 }
                        );

                        yr11FirstLine.Dispatcher.Invoke(
                            new delegates.updateYr11FirstLineCallback(this.updateYr11FirstLine),
                            new object[] { yr11CurrentLine }
                        );
                        yr11FirstLine.Dispatcher.Invoke(
                            new delegates.updateYr11FirstLineActiveCallback(this.updateYr11FirstActiveLine),
                            new object[] { 40.00 }
                        );
                        yr11SecondLine.Dispatcher.Invoke(
                            new delegates.updateYr11SecondLineCallback(this.updateYr11SecondLine),
                            new object[] { yr11NextLine }
                        );
                        yr11SecondLine.Dispatcher.Invoke(
                            new delegates.updateYr11SecondLineActiveCallback(this.updateYr11SecondActiveLine),
                            new object[] { 62.00 }
                        );
                    }
                    else if ((now > secondPeriod) && (now < thirdPeriod))
                    {
                        // Period 3
                        sql = String.Format(@"Select {0} from timetable where yr='{1}' AND period='{2}' AND wk='{3}'", day, 12, 3, (week + 1));
                        dbCommand = new SQLiteCommand(sql, dbConnection);
                        reader = dbCommand.ExecuteReader();

                        while (reader.Read())
                            yr12CurrentLine = "Line " + (string)reader[day];

                        sql = String.Format(@"Select {0} from timetable where yr='{1}' AND period='{2}' AND wk='{3}'", day, 12, 4, (week + 1));
                        dbCommand = new SQLiteCommand(sql, dbConnection);

                        reader = dbCommand.ExecuteReader();
                        while (reader.Read())
                            yr12NextLine = "Line " + (string)reader[day];

                        sql = String.Format(@"Select {0} from timetable where yr='{1}' AND period='{2}' AND wk='{3}'", day, 11, 3, (week + 1));
                        dbCommand = new SQLiteCommand(sql, dbConnection);
                        reader = dbCommand.ExecuteReader();

                        while (reader.Read())
                            yr11CurrentLine = "Line " + (string)reader[day];

                        sql = String.Format(@"Select {0} from timetable where yr='{1}' AND period='{2}' AND wk='{3}'", day, 11, 4, (week + 1));
                        dbCommand = new SQLiteCommand(sql, dbConnection);

                        reader = dbCommand.ExecuteReader();
                        while (reader.Read())
                            yr11NextLine = "Line " + (string)reader[day];

                        yr12FirstLine.Dispatcher.Invoke(
                            new delegates.updateYr12FirstLineCallback(this.updateYr12FirstLine),
                            new object[] { yr12CurrentLine }
                        );
                        yr12FirstLine.Dispatcher.Invoke(
                            new delegates.updateYr12FirstLineActiveCallback(this.updateYr12FirstActiveLine),
                            new object[] { 62.00 }
                        );
                        yr12SecondLine.Dispatcher.Invoke(
                            new delegates.updateYr12SecondLineCallback(this.updateYr12SecondLine),
                            new object[] { yr12NextLine }
                        );
                        yr12SecondLine.Dispatcher.Invoke(
                            new delegates.updateYr12SecondLineActiveCallback(this.updateYr12SecondActiveLine),
                            new object[] { 40.00 }
                        );

                        yr11FirstLine.Dispatcher.Invoke(
                            new delegates.updateYr11FirstLineCallback(this.updateYr11FirstLine),
                            new object[] { yr11CurrentLine }
                        );
                        yr11FirstLine.Dispatcher.Invoke(
                            new delegates.updateYr11FirstLineActiveCallback(this.updateYr11FirstActiveLine),
                            new object[] { 62.00 }
                        );
                        yr11SecondLine.Dispatcher.Invoke(
                            new delegates.updateYr11SecondLineCallback(this.updateYr11SecondLine),
                            new object[] { yr11NextLine }
                        );
                        yr11SecondLine.Dispatcher.Invoke(
                            new delegates.updateYr11SecondLineActiveCallback(this.updateYr11SecondActiveLine),
                            new object[] { 40.00 }
                        );
                    }
                    else if ((now > thirdPeriod) && (now < fourthPeriod))
                    {
                        // Period 4
                        sql = String.Format(@"Select {0} from timetable where yr='{1}' AND period='{2}' AND wk='{3}'", day, 12, 3, (week + 1));
                        dbCommand = new SQLiteCommand(sql, dbConnection);
                        reader = dbCommand.ExecuteReader();

                        while (reader.Read())
                            yr12CurrentLine = "Line " + (string)reader[day];

                        sql = String.Format(@"Select {0} from timetable where yr='{1}' AND period='{2}' AND wk='{3}'", day, 12, 4, (week + 1));
                        dbCommand = new SQLiteCommand(sql, dbConnection);

                        reader = dbCommand.ExecuteReader();
                        while (reader.Read())
                            yr12NextLine = "Line " + (string)reader[day];

                        sql = String.Format(@"Select {0} from timetable where yr='{1}' AND period='{2}' AND wk='{3}'", day, 11, 3, (week + 1));
                        dbCommand = new SQLiteCommand(sql, dbConnection);
                        reader = dbCommand.ExecuteReader();

                        while (reader.Read())
                            yr11CurrentLine = "Line " + (string)reader[day];

                        sql = String.Format(@"Select {0} from timetable where yr='{1}' AND period='{2}' AND wk='{3}'", day, 11, 4, (week + 1));
                        dbCommand = new SQLiteCommand(sql, dbConnection);

                        reader = dbCommand.ExecuteReader();
                        while (reader.Read())
                            yr11NextLine = "Line " + (string)reader[day];

                        yr12FirstLine.Dispatcher.Invoke(
                            new delegates.updateYr12FirstLineCallback(this.updateYr12FirstLine),
                            new object[] { yr12CurrentLine }
                        );
                        yr12FirstLine.Dispatcher.Invoke(
                            new delegates.updateYr12FirstLineActiveCallback(this.updateYr12FirstActiveLine),
                            new object[] { 40.00 }
                        );
                        yr12SecondLine.Dispatcher.Invoke(
                            new delegates.updateYr12SecondLineCallback(this.updateYr12SecondLine),
                            new object[] { yr12NextLine }
                        );
                        yr12SecondLine.Dispatcher.Invoke(
                            new delegates.updateYr12SecondLineActiveCallback(this.updateYr12SecondActiveLine),
                            new object[] { 62.00 }
                        );

                        yr11FirstLine.Dispatcher.Invoke(
                            new delegates.updateYr11FirstLineCallback(this.updateYr11FirstLine),
                            new object[] { yr11CurrentLine }
                        );
                        yr11FirstLine.Dispatcher.Invoke(
                            new delegates.updateYr11FirstLineActiveCallback(this.updateYr11FirstActiveLine),
                            new object[] { 40.00 }
                        );
                        yr11SecondLine.Dispatcher.Invoke(
                            new delegates.updateYr11SecondLineCallback(this.updateYr11SecondLine),
                            new object[] { yr11NextLine }
                        );
                        yr11SecondLine.Dispatcher.Invoke(
                            new delegates.updateYr11SecondLineActiveCallback(this.updateYr11SecondActiveLine),
                            new object[] { 62.00 }
                        );
                    }
                    else if ((now > fourthPeriod) && (now < fifthPeriod))
                    {
                        // Period 5
                        sql = String.Format(@"Select {0} from timetable where yr='{1}' AND period='{2}' AND wk='{3}'", day, 12, 5, (week + 1));
                        dbCommand = new SQLiteCommand(sql, dbConnection);
                        reader = dbCommand.ExecuteReader();

                        while (reader.Read())
                            yr12CurrentLine = "Line " + (string)reader[day];

                        sql = String.Format(@"Select {0} from timetable where yr='{1}' AND period='{2}' AND wk='{3}'", day, 12, 6, (week + 1));
                        dbCommand = new SQLiteCommand(sql, dbConnection);

                        reader = dbCommand.ExecuteReader();
                        while (reader.Read())
                            yr12NextLine = "Line " + (string)reader[day];

                        sql = String.Format(@"Select {0} from timetable where yr='{1}' AND period='{2}' AND wk='{3}'", day, 11, 5, (week + 1));
                        dbCommand = new SQLiteCommand(sql, dbConnection);
                        reader = dbCommand.ExecuteReader();

                        while (reader.Read())
                            yr11CurrentLine = "Line " + (string)reader[day];

                        sql = String.Format(@"Select {0} from timetable where yr='{1}' AND period='{2}' AND wk='{3}'", day, 11, 6, (week + 1));
                        dbCommand = new SQLiteCommand(sql, dbConnection);

                        reader = dbCommand.ExecuteReader();
                        while (reader.Read())
                            yr11NextLine = "Line " + (string)reader[day];

                        yr12FirstLine.Dispatcher.Invoke(
                            new delegates.updateYr12FirstLineCallback(this.updateYr12FirstLine),
                            new object[] { yr12CurrentLine }
                        );
                        yr12FirstLine.Dispatcher.Invoke(
                            new delegates.updateYr12FirstLineActiveCallback(this.updateYr12FirstActiveLine),
                            new object[] { 62.00 }
                        );
                        yr12SecondLine.Dispatcher.Invoke(
                            new delegates.updateYr12SecondLineCallback(this.updateYr12SecondLine),
                            new object[] { yr12NextLine }
                        );
                        yr12SecondLine.Dispatcher.Invoke(
                            new delegates.updateYr12SecondLineActiveCallback(this.updateYr12SecondActiveLine),
                            new object[] { 40.00 }
                        );

                        yr11FirstLine.Dispatcher.Invoke(
                            new delegates.updateYr11FirstLineCallback(this.updateYr11FirstLine),
                            new object[] { yr11CurrentLine }
                        );
                        yr11FirstLine.Dispatcher.Invoke(
                            new delegates.updateYr11FirstLineActiveCallback(this.updateYr11FirstActiveLine),
                            new object[] { 62.00 }
                        );
                        yr11SecondLine.Dispatcher.Invoke(
                            new delegates.updateYr11SecondLineCallback(this.updateYr11SecondLine),
                            new object[] { yr11NextLine }
                        );
                        yr11SecondLine.Dispatcher.Invoke(
                            new delegates.updateYr11SecondLineActiveCallback(this.updateYr11SecondActiveLine),
                            new object[] { 40.00 }
                        );
                    }
                    else if ((now > fifthPeriod) && (now < sixthPeriod))
                    {
                        // Period 6
                        sql = String.Format(@"Select {0} from timetable where yr='{1}' AND period='{2}' AND wk='{3}'", day, 12, 5, (week + 1));
                        dbCommand = new SQLiteCommand(sql, dbConnection);
                        reader = dbCommand.ExecuteReader();

                        while (reader.Read())
                            yr12CurrentLine = "Line " + (string)reader[day];

                        sql = String.Format(@"Select {0} from timetable where yr='{1}' AND period='{2}' AND wk='{3}'", day, 12, 6, (week + 1));
                        dbCommand = new SQLiteCommand(sql, dbConnection);

                        reader = dbCommand.ExecuteReader();
                        while (reader.Read())
                            yr12NextLine = "Line " + (string)reader[day];

                        sql = String.Format(@"Select {0} from timetable where yr='{1}' AND period='{2}' AND wk='{3}'", day, 11, 5, (week + 1));
                        dbCommand = new SQLiteCommand(sql, dbConnection);
                        reader = dbCommand.ExecuteReader();

                        while (reader.Read())
                            yr11CurrentLine = "Line " + (string)reader[day];

                        sql = String.Format(@"Select {0} from timetable where yr='{1}' AND period='{2}' AND wk='{3}'", day, 11, 6, (week + 1));
                        dbCommand = new SQLiteCommand(sql, dbConnection);

                        reader = dbCommand.ExecuteReader();
                        while (reader.Read())
                            yr11NextLine = "Line " + (string)reader[day];

                        yr12FirstLine.Dispatcher.Invoke(
                            new delegates.updateYr12FirstLineCallback(this.updateYr12FirstLine),
                            new object[] { yr12CurrentLine }
                        );
                        yr12FirstLine.Dispatcher.Invoke(
                            new delegates.updateYr12FirstLineActiveCallback(this.updateYr12FirstActiveLine),
                            new object[] { 40.00 }
                        );
                        yr12SecondLine.Dispatcher.Invoke(
                            new delegates.updateYr12SecondLineCallback(this.updateYr12SecondLine),
                            new object[] { yr12NextLine }
                        );
                        yr12SecondLine.Dispatcher.Invoke(
                            new delegates.updateYr12SecondLineActiveCallback(this.updateYr12SecondActiveLine),
                            new object[] { 62.00 }
                        );

                        yr11FirstLine.Dispatcher.Invoke(
                            new delegates.updateYr11FirstLineCallback(this.updateYr11FirstLine),
                            new object[] { yr11CurrentLine }
                        );
                        yr11FirstLine.Dispatcher.Invoke(
                            new delegates.updateYr11FirstLineActiveCallback(this.updateYr11FirstActiveLine),
                            new object[] { 40.00 }
                        );
                        yr11SecondLine.Dispatcher.Invoke(
                            new delegates.updateYr11SecondLineCallback(this.updateYr11SecondLine),
                            new object[] { yr11NextLine }
                        );
                        yr11SecondLine.Dispatcher.Invoke(
                            new delegates.updateYr11SecondLineActiveCallback(this.updateYr11SecondActiveLine),
                            new object[] { 62.00 }
                        );
                    }
                    else
                    {
                        // Do nothing
                    }
                    
                }
            }
        }
        public void noticesTimer()
        {
            String classChange = "";
            String meetingChange = "";
            String specEventsChange = "";

            DateTime runDate = DateTime.Parse("12:00 am");
            while (true)
            {
                DateTime rightNow = DateTime.Now;
                TimeSpan ts = runDate - rightNow;
                //Task.Delay(ts.Milliseconds).Wait();
                //testing();
                if (ts.TotalMilliseconds < 0)
                {
                    // Negative means time has past, add day to time of retrieval
                    // Loop once a minute to make sure correct time is found each day (sounds like a lot of loop, but code will only be executed on right time)
                    runDate = runDate.AddSeconds(1);
                }
                else
                {
                    Task.Delay(Convert.ToInt32(Math.Round(ts.TotalMilliseconds))).Wait();

                    SQLiteConnection dbConnection = new SQLiteConnection("Data Source=mainDatabase.db;Version=3;");
                    string sql = "";
                    SQLiteCommand dbCommand = new SQLiteCommand(sql, dbConnection);
                    

                    dbConnection.Open();
                    SQLiteDataReader reader = dbCommand.ExecuteReader();

                    // Find class changes first 
                    sql = String.Format(@"Select teacher from classchange where date='{0}'", DateTime.Now.Date);
                    dbCommand = new SQLiteCommand(sql, dbConnection);
                    reader = dbCommand.ExecuteReader();

                    while (reader.Read())
                        classChange = (string)reader["teacher"];
                    if (classChange == "")
                        classChange = "No classroom changes today";
                    // Find meeting changes
                    sql = String.Format(@"Select name from meetingreminder where date='{0}'", DateTime.Now.Date);
                    dbCommand = new SQLiteCommand(sql, dbConnection);
                    reader = dbCommand.ExecuteReader();

                    while (reader.Read())
                        meetingChange = (string)reader["name"];

                    if (meetingChange == "")
                        meetingChange = "Nothing planned for today";
                    // Find spec events
                    sql = String.Format(@"Select event from specevents where date='{0}'", DateTime.Now.Date);
                    dbCommand = new SQLiteCommand(sql, dbConnection);
                    reader = dbCommand.ExecuteReader();

                    while (reader.Read())
                        specEventsChange = (string)reader["event"];
                    if (specEventsChange == "")
                        specEventsChange = "Nothing planned for today";
                    
                    classChangesBox.Dispatcher.Invoke(
                        new delegates.updateClassChangesCallback(this.updateClassChanges),
                        new object[] { classChange }
                    );
                    meetingChangesBox.Dispatcher.Invoke(
                        new delegates.updateMeetingChangesCallback(this.updateMeetingChanges),
                        new object[] { meetingChange }
                    );
                    specEventsBox.Dispatcher.Invoke(
                        new delegates.updateSpecEventsCallback(this.updateSpecEvents),
                        new object[] { specEventsChange }
                    );
                }
            }
        }
        // Rss delegates
        private void updateNewsHeadline(string stringNewsHeadline) { newsTitle.Text = stringNewsHeadline; }
        private void updateNewsAbstract(string stringNewsAbstract) { newsAbstract.Text = stringNewsAbstract; }
        private void updateNewsImage(string stringNewsImage) {
            try { newsImage.Source = new BitmapImage(new Uri(stringNewsImage, UriKind.Absolute));
            } catch (System.UriFormatException e) {
                // No Image found in feed display nothing ... this will be until bing integration can be found
                newsImage.Source = null;
            }
        }
        // Timetable delegates
        private void updateYr11FirstLine(string stringYr11firstline) { yr11FirstLine.Content = stringYr11firstline; }
        private void updateYr11SecondLine(string stringYr11secondline) { yr11SecondLine.Content = stringYr11secondline; }
        private void updateYr12FirstLine(string stringYr12firstline) { yr12FirstLine.Content = stringYr12firstline; }
        private void updateYr12SecondLine(string stringYr12secondline){ yr12SecondLine.Content = stringYr12secondline; }
        private void updateYr11FirstActiveLine(double doubleYr11size) { yr11FirstLine.FontSize = doubleYr11size; }
        private void updateYr11SecondActiveLine(double doubleYr11size) { yr11SecondLine.FontSize = doubleYr11size; }
        private void updateYr12FirstActiveLine(double doubleYr12size) { yr12FirstLine.FontSize = doubleYr12size; }
        private void updateYr12SecondActiveLine(double doubleYr12size){ yr12SecondLine.FontSize = doubleYr12size; }
        // Notice delegates
        private void updateClassChanges(string stringClassChange) { classChangesBox.Text = stringClassChange; }
        private void updateMeetingChanges(string stringMeetingChange) { meetingChangesBox.Text = stringMeetingChange; }
        private void updateSpecEvents(string stringSpecEvents) { specEventsBox.Text = stringSpecEvents; }
    }
}
