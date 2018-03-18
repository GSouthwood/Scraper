using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using SurfScraper.Model;
using TestScraper;
using SurfScraper.UtilityMethods;

namespace SurfScraper.ScrapeMethods
{
    public class Flight
    {
        


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
        public List<decimal> ScrapePrice(DateTime now, string connectionString)
        {
            FlightsSql flightsSql = new FlightsSql(connectionString);
            List<FlightUrl> destinations = flightsSql.LoadDestinationsToScrape(connectionString);

            int tryCount = 0;
            string text = "";
            decimal price = 0;
            bool isWorking = false;
            int count = 0;
            int failureCount = 0;
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
                        
                        
                        if (tryCount == 5)
                        {
                            
                            count++;
                            isWorking = true;
                            Email.SendEmailFailure("Flight data failure", $"Failed at " +
                                $"{count}/{destinations.Count} - {destinations[i].DestinationAirportCode} after {tryCount} attempts.");
                            Console.WriteLine($"Failed at {count}/{destinations.Count} after {tryCount} attempts");
                            failureCount++;
                            tryCount = 0;
                        }
                        else if (wb.Document.GetElementsByTagName("tbody").Count > 0)
                        {
                            isWorking = true;
                            tryCount = 0;

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
                                if (DateAndTime.CurrentDateTime().Minute - now.Minute >= 30)
                                {
                                    Email.SendEmail($"Flight data update at {DateAndTime.CurrentDateTime()}", 
                                        $"SurfScraper has gotten {Percent.DataAvailableVsPercentScraped(count, failureCount, 50)}% of all flight data so far.");
                                }
                                if (DateAndTime.CurrentDateTime().Minute - now.Minute >= 60)
                                {
                                    Email.SendEmail($"Flight data update at {DateAndTime.CurrentDateTime()}", 
                                        $"SurfScraper has gotten {Percent.DataAvailableVsPercentScraped(count, failureCount, 50)}% of all flight data so far.");
                                }
                            }
                        }
                    }



                   

                }

            }
            Email.SendEmail("Flight data overview", $"SurfScraper got " +
                $"{Percent.DataAvailableVsPercentScraped(count, failureCount, 50)}% of all flight data.");
            return prices;
        }
    }
}

