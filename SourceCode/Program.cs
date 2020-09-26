using AngleSharp;

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
                var config = Configuration.Default.WithDefaultLoader().WithXPath();
                var source = "https://tradeskillmaster.com/black-market?realm=" + wowserver;
                var document = await BrowsingContext.New(config).OpenAsync(source);

                var ItemName = document.QuerySelectorAll("span:nth-child(1)");
            }
        }
    }
}