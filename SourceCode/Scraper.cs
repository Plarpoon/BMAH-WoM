using HtmlAgilityPack;
using System.Diagnostics;
using System.Linq;

namespace BMAH_WoM.SourceCode
{
    public class Scraper
    {
        public void ScrapeData()
        {
            string[] wowservers = new string[]
            {
                "EU-aegwynn",
                "US-area-52",
                "US-tichondrius",
                "US-illidan",
                "US-zuljin",
                "US-malganis",
                "US-thrall",
                "US-bleeding-hollow",
                "US-hyjal",
                "US-turalyon",
                "US-proudmoore",
                "US-stormrage",
                "US-sargeras",
                "US-frostmourne",
                "US-barthilas",
                "US-kiljaeden",
                "US-nordrassil",
                "US-gorefiend",
                "US-silver-hand",
                "US-bloodhoof",
                "US-kelthuzad",
                "US-arthas"
            };

            int TotalWoWservers = wowservers.Count();

            for (int i = 0; i < TotalWoWservers; i++)
            /*cycle trough different WoW servers consider changing said cycle with another type that supports
             * an index (will be the same amount of supported servers) so that you can apply a retry extra
             * cycle if it fails during data retrieval*/

            {
                string wowserver = wowservers[i];

                var url = @"https://www.tradeskillmaster.com/black-market?realm=" + wowserver;
                Debug.Print(wowserver);

                HtmlWeb web = new HtmlWeb();
                var htmlDoc = web.Load(url);
                htmlDoc.OptionEmptyCollection = true;
                var rows = htmlDoc.DocumentNode.SelectNodes("//tbody/tr");

                foreach (var row in rows)
                {
                    var EmptyTSM = row.SelectSingleNode("td[1]").InnerText;
                    if (EmptyTSM == "No results found.")
                    {
                        Debug.Print("TSM has no data for this server at the moment, please try later!");
                    }
                    else
                    {
                        var ItemName = row.SelectSingleNode("td[1]/a[1]").Attributes["title"].Value;
                        var CurrentBid = row.SelectSingleNode("td[2]").InnerText;
                        var MinBid = row.SelectSingleNode("td[3]").InnerText;
                        var TimeLeft = row.SelectSingleNode("td[4]").InnerText;
                        var NBids = row.SelectSingleNode("td[5]").InnerText;
                        var RealmMarket = row.SelectSingleNode("td[6]").InnerText;
                        var GlobalMarket = row.SelectSingleNode("td[7]").InnerText;
                        var RealmAHQty = row.SelectSingleNode("td[8]").InnerText;

                        Debug.Print("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}", ItemName, CurrentBid, MinBid, TimeLeft, NBids, RealmMarket, GlobalMarket, RealmAHQty);
                    }
                }
            }
        }
    }
}