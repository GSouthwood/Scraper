using System;


namespace SurfScraper.Model
{
    public class FlightUrl
    {
        public string DestinationAirportCode { get; set; }       
        public string LocationName { get; set; }
        public int LocationId { get; set; }
        public string SkyScannerDomainName { get; set; }
        public string DepartureDate { get; set; }
        public string ReturnDate { get; set; }
        public string SkyScannerVariables { get; set; }
        public DateTime Depart { get; set; }
        public DateTime Return { get; set; }
             
        public override string ToString()
        {
            return SkyScannerDomainName + DestinationAirportCode + DepartureDate + ReturnDate + SkyScannerVariables;
        }
    }
}
