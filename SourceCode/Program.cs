using AngleSharp;

namespace BMAH_WoM.SourceCode
{
    public class Program
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
                var config = Configuration.Default;
                var context = BrowsingContext.New(config);

                var source = @"https://www.tradeskillmaster.com/black-market?realm=" + wowserver;
                var document = await context.OpenAsync(req => req.Content(source));
            }
        }
    }
}