using AngleSharp;
using AngleSharp.Dom;
using System.Diagnostics;

namespace BMAH_WoM.SourceCode
{
    public class Scraper
    {
        public async void ScrapeData()
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

            foreach (string wowserver in wowservers)
            {
                var config = Configuration.Default.WithDefaultLoader().WithXPath().WithJs();
                var source = "https://tradeskillmaster.com/black-market?realm=" + wowserver;
                var document = await BrowsingContext.New(config).OpenAsync(source).WaitUntilAvailable();

                var rows = document.QuerySelectorAll("*[xpath>'//tbody/tr']");
                Debug.Print(wowserver);

                foreach (var row in rows)
                {
                    var EmptyTSM = row.QuerySelector("*[xpath>'//td[1]']").TextContent;
                    if (EmptyTSM == "No results found.")
                    {
                        Debug.Print("TSM has no data for this server at the moment, please try later!");
                    }
                    else
                    {
                        var ItemName = row.QuerySelector("*[xpath>'//td[1]/a[1]']").Attributes["title"].Value;
                        var CurrentBid = row.QuerySelector("*[xpath>'//td[2]']").TextContent;
                        var MinBid = row.QuerySelector("*[xpath>'//td[3]']").TextContent;
                        var TimeLeft = row.QuerySelector("*[xpath>'//td[4]']").TextContent;
                        var NBids = row.QuerySelector("*[xpath>'//td[5]']").TextContent;
                        var RealmMarket = row.QuerySelector("*[xpath>'//td[6]']").TextContent;
                        var GlobalMarket = row.QuerySelector("*[xpath>'//td[7]']").TextContent;
                        var RealmAHQty = row.QuerySelector("*[xpath>'//td[8]']").TextContent;

                        Debug.Print("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}", ItemName, CurrentBid, MinBid, TimeLeft, NBids, RealmMarket, GlobalMarket, RealmAHQty);
                    }
                }
            }
        }
    }
}