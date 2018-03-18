using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Mail;


namespace SurfScraper.UtilityMethods
{
    public class DateAndTime

    {
        public static List<TimeSpan> LoadTimes()
        {
            List<TimeSpan> times = new List<TimeSpan>();
            for (int i = 0; i <= 21; i += 3)
            {
                times.Add(new TimeSpan(i, 0, 0));
            }
            return times;
        }
        public static DateTime CurrentDateTime()
        {
            return DateTime.Now;
        }
        public static DateTime GetDepartureDate()
        {
            return DateTime.Now.AddDays(2);
        }
        public static DateTime GetReturnDate()
        {
            return DateTime.Now.AddDays(7);
        }
 
    }
}
