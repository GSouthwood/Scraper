﻿using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Data.SqlClient;
using System.Collections.Generic;
using SurfScraper.Model;
using System;
using System.Linq;


namespace TestScraper
{
    public class SurfHeightSql
    {
        private string connectionString;
        private const string SQL_LoadSpots = "SELECT spot_name, spot_id, location_id FROM Spot";
        private const string SQL_WriteSurfInfo = "  INSERT INTO Surf (wave_height_feet, log_date, location_id, spot_name, forecast_for_date, forecast_for_time, spot_id)" +
            " VALUES (@waveHeight, @logDate, @locationId, @spotName, @forecastForDate, @ForecastForTime, @spotId);";

        public SurfHeightSql(string databaseconnectionString)
        {
            connectionString = databaseconnectionString;

        }
        //Take in all of the destination data from Scraper and load into a list of skyscanner URL's to scrape
        //any number of destinations can be loaded from database without having to change this method
        //which means the method for scraping data will never have to change, only the database
        public List<SurfUrl> LoadSpotsToScrape()
        {
            List<SurfUrl> spots = new List<SurfUrl>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();


                    SqlCommand cmd = new SqlCommand(SQL_LoadSpots, conn);

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        SurfUrl s = new SurfUrl();
                        s.SeaweedDomainName = "https://magicseaweed.com/";
                        s.SpotId = Convert.ToInt32(reader["spot_id"]);
                        s.SpotName = Convert.ToString(reader["spot_name"]);
                        s.LocationId = Convert.ToInt32(reader["location_id"]);

                        spots.Add(s);
                    }

                }
            }
            catch (SqlException ex)
            {

                throw;
            }

            return spots;

        }
        //Scrape data from magic seaweed for surf height for 10 day forecast
        public List<decimal> ScrapeSurfHeight()
        {
            //HtmlWeb web = new HtmlWeb();
            //HtmlAgilityPack.HtmlDocument doc = web.Load("http://www.surfline.com/surf-forecasts/indonesia/bali_2169/");
            //List<HtmlNode> surfSize = doc.DocumentNode.SelectNodes("//*[@id='observed_component']/div[3]/div[1]/div/div/div/div[4]/div/div/h1").ToList();
            ////*[@id="msw-js-fc"]/div[2]/div/table/tbody/tr[2]/td[3]/span

            List<string> weather = LogWeatherData();
            List<SurfUrl> spots = LoadSpotsToScrape();
            int counter = 0;
            HtmlWeb web = new HtmlWeb();
            List<HtmlNode> surfSize = new List<HtmlNode>();
            List<decimal> surf = new List<decimal>();
            foreach (var url in spots)
            {




                //if .Contains("s") accounts for when the page actually misreads the td element.
                HtmlAgilityPack.HtmlDocument doc = web.Load(url.ToString());
                try
                {
                    surfSize = doc.DocumentNode.SelectNodes("//*[@id='msw-js-fc']/div[2]/div/table/tbody/tr/td[4]/span").ToList();
                    if (surfSize[0].InnerText.Contains("s"))
                    {
                        surfSize = doc.DocumentNode.SelectNodes("//*[@id='msw-js-fc']/div[2]/div/table/tbody/tr/td[3]/span").ToList();
                    }
                }

                catch (Exception ex)
                {
                    throw;
                }


                //Gotta finish figuring out how to store the scraped data to then be put into the right data
                //in Scraper
                //create a DateTime method that gets the date for each set of 8 entries and the times associated with each entry
                //make sure the last entry only has 7 and keep track of everything with counters (may be a little complicated)
                foreach (var item in surfSize)
                {
                    counter++;
                    surf.Add(Decimal.Parse(item.InnerText.Substring(0, item.InnerText.Length - 3)));


                }
                if (surf.Count == 79 * spots.Count)
                {
                    Console.WriteLine("Complete surf data.");
                }
                else
                {
                    Console.WriteLine($"{79 * spots.Count - counter} data nodes to go...");
                }

            }

            return surf;
        }
        //Method for getting weather data from magicseaweed 
        //buggy
        public List<string> LogWeatherData()
        {

            HtmlWeb web = new HtmlWeb();
            List<HtmlNode> weatherNodes = new List<HtmlNode>();
            List<string> weather = new List<string>();
            HtmlAgilityPack.HtmlDocument doc = web.Load("https://magicseaweed.com/Uluwatu-Surf-Report/565/");
            try
            {
                weatherNodes = doc.DocumentNode.SelectNodes("//*[@id='msw-js-fc']/div[2]/div/table/tbody/tr/td[12]/span").ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
            foreach (var item in weatherNodes)
            {
                weather.Add(item.InnerText);
            }


            return weather;


        }
        public void LogSurfData()
        {
            //total log count
            int counter1 = 0;
            //total log spot start count
            int counter5 = 8;
            //spot log count
            int counter2 = 0;
            //day count
            int counter3 = 0;
            //time count
            int counter4 = 0;
            //load list of date times
            List<TimeSpan> times = LoadTimes();
            SurfHeightSql surfSql = new SurfHeightSql(connectionString);
            List<SurfUrl> spots = surfSql.LoadSpotsToScrape();

            List<decimal> surf = ScrapeSurfHeight();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    foreach (var item in spots)
                    {


                        while (counter2 < 79)
                        {

                            if (counter2 == 72)
                            {
                                counter4 = 0;
                                for (int i = counter1; i < counter5 - 1; i++)
                                {

                                    
                                    SqlCommand cmd = new SqlCommand(SQL_WriteSurfInfo, conn);
                                    //@waveHeight, @logDate, @locationId, @spotName, @forecastForDate, @ForecastForTime, @spotId
                                    cmd.Parameters.AddWithValue("@waveHeight", surf[i]);
                                    cmd.Parameters.AddWithValue("@logDate", CurrentDate());
                                    cmd.Parameters.AddWithValue("@locationId", item.LocationId);
                                    cmd.Parameters.AddWithValue("@spotName", item.SpotName);
                                    cmd.Parameters.AddWithValue("@forecastForDate", CurrentDate().AddDays(counter3));
                                    cmd.Parameters.AddWithValue("@ForecastForTime", times[counter4]);
                                    cmd.Parameters.AddWithValue("@spotId", item.SpotId);
                                    cmd.ExecuteNonQuery();
                                    counter2++;
                                    counter4++;
                                }
                                
                            }
                            else
                            {
                                counter4 = 0;
                                for (int i = counter1; i < counter5; i++)
                                {


                                    SqlCommand cmd = new SqlCommand(SQL_WriteSurfInfo, conn);
                                    //@waveHeight, @logDate, @locationId, @spotName, @forecastForDate, @ForecastForTime, @spotId
                                    cmd.Parameters.AddWithValue("@waveHeight", surf[i]);
                                    cmd.Parameters.AddWithValue("@logDate", CurrentDate());
                                    cmd.Parameters.AddWithValue("@locationId", item.LocationId);
                                    cmd.Parameters.AddWithValue("@spotName", item.SpotName);
                                    cmd.Parameters.AddWithValue("@forecastForDate", CurrentDate().AddDays(counter3));
                                    cmd.Parameters.AddWithValue("@ForecastForTime", times[counter4].ToString());
                                    cmd.Parameters.AddWithValue("@spotId", item.SpotId);
                                    cmd.ExecuteNonQuery();

                                    counter2++;
                                    counter4++;
                                }
                                

                            }
                            counter3++;
                            counter1 += 7;
                            counter5 += 7;
                        }

                        counter2 = 0;
                        counter3 = 0;
                    }
                }
            }
            catch (SqlException ex)
            {

                throw;
            }
        }
        public static List<TimeSpan> LoadTimes()
        {
            List<TimeSpan> times = new List<TimeSpan>();
            for (int i = 0; i <= 21; i += 3)
            {
                times.Add(new TimeSpan(i, 0, 0));
            }
            return times;
        }
        public static DateTime CurrentDate()
        {
            return DateTime.Now;
        }


    }
}
