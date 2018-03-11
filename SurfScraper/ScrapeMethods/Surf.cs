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

        ////Scrape data from magic seaweed for surf height for 10 day forecast
        //public List<decimal> ScrapeSurfHeight()
        //{
        //    //SurfHeight Data Access Object for methods
        //    SurfHeightSql surfDal = new SurfHeightSql(connectionString);

        //    //currently unused method for scraping weather
        //   // List<string> weather = surfDal.LogWeatherData();

        //    //urls for scraping
        //    List<SurfUrl> spots = surfDal.LoadSpotsToScrape();

        //    //counter for logging progress on the console
        //    int counter = 0;

        //    HtmlWeb web = new HtmlWeb();
        //    List<HtmlNode> surfSize = new List<HtmlNode>();
        //    List<decimal> surf = new List<decimal>();
        //    foreach (var url in spots)
        //    {

        //        //if .Contains("s") accounts for when the page actually misreads the td element.
        //        HtmlDocument doc = web.Load(url.ToString());
        //        try
        //        {
        //            surfSize = doc.DocumentNode.SelectNodes("//*[@id='msw-js-fc']/div[2]/div/table/tbody/tr/td[4]/span").ToList();
        //            if (surfSize[0].InnerText.Contains("s"))
        //            {
        //                surfSize = doc.DocumentNode.SelectNodes("//*[@id='msw-js-fc']/div[2]/div/table/tbody/tr/td[3]/span").ToList();
        //            }
        //        }

        //        catch (Exception ex)
        //        {
        //            throw;
        //        }
        //        foreach (var item in surfSize)
        //        {
        //            counter++;
        //            surf.Add(Decimal.Parse(item.InnerText.Substring(0, item.InnerText.Length - 3)));


        //        }
        //        if (surf.Count == 79 * spots.Count)
        //        {
        //            Console.WriteLine("Complete surf data.");
        //        }
        //        else
        //        {
        //            Console.WriteLine($"{79 * spots.Count - counter} data nodes to go...");
        //        }

        //    }

        //    return surf;
        //}

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
                HtmlDocument doc = web.Load(url.ToString());
                try
                {
                    
                    if (url.ToString().Equals("https://magicseaweed.com/Uluwatu-Surf-Report/565/") 
                        || url.ToString().Equals("https://magicseaweed.com/Shonan-Surf-Report/806/"))
                    {
                        //*[@id="msw-js-fc"]/div[2]/div/table/tbody/tr[2]/td[3]/span
                        surfSize = doc.DocumentNode.SelectNodes("//*[@id='msw-js-fc']/div[2]/div/table/tbody/tr/td[3]/span").ToList();
                    }
                    else
                    {
                    surfSize = doc.DocumentNode.SelectNodes("//*[@id='msw-js-fc']/div[2]/div/table/tbody/tr/td[2]/span").ToList();
                        if (surfSize[0].InnerText.Contains("s"))
                        {
                            surfSize = doc.DocumentNode.SelectNodes("//*[@id='msw-js-fc']/div[2]/div/table/tbody/tr/td[3]/span").ToList();
                        }
                    }
                    
                }
                

                catch (Exception ex)
                {
                    
                    throw;
                }
                foreach (var item in surfSize)
                {
                    counter++;
                    
                    //for wave heights with format xx-xxft
                    if (item.InnerText.Length == 11)
                    {
                        surf.Add((Decimal.Parse(item.InnerText.Substring(item.InnerText.IndexOf('-') + 1, 2)) +
                            Decimal.Parse(item.InnerText.Substring(item.InnerText.IndexOf('-') - 2, 2)))/2);
                    }

                    //for wave heights with format x-xxft
                    else if (item.InnerText.Length == 10)
                    {

                        surf.Add((Decimal.Parse(item.InnerText.Substring(item.InnerText.IndexOf('-') + 1, 2)) +
                            Decimal.Parse(item.InnerText.Substring(item.InnerText.IndexOf('-') - 1, 1)))/2);

                    }

                    //for wave heights with format x-xft
                    else if (item.InnerText.Length == 9)
                    {

                        surf.Add((Decimal.Parse(item.InnerText.Substring(item.InnerText.IndexOf('-') + 1, 1)) +
                            Decimal.Parse(item.InnerText.Substring(item.InnerText.IndexOf('-') - 1, 1)))/2);

                    }

                    //for wave heights with format xxft
                    else if (item.InnerText.Length == 8 && !item.InnerText.Contains("-"))
                    {

                        surf.Add(Decimal.Parse(item.InnerText.Substring(item.InnerText.IndexOf('f') - 2, 2)));

                    }

                    //for wave heights with format xft
                    else if (item.InnerText.Length == 7 && !item.InnerText.Contains("-"))
                    {

                        surf.Add(Decimal.Parse(item.InnerText.Substring(item.InnerText.IndexOf('f') - 1, 1)));

                    }

                    //for wave heights with format xx x or Flat
                    else if (item.InnerText.Length == 6 || item.InnerText.Length == 5 && item.InnerText.Contains(" Flat "))
                    {
                        surf.Add(0);
                    }

                    //for wave heights with format xx x
                    else if (item.InnerText.Length == 6 || item.InnerText.Length == 5 && !item.InnerText.Contains(" Flat "))
                    {
                        surf.Add(Decimal.Parse(item.InnerText.Substring(0, item.InnerText.Length - 3)));
                    }

                    //for wave heights that fail to pass bools
                    else
                    {
                        Console.WriteLine($"Failed to scrape surf height for node {counter}");
                        
                    }

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
