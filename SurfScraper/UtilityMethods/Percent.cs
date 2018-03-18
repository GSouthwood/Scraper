using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurfScraper.UtilityMethods
{
    public class Percent
    {
        public static double DataAvailableVsPercentScraped(int nodesSraped, int nodesMissed, int total)
        {
            return ((((double)(nodesSraped - nodesMissed) / total)) * 100);
        }

        


    }
}
