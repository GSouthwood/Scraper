using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Data.SqlClient;
using System.Collections.Generic;
using SurfScraper.Model;
using System;



namespace TestScraper
{
    public class FlightsSql
    {
        //SQL command for inseting flight info into Flight table 
        private const string SQL_WriteFlightInfo = "INSERT INTO Flight (price, origin_airport_code, departure_date, return_date, airport_code, log_date) " +
            "VALUES (@price, @originCode, @departureDate, @returnDate, @destinationCode, @logTime);";
        private const string SQL_LoadDestinations = "SELECT airport_code, name, location_id FROM Destination";
        private string connectionString;
        
        
        


        public FlightsSql(string databaseconnectionString)
        {
            connectionString = databaseconnectionString;
        }

        //Take in all of the destination data from Scraper and load into a list of skyscanner URL's to scrape
        //any number of destinations can be loaded from database without having to change this method
        //which means the method for scraping data will never have to change, only the database
        public List<FlightUrl> LoadDestinationsToScrape()
        {
            List<FlightUrl> destinations = new List<FlightUrl>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    

                    SqlCommand cmd = new SqlCommand(SQL_LoadDestinations, conn);

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        FlightUrl f = new FlightUrl();
                        f.DestinationAirportCode = Convert.ToString(reader["airport_code"]);
                        f.LocationName = Convert.ToString(reader["name"]);
                        f.LocationId = Convert.ToInt32(reader["location_id"]);
                        f.SkyScannerDomainName = "https://www.skyscanner.com/transport/flights/lax/";
                        f.DepartureDate = GetDepartureDate().ToString("/yyMMdd/");
                        f.ReturnDate = GetReturnDate().ToString("yyMMdd");
                        f.SkyScannerVariables = "?adults=2&children=0&adultsv2=2&childrenv2=&infants=0&" +
                            "cabinclass=economy&rtn=1&preferdirects=false&" +
                            "outboundaltsenabled=false&inboundaltsenabled=false&ref=home#results";

                        destinations.Add(f);
                        
                    }

                }
            }
            catch (SqlException ex)
            {

                throw;
            }

            return destinations;

        }

        //scrape prices from SkyScanner
        public List<decimal> ScrapePrice()
        {
            FlightsSql flightsSql = new FlightsSql(connectionString);
            List<FlightUrl> destinations = flightsSql.LoadDestinationsToScrape();

            using (WebBrowser wb = new WebBrowser())
            {
                string text = "";
                decimal price = 0;
                bool isWorking = false;
                int count = 0;
                List<decimal> prices = new List<decimal>();
                
                foreach (var url in destinations)
                {
                    isWorking = false;
                    Console.WriteLine($"Trying {url.DestinationAirportCode}...");
                    while (!isWorking)
                    {
                        wb.ScriptErrorsSuppressed = true;
                        wb.Navigate($"{url}");
                        
                        while (wb.ReadyState != WebBrowserReadyState.Complete)
                        {
                            Application.DoEvents();
                            Thread.Sleep(20000);
                            Application.DoEvents();

                        }
                        if (wb.Document.GetElementsByTagName("tbody").Count > 0)
                        {
                            isWorking = true;
                        }
                    }


                    text = wb.Document.GetElementsByTagName("tbody")[0].InnerText.Substring(5, 7);
                    if (text.Contains(" "))
                    {
                        int limit = text.IndexOf(" ");
                        text = wb.Document.GetElementsByTagName("tbody")[0].InnerText.Substring(5, limit);
                        price = Decimal.Parse(text);
                    }
                    
                    prices.Add(price);
                    count++;
                    Console.WriteLine($"{count}/{destinations.Count} Done.");

                }
                return prices;
            }
        }
        // Insert flights rows into Scraper
        public void WritePrices()
        {
            FlightsSql flightsSql = new FlightsSql(connectionString);
            List<FlightUrl> destinations = flightsSql.LoadDestinationsToScrape();
            
            List<decimal> prices = ScrapePrice();
            
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    for(int i = 0; i < prices.Count; i++)
                    {


                        SqlCommand cmd = new SqlCommand(SQL_WriteFlightInfo, conn);

                        cmd.Parameters.AddWithValue("@price", prices[i]);
                        cmd.Parameters.AddWithValue("@originCode", "LAX");
                        cmd.Parameters.AddWithValue("@departureDate", GetDepartureDate());
                        cmd.Parameters.AddWithValue("@returnDate", GetReturnDate());
                        cmd.Parameters.AddWithValue("@destinationCode", destinations[i].DestinationAirportCode);
                        cmd.Parameters.AddWithValue("@logTime", GetCurrentTime());
                        cmd.ExecuteNonQuery();
                        
                        
                    }
                }
            }
            catch (SqlException ex)
            {

                throw;
            }
        }
        public static DateTime GetDepartureDate()
        {
            return DateTime.Now.AddDays(5);
        }
        public static DateTime GetReturnDate()
        {
            return DateTime.Now.AddDays(12);
        }
        public static DateTime GetCurrentTime()
        {
            return DateTime.Now;
        }
    }

}


