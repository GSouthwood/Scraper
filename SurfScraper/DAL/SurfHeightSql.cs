using System.Data.SqlClient;
using System.Collections.Generic;
using SurfScraper.Model;
using System;
using SurfScraper.UtilityMethods;
using SurfScraper.ScrapeMethods;


namespace TestScraper
{
    public class SurfHeightSql
    {
        
        //log file path in app.config
        //add api functionality -- 

        private string connectionString;
        private const string SQL_LoadSpots = "SELECT spot_name, spot_id, location_id FROM Spot";
        private const string SQL_WriteSurfInfo = "  INSERT INTO Surf (swell_height_feet, log_date, location_id, spot_name, forecast_for_date, spot_id, wind_direction)" +
            " VALUES (@swellHeight, @logDate, @locationId, @spotName, @forecastForDate, @spotId, @windDirection);";

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
            catch (SqlException e)
            {
                Email.SendEmailFailure(e.ToString(), "Failed during: public List<SurfUrl> LoadSpotsToScrape()");
                throw;
            }

            return spots;

        }

        
        //public void LogSurfData()
        //{
        //    //these counters all work together to allow for effective scraping 
        //    //total log count
        //    int totalLogCount = 0;

        //    //total log spot start count
        //    int totalLogPerSpotStartCount = 8;

        //    //spot log count
        //    int totalLogPerSpotCount = 0;

        //    //day count
        //    int dayCount = 0;

        //    //time count
        //    int timeCount = 0;


        //    //load list of date times
        //    List<TimeSpan> times = Utility.LoadTimes();

        //    //load list of spots
        //    SurfHeightSql surfSql = new SurfHeightSql(connectionString);
        //    List<SurfUrl> spots = surfSql.LoadSpotsToScrape();


