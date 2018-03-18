using System;
using System.Configuration;
using System.Net.Mail;
using SurfScraper.UtilityMethods;



namespace TestScraper
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ScraperDatabase"].ConnectionString;
            DateTime start = DateTime.Now;
            Email.SendEmail("Start Time", $"SurfScraper has started running at: {start.ToString()}");
            Console.WriteLine();
            SurfHeightSql surf = new SurfHeightSql(connectionString);
            //fix wind
            surf.LogSurfData();
            FlightsSql Flights = new FlightsSql(connectionString);
            Flights.WritePrices();
            
            DateTime end = DateTime.Now;
            Email.SendEmail("End Time", $"SurfScraper has ended running at: {end.ToString()}\n" +
                $"Total elapsed time: {(end.Subtract(start)).TotalMinutes}");
            Console.WriteLine($"End time: {end.ToString()}");
            Console.WriteLine($"Total elapsed time: {Math.Round((end.Subtract(start)).TotalMinutes, 2)}");
            
            


        }
    }
}
