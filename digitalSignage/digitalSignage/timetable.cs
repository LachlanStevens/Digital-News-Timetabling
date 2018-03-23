using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

// Sqlite usings
using System.Data.SQLite;
using System.IO;


using System.Diagnostics;


namespace digitalSignage
{
    public class timetable
    {
        timetable() { }
        public static string deHTMLise(string html) { return Regex.Replace(html, @"<[^>]*>|\r\n?|\n|\r", String.Empty); }
        public static void rssUpdateProcess()
        {
            // Connect to db, open to start taking queries
            SQLiteConnection dbConnection = new SQLiteConnection("Data Source=mainDatabase.db;Version=3;");
            string sql = "";
            SQLiteCommand dbCommand = new SQLiteCommand(sql, dbConnection);
            
            // by the time this code is executed it is known database exists.
            dbConnection.Open();

            // Query to select all rss feeds from db
            sql = "select * from rssFeed";
            dbCommand = new SQLiteCommand(sql, dbConnection);

            List<string> rssFeeds = new List<string>();

            // Read all rss links
            SQLiteDataReader reader = dbCommand.ExecuteReader();

            while (reader.Read())
                rssFeeds.Add((string)reader["link"]);

            //rssFeedList.Clear();
            // retrieve TOP 5 stories from each
            foreach (String currentFeed in rssFeeds)
            {
                var posts = new rssReader().ReadFeed(currentFeed).ToList();
                for (int i = 0; i < 5; i++)
                {
                    string title = posts.ElementAt(i).Title;
                    title = deHTMLise(title);
                    string desc = posts.ElementAt(i).Description;
                    desc = deHTMLise(desc);
                    string img = posts.ElementAt(i).Description;
                    img = Regex.Match(img, "<img.+?src=[\"'](.+?)[\"'].+?>", RegexOptions.IgnoreCase).Groups[1].Value;
                    //img =  Regex.Replace(img, @"(src=['\""].+?['\""])", String.Copy(tmpImg));
                    // Use ~ due to it being a character that is rarely (if ever) used. Making it a great Delimeter 
                    //rssFeedList.Add(title + "~" + desc + "~" + img);
                }
            }
        }
        public static void updateRss()
        {
            rssUpdateProcess();
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
                    runDate = runDate.AddSeconds(30);
                }
                else
                {

                    // execute function after task.delay has finished
                    // task.delay > thread.sleep due to the fact that task.delay doesnt block ui thread
                    // Enable no lack of response within wpf (ui).
                    Task.Delay(Convert.ToInt32(Math.Round(ts.TotalMilliseconds))).Wait();
                    rssUpdateProcess();
                    // Send updated list to separate method to display on screen
                    // Separate method as it can loop on its own
                }

            }
        }
    }
}
