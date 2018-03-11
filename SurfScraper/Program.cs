﻿using System;
using System.Configuration;



namespace TestScraper
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ScraperDatabase"].ConnectionString;
            FlightsSql Flights = new FlightsSql(connectionString);
            Flights.WritePrices();
            SurfHeightSql surf = new SurfHeightSql(connectionString);           
            surf.LogSurfData();
            Console.ReadLine();
            


        }
    }
}
