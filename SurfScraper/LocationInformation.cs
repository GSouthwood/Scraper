using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurfScraper
{
    public class LocationInformation
    {
        private string date;
        private Dictionary<string, string> airports;
        private Dictionary<string, string> region;

        public Dictionary<string, string> Airports
        {
            get
            {
                return airports;
            }
        }
        public Dictionary<string, string> Region
        {
            get
            {
                return region;
            }
        }

        public LocationInformation()
        {

        }

       
    }
}
