using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.IO;
using System.Windows.Forms;
using System.Threading;

namespace TestScraper
{
    public class BaliSurfTrip
    {


        public BaliSurfTrip()
        {


        }

        public static void LogSurfHeight()
        {

            HtmlWeb web = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = web.Load("http://www.surfline.com/surf-forecasts/indonesia/bali_2169/");
            List<HtmlNode> surfSize = doc.DocumentNode.SelectNodes("//*[@id='observed_component']//div[3]//div[1]//div[1]//div//div//div[3]//div//div[1]//h1").ToList();
            using (StreamWriter sw = new StreamWriter("SizeLog.txt", false))
            {

                foreach (var item in surfSize)
                {
                    sw.WriteLine(item.InnerText);
                }
            }

        }
        public static void LogDates()
        {

            HtmlWeb web = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = web.Load("http://www.surfline.com/surf-forecasts/indonesia/bali_2169/");
            List<HtmlNode> surfSize = doc.DocumentNode.SelectNodes("//*[@id='observed_component']/div[3]/div[1]/div[1]/div/div/div[1]/span").ToList();
            using (StreamWriter sw = new StreamWriter("DateLog.txt", false))
            {


                foreach (var item in surfSize)
                {
                    sw.WriteLine(item.InnerText);
                }
            }

        }

        public void GetSurfHeight()
        {
            LogSurfHeight();
            LogDates();
            BaliFlights flights = new BaliFlights();
            string flightPrice = flights.GetHtmlJs();


            List<string> sizes = new List<string>();
            List<string> dates = new List<string>();
           
            using (StreamReader sizesSr = new StreamReader("SizeLog.txt"))
            {
                using (StreamReader datesSr = new StreamReader("DateLog.txt"))
                {


                    while (!sizesSr.EndOfStream && !datesSr.EndOfStream)
                    {

                        sizes.Add(sizesSr.ReadLine());

                        dates.Add(datesSr.ReadLine());

                    }

                }
            }
            //for (int i = 0; i < dates.Count; i++)
            //{


                if (int.Parse(sizes[0].Substring(0, 1)) > 4)
                {
                    Console.WriteLine($"There is a roundtrip flight to Bali for {flightPrice} on {dates[0]} and the waves are {sizes[0]} the rest of the week.\nYou should book a flight! ");
                    Console.WriteLine("--------------------------------------------------------------------------------------------------------------------");
                }
                else if (int.Parse(sizes[1].Substring(0, 1)) > 4)
                {
                    Console.WriteLine($"There is a roundtrip flight to Bali for {flightPrice} on {dates[0]} and the waves are {sizes[1]} the rest of the week.\nYou should book a flight! ");
                    Console.WriteLine("--------------------------------------------------------------------------------------------------------------------");
                }
                else if (int.Parse(sizes[2].Substring(0, 1)) > 4)
                {
                    Console.WriteLine($"There is a roundtrip flight to Bali for {flightPrice} on {dates[0]} and the waves are {sizes[2]} the rest of the week.\nYou should book a flight! ");
                    Console.WriteLine("--------------------------------------------------------------------------------------------------------------------");
                }
                else
                {
                    Console.WriteLine($"Bali has small waves this week, don't bother flying there for {flightPrice}.");
                    Console.WriteLine("-----------------------------------------------------------------------------");
                }
            //}
            Console.ReadLine();

        }

    }
}