        //    //load list of surf heights
        //    Surf surf = new Surf();
        //    List<decimal> surfHeight = surf.ScrapeSurfHeight();

        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(connectionString))
        //        {
        //            conn.Open();
        //            foreach (var item in spots)
        //            {

        //                //there are 79 nodes of data to scrape per page
        //                //(10 days and 8 forecast nodes per day) except
        //                //for last day which has 7 nodes
        //                while (totalLogPerSpotCount < 79)
        //                {
        //                    //this will prevent the scraping from breaking by trying to 
        //                    //scrape the "extra" 8th node on the 10th forecast day
        //                    //instead it will only scrape 7 
        //                    if (totalLogPerSpotCount == 72)
        //                    {
        //                        timeCount = 0;
        //                        for (int i = totalLogCount; i < totalLogPerSpotStartCount - 1; i++)
        //                        {

                                    
        //                            SqlCommand cmd = new SqlCommand(SQL_WriteSurfInfo, conn);                                    
        //                            cmd.Parameters.AddWithValue("@waveHeight", surfHeight[i]);
        //                            cmd.Parameters.AddWithValue("@logDate", Utility.CurrentDateTime());
        //                            cmd.Parameters.AddWithValue("@locationId", item.LocationId);
        //                            cmd.Parameters.AddWithValue("@spotName", item.SpotName);
        //                            cmd.Parameters.AddWithValue("@forecastForDate", Utility.CurrentDateTime().AddDays(dayCount));
        //                            cmd.Parameters.AddWithValue("@ForecastForTime", times[timeCount]);
        //                            cmd.Parameters.AddWithValue("@spotId", item.SpotId);
        //                            cmd.ExecuteNonQuery();
        //                            totalLogPerSpotCount++;
        //                            timeCount++;
        //                        }
                                
        //                    }
        //                    //this is the same as above but for forecast days 1-9 which have all 8 nodes
        //                    else
        //                    {
        //                        timeCount = 0;
        //                        for (int i = totalLogCount; i < totalLogPerSpotStartCount; i++)
        //                        {


        //                            SqlCommand cmd = new SqlCommand(SQL_WriteSurfInfo, conn);                                   
        //                            cmd.Parameters.AddWithValue("@waveHeight", surfHeight[i]);
        //                            cmd.Parameters.AddWithValue("@logDate", Utility.CurrentDateTime());
        //                            cmd.Parameters.AddWithValue("@locationId", item.LocationId);
        //                            cmd.Parameters.AddWithValue("@spotName", item.SpotName);
        //                            cmd.Parameters.AddWithValue("@forecastForDate", Utility.CurrentDateTime().AddDays(dayCount));
        //                            cmd.Parameters.AddWithValue("@ForecastForTime", times[timeCount].ToString());
        //                            cmd.Parameters.AddWithValue("@spotId", item.SpotId);
        //                            cmd.ExecuteNonQuery();

        //                            totalLogPerSpotCount++;
        //                            timeCount++;
        //                        }
                                

        //                    }
        //                    dayCount++;
        //                    totalLogCount += 7;
        //                    totalLogPerSpotStartCount += 7;
        //                }

        //                totalLogPerSpotCount = 0;
        //                dayCount = 0;
        //            }
        //        }
        //    }
        //    catch (SqlException ex)
        //    {

        //        throw;
        //    }
        //}
        public void LogSurfData()
        {
            //these counters all work together to allow for effective scraping 
            //total log count
            int totalLogCount = 0;

            //total log spot start count
            int totalLogPerSpotStartCount = 7;

            //day count
            int dayCount = 0;

            //failure count for logging
            int failureCount = 0;

            //variable to hold the average surf height per day for each spot
            decimal averageSurfHeight = 0;

            //variable to hold the average wind direction per day for each spot
            decimal averageWindDirection = 0;

            //load list of date times
            List<TimeSpan> times = DateAndTime.LoadTimes();

            //load list of spots
            SurfHeightSql surfSql = new SurfHeightSql(connectionString);
            List<SurfUrl> spots = surfSql.LoadSpotsToScrape();


            //load list of surf heights
            Surf surf = new Surf();
            List<decimal> surfHeight = surf.ScrapeSurfHeight(connectionString);
           // List<int> windDirection = surf.ScrapeWindDirection(connectionString);

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    foreach (var item in spots)
                    {

                        //there are 79 nodes of data to scrape per page
                        //(10 days and 8 forecast nodes per day) except
                        //for last day which has 7 nodes
                        while (dayCount < 10)
                        {
                            //this will prevent the scraping from breaking by trying to 
                            //scrape the "extra" 8th node on the 10th forecast day
                            //instead it will only scrape 7 
                            if (dayCount == 9)
                            {
                                
                                for (int i = totalLogCount; i <= totalLogPerSpotStartCount - 1; i++)
                                {
                                    averageSurfHeight += surfHeight[i];
                                }
                                //average for the last day with only 7 nodes of forecast data
                                    averageSurfHeight /= 7;

                                //for (int i = totalLogCount; i <= totalLogPerSpotStartCount - 1; i++)
                                //{
                                //    averageWindDirection += windDirection[i];
                                //}
                                ////average for the last day with only 7 nodes of forecast data
                                //averageWindDirection /= 7;

                                try
                                {

                                    SqlCommand cmd = new SqlCommand(SQL_WriteSurfInfo, conn);
                                    cmd.Parameters.AddWithValue("@swellHeight", averageSurfHeight);
                                    cmd.Parameters.AddWithValue("@logDate", DateAndTime.CurrentDateTime());
                                    cmd.Parameters.AddWithValue("@locationId", item.LocationId);
                                    cmd.Parameters.AddWithValue("@spotName", item.SpotName);
                                    cmd.Parameters.AddWithValue("@forecastForDate", DateAndTime.CurrentDateTime().AddDays(dayCount));
                                    cmd.Parameters.AddWithValue("@spotId", item.SpotId);
                                    cmd.Parameters.AddWithValue("@windDirection", Math.Round(averageWindDirection, 0));
                                    cmd.ExecuteNonQuery();

                                }
                                catch (Exception e)
                                {
                                    Email.SendEmailFailure(e.ToString(), $"Surf data failure at {item} for day {dayCount}");
                                    failureCount++;
                                }


                                    averageSurfHeight = 0;
                                    averageWindDirection = 0;
                                    Console.WriteLine("Inserted 1 row");

                                    totalLogCount += 7;
                                    totalLogPerSpotStartCount += 7;
                                

                            }
                            //this is the same as above but for forecast days 1-9 which have all 8 nodes
                            else
                            {
                                for (int i = totalLogCount; i <= totalLogPerSpotStartCount; i++)
                                {
                                    averageSurfHeight += surfHeight[i];
                                    
                                }
                                //average for first 9 days with only 8 nodes of forecast data
                                averageSurfHeight /= 8;

                                //for (int i = totalLogCount; i <= totalLogPerSpotStartCount; i++)
                                //{
                                //    averageWindDirection += windDirection[i];
                                //}
                                ////average for the last day with only 8 nodes of forecast data
                                //averageWindDirection /= 8;

                                try
                                {



                                    SqlCommand cmd = new SqlCommand(SQL_WriteSurfInfo, conn);
                                    cmd.Parameters.AddWithValue("@swellHeight", averageSurfHeight);
                                    cmd.Parameters.AddWithValue("@logDate", DateAndTime.CurrentDateTime());
                                    cmd.Parameters.AddWithValue("@locationId", item.LocationId);
                                    cmd.Parameters.AddWithValue("@spotName", item.SpotName);
                                    cmd.Parameters.AddWithValue("@forecastForDate", DateAndTime.CurrentDateTime().AddDays(dayCount));
                                    cmd.Parameters.AddWithValue("@spotId", item.SpotId);
                                    cmd.Parameters.AddWithValue("@windDirection", Math.Round(averageWindDirection, 0));
                                    cmd.ExecuteNonQuery();
                                    
                                }
                                catch (Exception e)
                                {
                                    Email.SendEmailFailure(e.ToString(), $"Surf data failure at {item} for day {dayCount}");
                                    failureCount++;
                                }
                                averageSurfHeight = 0;
                                averageWindDirection = 0;
                                Console.WriteLine("Inserted 1 row");
                                totalLogCount += 8;
                                totalLogPerSpotStartCount += 8;

                            }
                            dayCount++;
                            

                        }
                        dayCount = 0;
                    }
                   
                }
            }
            catch (SqlException ex)
            {

                throw;
            }
        }


    }
}
