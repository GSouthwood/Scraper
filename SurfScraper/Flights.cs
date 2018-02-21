using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.IO;
using System.Windows.Forms;
using System.Threading;


namespace TestScraper
{
    public class BaliFlights
    {
        public BaliFlights()
        {

        }

        public string GetHtmlJs()
        {
            using (WebBrowser wb = new WebBrowser())
            {
                string text = "";
                bool isWorking = false;
                while (!isWorking)
                {
                    wb.ScriptErrorsSuppressed = true;
                    wb.Navigate("https://www.skyscanner.com/transport/flights/cmh/dps/180221/180228?adults=2&children=0&adultsv2=2&childrenv2=&infants=0&cabinclass=economy&rtn=1&preferdirects=false&outboundaltsenabled=false&inboundaltsenabled=false&ref=home#results");

                    while (wb.ReadyState != WebBrowserReadyState.Complete)
                    {
                        Application.DoEvents();
                        Thread.Sleep(2000);
                        Application.DoEvents();

                    }
                    if (wb.Document.GetElementsByTagName("tbody").Count > 0)
                    {
                        isWorking = true;
                    }
                }

                //string text = wb.Document.GetElementsByTagName("tbody")[0].OuterHtml;
                text = wb.Document.GetElementsByTagName("tbody")[0].OuterHtml.Substring(349, 6);

                //using (StreamWriter sr = new StreamWriter("FlightLog.txt", false))
                //{
                //    sr.Write(text);
                //}
                return text;
            }
        }




    }

}


