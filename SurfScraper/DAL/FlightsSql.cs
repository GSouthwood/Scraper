using System.Data.SqlClient;
using System.Collections.Generic;
using SurfScraper.Model;
using System;
using SurfScraper.ScrapeMethods;
using SurfScraper.UtilityMethods;



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
        public List<FlightUrl> LoadDestinationsToScrape(string connectionString)
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
                        for (int i = 0; i < 6; i++)
                        {

                            

                            //six day trips (4 full days, 2 days travel)
                                FlightUrl f = new FlightUrl();
                                f.DestinationAirportCode = Convert.ToString(reader["airport_code"]);
                                f.LocationName = Convert.ToString(reader["name"]);
                                f.LocationId = Convert.ToInt32(reader["location_id"]);
                                f.SkyScannerDomainName = "https://www.skyscanner.com/transport/flights/lax/";
                                f.DepartureDate = DateAndTime.GetDepartureDate().AddDays(i).ToString("/yyMMdd/");
                                f.ReturnDate = DateAndTime.GetReturnDate().AddDays(i).ToString("yyMMdd");
                                f.SkyScannerVariables = "?adults=1&children=0&adultsv2=1&childrenv2=&infants=0&" +
                                    "cabinclass=economy&rtn=1&preferdirects=false&" +
                                    "outboundaltsenabled=false&inboundaltsenabled=false&ref=home#results";
                                f.Depart = DateAndTime.GetDepartureDate().AddDays(i);
                                f.Return = DateAndTime.GetReturnDate().AddDays(i);

                                destinations.Add(f);




                           //seven day trips (5 full days, 2 days travel)
                                FlightUrl f2 = new FlightUrl();
                                f2.DestinationAirportCode = Convert.ToString(reader["airport_code"]);
                                f2.LocationName = Convert.ToString(reader["name"]);
                                f2.LocationId = Convert.ToInt32(reader["location_id"]);
                                f2.SkyScannerDomainName = "https://www.skyscanner.com/transport/flights/lax/";
                                f2.DepartureDate = DateAndTime.GetDepartureDate().AddDays(i).ToString("/yyMMdd/");
                                f2.ReturnDate = DateAndTime.GetReturnDate().AddDays(i+1).ToString("yyMMdd");
                                f2.SkyScannerVariables = "?adults=1&children=0&adultsv2=1&childrenv2=&infants=0&" +
                                    "cabinclass=economy&rtn=1&preferdirects=false&" +
                                    "outboundaltsenabled=false&inboundaltsenabled=false&ref=home#results";
                                f2.Depart = DateAndTime.GetDepartureDate().AddDays(i);
                                f2.Return = DateAndTime.GetReturnDate().AddDays(i+1);

                                destinations.Add(f2);
                            
                        }
                    }

                }
            }
            catch (SqlException e)
            {
                Email.SendEmailFailure(e.ToString(), "Failed during: public List<FlightUrl> LoadDestinationsToScrape()");
                throw;
            }

            return destinations;

        }


        // Insert flights rows into Scraper
        public void WritePrices()
        {
            FlightsSql flightsSql = new FlightsSql(connectionString);
            List<FlightUrl> destinations = flightsSql.LoadDestinationsToScrape(connectionString);

            //Flight object for flight methods
            Flight flight = new Flight();
            List<decimal> prices = flight.ScrapePrice(DateAndTime.CurrentDateTime(), connectionString);
            

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
                        cmd.Parameters.AddWithValue("@departureDate", destinations[i].Depart);
                        cmd.Parameters.AddWithValue("@returnDate", destinations[i].Return);
                        cmd.Parameters.AddWithValue("@destinationCode", destinations[i].DestinationAirportCode);
                        cmd.Parameters.AddWithValue("@logTime", DateAndTime.CurrentDateTime());
                        int worked = cmd.ExecuteNonQuery();
                        if (worked > 0)
                        {
                            Console.WriteLine($"Input {worked} row");
                        }
                        
                       
                          
                        
                        
                        
                    }
                }
            }
            catch (SqlException e)
            {
                Email.SendEmailFailure(e.ToString(), "Failed during: public void WritePrices()");
                throw;
            }
        }
        
    }

}


