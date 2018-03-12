using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using SurfScraper.Model;
using TestScraper;

namespace SurfScraper.ScrapeMethods
{
    public class Flight
    {
        

        private string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=Scraper;Integrated Security=True";


        //scrape prices from SkyScanner

        //public List<decimal> ScrapePrice()
        //{
        //    FlightsSql flightsSql = new FlightsSql(connectionString);
        //    List<FlightUrl> destinations = flightsSql.LoadDestinationsToScrape();

        //    using (WebBrowser wb = new WebBrowser())
        //    {
        //        string text = "";
        //        decimal price = 0;
        //        bool isWorking = false;
        //        int count = 0;
        //        int tryCounter = 0;
        //        List<decimal> prices = new List<decimal>();

        //        foreach (var url in destinations)
        //        {
        //            isWorking = false;
        //            Console.WriteLine($"Trying {url.DestinationAirportCode}...");
        //            while (!isWorking)
        //            {
        //                wb.ScriptErrorsSuppressed = true;
        //                wb.Navigate($"{url}");

        //                while (wb.ReadyState != WebBrowserReadyState.Complete || tryCounter != 5)
        //                {

        //                    Application.DoEvents();
        //                    Thread.Sleep(10000);
        //                    Application.DoEvents();
        //                    tryCounter++;

        //                }

        //                if (wb.Document.GetElementsByTagName("tbody").Count > 0 || tryCounter == 5)
        //                {
        //                    isWorking = true;
        //                }
        //            }
        //            if (tryCounter == 5)
        //            {
        //                count++;

        //                Console.WriteLine($"Number {count} of {destinations.Count} failed.");
        //                tryCounter = 0;
        //            }

        //            else
        //            {


        //                text = wb.Document.GetElementsByTagName("tbody")[0].InnerText.Substring(5, 7);
        //                if (text.Contains(" "))
        //                {
        //                    int limit = text.IndexOf(" ");
        //                    text = wb.Document.GetElementsByTagName("tbody")[0].InnerText.Substring(5, limit);
        //                    price = Decimal.Parse(text);
        //                }

        //                prices.Add(price);
        //                count++;
        //                Console.WriteLine($"{count}/{destinations.Count} Done. Price = ${price}");
        //            }

        //        }
        //        return prices;
        //    }
        //}
        public List<decimal> ScrapePrice()
        {
            FlightsSql flightsSql = new FlightsSql(connectionString);
            List<FlightUrl> destinations = flightsSql.LoadDestinationsToScrape();

            int tryCount = 0;
            string text = "";
            decimal price = 0;
            bool isWorking = false;
            int count = 0;
            List<decimal> prices = new List<decimal>();

            for (int i = 0; i < 50; i++)
            {
                using (WebBrowser wb = new WebBrowser())
                {

                    isWorking = false;
                    Console.WriteLine($"Trying {destinations[i].DestinationAirportCode}...");

                    while (!isWorking)
                    {
                        wb.ScriptErrorsSuppressed = true;
                        wb.Navigate($"{destinations[i]}");

                        while ((wb.ReadyState != WebBrowserReadyState.Complete) && tryCount != 5)
                        {
                            Application.DoEvents();
                            Thread.Sleep(3000);
                            Application.DoEvents();
                            tryCount++;
                        }
                        
                        if (wb.Document.GetElementsByTagName("tbody").Count > 0)
                        {
                            isWorking = true;
                            tryCount = 0;
                        }
                        else if (tryCount == 5)
                        {
                            
                            count++;
                            isWorking = true;
                            Console.WriteLine($"Failed at {count}/{destinations.Count} after {tryCount} attempts");
                            tryCount = 0;
                        }
                    }


                    if (wb.Document.GetElementsByTagName("tbody").Count > 0)
                    {

                        text = wb.Document.GetElementsByTagName("tbody")[0].InnerText.Substring(5, 7);
                        if (text.Contains(" "))
                        {
                            int limit = text.IndexOf(" ");
                            text = wb.Document.GetElementsByTagName("tbody")[0].InnerText.Substring(5, limit);
                            price = Decimal.Parse(text);
                        }

                        prices.Add(price);
                        count++;
                        Console.WriteLine($"{count}/{destinations.Count} Done.");
                    }
                    

                }

            }
            return prices;
        }
    }
}

