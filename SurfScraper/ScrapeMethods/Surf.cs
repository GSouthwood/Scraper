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

        //Scrape data from magic seaweed for surf height for 10 day forecast
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


        public List<decimal> ScrapeSurfHeight(string connectionString)
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
            //for surf nodes
            List<HtmlNode> surfSize = new List<HtmlNode>();

            //for surf sizes
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
                        surfSize = doc.DocumentNode.SelectNodes("//*[@id='msw-js-fc']/div[2]/div/table/tbody/tr/td[3]/span").ToList();
                    }
                    else
                    {
                        surfSize = doc.DocumentNode.SelectNodes("//*[@id='msw-js-fc']/div[2]/div/table/tbody/tr/td[2]/span").ToList();

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
                            Decimal.Parse(item.InnerText.Substring(item.InnerText.IndexOf('-') - 2, 2))) / 2);
                    }

                    //for wave heights with format x-xxft
                    else if (item.InnerText.Length == 10)
                    {

                        surf.Add((Decimal.Parse(item.InnerText.Substring(item.InnerText.IndexOf('-') + 1, 2)) +
                            Decimal.Parse(item.InnerText.Substring(item.InnerText.IndexOf('-') - 1, 1))) / 2);

                    }

                    //for wave heights with format x-xft
                    else if (item.InnerText.Length == 9)
                    {

                        surf.Add((Decimal.Parse(item.InnerText.Substring(item.InnerText.IndexOf('-') + 1, 1)) +
                            Decimal.Parse(item.InnerText.Substring(item.InnerText.IndexOf('-') - 1, 1))) / 2);

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
                        Email.SendEmailFailure("Surf data failure", $"Failed to scrape surf height for node {counter}");
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
            Email.SendEmail("Surf data overview", $"SurfScraper got " +
                $"{((double)(surf.Count / (spots.Count * 79)) * 100)}% of all surf data.");
            return surf;
        }

        //public List<int> ScrapeWindDirection(string connectionString)
        //{
        //    //SurfHeight Data Access Object for methods
        //    SurfHeightSql surfDal = new SurfHeightSql(connectionString);

        //    //urls for scraping
        //    List<SurfUrl> spots = surfDal.LoadSpotsToScrape();

        //    HtmlWeb web = new HtmlWeb();


        //    //for wind nodes
        //    List<HtmlNode> windDirection = new List<HtmlNode>();

        //    //for wind direction degrees
        //    List<int> wind = new List<int>();

        //    foreach (var url in spots)
        //    {

        //        //if .Contains("s") accounts for when the page actually misreads the td element.
        //        HtmlDocument doc = web.Load(url.ToString());
        //        try
        //        {


        //            if (url.ToString().Equals("https://magicseaweed.com/Uluwatu-Surf-Report/565/")
        //                || url.ToString().Equals("https://magicseaweed.com/Shonan-Surf-Report/806/"))
        //            {

        //                for (int i = 2; i < 71; i += 10)
        //                {
        //                    if (i == 70)
        //                    {
        //                        for (int k = i; k < i + 7; k++)
        //                        {


        //                            if (!doc.DocumentNode.SelectSingleNode($"//*[@id='msw-js-fc']/div[2]/div/table/tbody/tr[{k}]/td[13]").InnerHtml.Contains("<span"))
        //                            {
        //                                windDirection.Add(doc.DocumentNode.SelectSingleNode($"//*[@id='msw-js-fc']/div[2]/div/table/tbody/tr[{k}]/td[13]"));
        //                            }
        //                            else if (doc.DocumentNode.SelectSingleNode($"//*[@id='msw-js-fc']/div[2]/div/table/tbody/tr[{k}]/td[13]").InnerHtml.Contains("<span"))
        //                            {
        //                                windDirection.Add(doc.DocumentNode.SelectSingleNode($"//*[@id='msw-js-fc']/div[2]/div/table/tbody/tr[{k}]/td[11]"));
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        for (int k = i; k < i + 8; k++)
        //                        {

        //                            //*[@id="msw-js-fc"]/div[2]/div/table/tbody/tr[2]/td[11]
        //                            if (!doc.DocumentNode.SelectSingleNode($"//*[@id='msw-js-fc']/div[2]/div/table/tbody/tr[{k}]/td[13]").InnerHtml.Contains("<span"))
        //                            {
        //                                windDirection.Add(doc.DocumentNode.SelectSingleNode($"//*[@id='msw-js-fc']/div[2]/div/table/tbody/tr[{k}]/td[13]"));
        //                            }
        //                            else if (doc.DocumentNode.SelectSingleNode($"//*[@id='msw-js-fc']/div[2]/div/table/tbody/tr[{k}]/td[13]").InnerHtml.Contains("<span"))
        //                            {
        //                                windDirection.Add(doc.DocumentNode.SelectSingleNode($"//*[@id='msw-js-fc']/div[2]/div/table/tbody/tr[{k}]/td[11]"));
        //                            }
        //                        }
        //                    }
        //                }


        //            }
        //            else if (url.ToString().Equals("https://magicseaweed.com/Snapper-Rocks-Surf-Report/1014/"))
        //            {
        //                for (int i = 2; i < 93; i += 10)
        //                {

        //                    if (i == 92)
        //                    {

        //                        for (int k = i; k < i + 7; k++)
        //                        {

        //                            if (doc.DocumentNode.SelectSingleNode($"//*[@id='msw-js-fc']/div[2]/div/table/tbody/tr[{k}]/td[14]") == null)
        //                            {
        //                                windDirection.Add(doc.DocumentNode.SelectSingleNode($"//*[@id='msw-js-fc']/div[2]/div/table/tbody/tr[{k}]/td[10]"));
        //                            }

        //                            else if (doc.DocumentNode.SelectSingleNode($"//*[@id='msw-js-fc']/div[2]/div/table/tbody/tr[{k}]/td[14]").InnerHtml.Contains("<span"))
        //                            {
        //                                windDirection.Add(doc.DocumentNode.SelectSingleNode($"//*[@id='msw-js-fc']/div[2]/div/table/tbody/tr[{k}]/td[12]"));
        //                            }
        //                            else
        //                            {
        //                                windDirection.Add(doc.DocumentNode.SelectSingleNode($"//*[@id='msw-js-fc']/div[2]/div/table/tbody/tr[{k}]/td[14]"));
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {


        //                        for (int k = i; k < i + 8; k++)
        //                        {
        //                            if (doc.DocumentNode.SelectSingleNode($"//*[@id='msw-js-fc']/div[2]/div/table/tbody/tr[{k}]/td[14]") == null)
        //                            {
        //                                windDirection.Add(doc.DocumentNode.SelectSingleNode($"//*[@id='msw-js-fc']/div[2]/div/table/tbody/tr[{k}]/td[10]"));
        //                            }

        //                            else if (doc.DocumentNode.SelectSingleNode($"//*[@id='msw-js-fc']/div[2]/div/table/tbody/tr[{k}]/td[14]").InnerHtml.Contains("<span"))
        //                            {
        //                                windDirection.Add(doc.DocumentNode.SelectSingleNode($"//*[@id='msw-js-fc']/div[2]/div/table/tbody/tr[{k}]/td[12]"));
        //                            }
        //                            else
        //                            {
        //                                windDirection.Add(doc.DocumentNode.SelectSingleNode($"//*[@id='msw-js-fc']/div[2]/div/table/tbody/tr[{k}]/td[14]"));
        //                            }

        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {

        //                for (int i = 2; i < 93; i += 10)
        //                {

        //                    if (i == 92)
        //                    {

        //                        for (int k = i; k < i + 7; k++)
        //                        {

        //                            //*[@id="msw-js-fc"]/div[2]/div/table/tbody/tr[94]/td[14]
        //                            if (!doc.DocumentNode.SelectSingleNode($"//*[@id='msw-js-fc']/div[2]/div/table/tbody/tr[{k}]/td[14]").InnerHtml.Contains("<span"))
        //                            {
        //                                windDirection.Add(doc.DocumentNode.SelectSingleNode($"//*[@id='msw-js-fc']/div[2]/div/table/tbody/tr[{k}]/td[14]"));
        //                            }
        //                            else if (doc.DocumentNode.SelectSingleNode($"//*[@id='msw-js-fc']/div[2]/div/table/tbody/tr[{k}]/td[14]").InnerHtml.Contains("<span"))
        //                            {
        //                                windDirection.Add(doc.DocumentNode.SelectSingleNode($"//*[@id='msw-js-fc']/div[2]/div/table/tbody/tr[{k}]/td[12]"));
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {


        //                        for (int k = i; k < i + 8; k++)
        //                        {


        //                            if (!doc.DocumentNode.SelectSingleNode($"//*[@id='msw-js-fc']/div[2]/div/table/tbody/tr[{k}]/td[14]").InnerHtml.Contains("<span"))
        //                            {
        //                                windDirection.Add(doc.DocumentNode.SelectSingleNode($"//*[@id='msw-js-fc']/div[2]/div/table/tbody/tr[{k}]/td[14]"));
        //                            }
        //                            else if (doc.DocumentNode.SelectSingleNode($"//*[@id='msw-js-fc']/div[2]/div/table/tbody/tr[{k}]/td[14]").InnerHtml.Contains("<span"))
        //                            {
        //                                windDirection.Add(doc.DocumentNode.SelectSingleNode($"//*[@id='msw-js-fc']/div[2]/div/table/tbody/tr[{k}]/td[12]"));
        //                            }
        //                        }
        //                    }
        //                }

        //            }

        //        }


        //        catch (Exception ex)
        //        {

        //            throw;
        //        }


        //        foreach (var item in windDirection)
        //        {

        //            //"  <i class=\"svg-icon svg-wind-icon svg-wind-icon-light\" style=\"-webkit-transform: rotate(298deg); -ms-transform: rotate(298deg); transform: rotate(298deg);\"></i> <i class=\"msw-saw-300 msw-swa\"></i>  "
        //            wind.Add(Int32.Parse(item.InnerHtml.Substring(item.InnerHtml.IndexOf('(') + 1,
        //                (item.InnerHtml.IndexOf('d', item.InnerHtml.IndexOf('('))) - (item.InnerHtml.IndexOf('(') + 1))));
        //        }


        //    }

        //    return wind;
        //}

    }
}
