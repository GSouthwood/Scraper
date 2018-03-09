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

        public List<decimal> ScrapePrice()
        {
            FlightsSql flightsSql = new FlightsSql(connectionString);
            List<FlightUrl> destinations = flightsSql.LoadDestinationsToScrape();

            using (WebBrowser wb = new WebBrowser())
            {
                string text = "";
                decimal price = 0;
                bool isWorking = false;
                int count = 0;
                List<decimal> prices = new List<decimal>();

                foreach (var url in destinations)
                {
                    isWorking = false;
                    Console.WriteLine($"Trying {url.DestinationAirportCode}...");
                    while (!isWorking)
                    {
                        wb.ScriptErrorsSuppressed = true;
                        wb.Navigate($"{url}");

                        while (wb.ReadyState != WebBrowserReadyState.Complete)
                        {
                            Application.DoEvents();
                            Thread.Sleep(10000);
                            Application.DoEvents();

                        }
                        if (wb.Document.GetElementsByTagName("tbody").Count > 0)
                        {
                            isWorking = true;
                        }
                    }


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
                return prices;
            }
        }
    }
}
