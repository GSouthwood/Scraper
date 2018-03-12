using System;
using System.Configuration;



namespace TestScraper
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ScraperDatabase"].ConnectionString;
            DateTime start = DateTime.Now;
            Console.WriteLine($"Start time: {start.ToString()}");
            FlightsSql Flights = new FlightsSql(connectionString);
            Flights.WritePrices();
            SurfHeightSql surf = new SurfHeightSql(connectionString);           
            surf.LogSurfData();
            DateTime end = DateTime.Now;
            Console.WriteLine($"End time: {end.ToString()}");
            Console.WriteLine($"Total elapsed time: {(end.Subtract(start)).TotalMinutes}");
            Console.ReadLine();
            


        }
    }
}
