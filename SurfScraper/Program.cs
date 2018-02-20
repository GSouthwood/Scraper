using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TestScraper
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            BaliSurfTrip trip = new BaliSurfTrip();
            BaliFlights flights = new BaliFlights();
            flights.GetHtmlJs();
            trip.GetSurfHeight();

        }
    }
}
