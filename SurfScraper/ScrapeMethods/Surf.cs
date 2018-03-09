using HtmlAgilityPack;
using System.Collections.Generic;
using SurfScraper.Model;
using System;
using System.Linq;
using SurfScraper.UtilityMethods;
using TestScraper;

namespace SurfScraper.ScrapeMethods
{
    public class Surf
    {
        private string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=Scraper;Integrated Security=True";

        //Scrape data from magic seaweed for surf height for 10 day forecast
        public List<decimal> ScrapeSurfHeight()
        {
            //SurfHeight Data Access Object for methods
            SurfHeightSql surfDal = new SurfHeightSql(connectionString);

            //currently unused method for scraping weather
           // List<string> weather = surfDal.LogWeatherData();

            //urls for scraping
            List<SurfUrl> spots = surfDal.LoadSpotsToScrape();

            //counter for logging progress on the console
            int counter = 0;

            HtmlWeb web = new HtmlWeb();
            List<HtmlNode> surfSize = new List<HtmlNode>();
            List<decimal> surf = new List<decimal>();
            foreach (var url in spots)
            {

                //if .Contains("s") accounts for when the page actually misreads the td element.
                HtmlAgilityPack.HtmlDocument doc = web.Load(url.ToString());
                try
                {
                    surfSize = doc.DocumentNode.SelectNodes("//*[@id='msw-js-fc']/div[2]/div/table/tbody/tr/td[4]/span").ToList();
                    if (surfSize[0].InnerText.Contains("s"))
                    {
                        surfSize = doc.DocumentNode.SelectNodes("//*[@id='msw-js-fc']/div[2]/div/table/tbody/tr/td[3]/span").ToList();
                    }
                }

                catch (Exception ex)
                {
                    throw;
                }
                foreach (var item in surfSize)
                {
                    counter++;
                    surf.Add(Decimal.Parse(item.InnerText.Substring(0, item.InnerText.Length - 3)));


                }
                if (surf.Count == 79 * spots.Count)
                {
                    Console.WriteLine("Complete surf data.");
                }
                else
                {
                    Console.WriteLine($"{79 * spots.Count - counter} data nodes to go...");
                }

            }

            return surf;
        }


        ////Method for getting weather data from magicseaweed 
        ////buggy

        //public List<string> LogWeatherData()
        //{

        //    HtmlWeb web = new HtmlWeb();
        //    List<HtmlNode> weatherNodes = new List<HtmlNode>();
        //    List<string> weather = new List<string>();
        //    HtmlAgilityPack.HtmlDocument doc = web.Load("https://magicseaweed.com/Uluwatu-Surf-Report/565/");
        //    try
        //    {
        //        weatherNodes = doc.DocumentNode.SelectNodes("//*[@id='msw-js-fc']/div[2]/div/table/tbody/tr/td[12]/span").ToList();
        //        if (weatherNodes[0].InnerText.Contains("s"))
        //        {
        //            weatherNodes = doc.DocumentNode.SelectNodes("//*[@id='msw-js-fc']/div[2]/div/table/tbody/tr/td[13]/span").ToList();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //    foreach (var item in weatherNodes)
        //    {
        //        weather.Add(item.InnerText);
        //    }


        //    return weather;


        //}
    }
}
