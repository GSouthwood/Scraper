using System;


namespace SurfScraper.Model
{
    public class SurfUrl
    {



        public string SpotName { get; set; }
        public int SpotId { get; set; }
        public string SeaweedDomainName { get; set; }
        public int LocationId { get; set; }



        public override string ToString()
        {
            return $"{SeaweedDomainName}{SpotName}-Surf-Report/{SpotId}/";
        }

    }
}
