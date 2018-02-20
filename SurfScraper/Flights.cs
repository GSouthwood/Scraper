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

        public void GetHtmlJs()
        {
            using (WebBrowser wb = new WebBrowser())
            {
                bool isWorking = false;
                while (!isWorking)
                {
                    wb.ScriptErrorsSuppressed = true;
                    wb.Navigate("https://www.skyscanner.com/transport/flights/cmha/dps/180221/180228?adults=1&children=0&adultsv2=1&childrenv2=&infants=0&cabinclass=economy&rtn=1&preferdirects=false&outboundaltsenabled=false&inboundaltsenabled=false&ref=home#results");

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

                string text = wb.Document.GetElementsByTagName("tbody")[0].OuterHtml;


                using (StreamWriter sr = new StreamWriter("FlightLog.txt", false))
                {
                    sr.Write(text);
                }

            }
        }




    }

}


