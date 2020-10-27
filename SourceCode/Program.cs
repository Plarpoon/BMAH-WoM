using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Io;
using ClosedXML.Excel;
using Microsoft.Win32;
using System;
using System.Diagnostics;

namespace BMAH_WoM.SourceCode
{
    public class Scraper
    {
        public async void ScrapeData()
        {
            var wb = new XLWorkbook();

            //preparing time string
            // Don't be fooled - this really is the Pacific time zone,
            // not just standard time...
            var zone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            var utcNow = DateTime.UtcNow;
            var pacificNow = TimeZoneInfo.ConvertTimeFromUtc(utcNow, zone);
            string timestamp = pacificNow.ToString();

            //define Table and it's child names
            var ws = wb.Worksheets.Add("WoM-BMAH scan");

            ws.Cell("B2").Value = "WoM-BMAH " + timestamp + " PST";
            ws.Cell("B3").Value = "Server Name";
            ws.Cell("C3").Value = "Item Name";
            ws.Cell("D3").Value = "Current Bid";
            ws.Cell("E3").Value = "Min. Bid";
            ws.Cell("F3").Value = "Time Left";
            ws.Cell("G3").Value = "# of Bids";
            ws.Cell("H3").Value = "Realm Market";
            ws.Cell("I3").Value = "Global Market";
            ws.Cell("J3").Value = "Realm AH Qty.";

            //define ranges
            var rngTable = ws.Range("B2:J3");   //change the second value of the range to something adaptive based on the amount of data received

            //world of warcraft server array
            string[] wowservers = new string[]
            {
                "US-area-52",
                "US-arthas",
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
                "US-scilla",
                "US-nordrassil",
                "US-deathwing",
                "US-dentarg",
                "US-blackhand",
                "US-gorefiend",
                "US-wyrmrest-accord",
                "US-silver-hand",
                "US-azuremyst",
                "US-bloodhoof",
                "US-kelthuzad"
            };
            int RowCounter = 3;
            foreach (string wowserver in wowservers)
            {
                //user-agent
                var requester = new DefaultHttpRequester();
                requester.Headers["User-Agent"] = "Mozilla/5.0 (compatible; Googlebot/2.1; +http://www.google.com/bot.html)";

                //anglesharp configuration
                var config = Configuration.Default
                    .With(requester)
                    .WithDefaultLoader()
                    .WithXPath()
                    .WithJs();
                var source = "https://tradeskillmaster.com/black-market?realm=" + wowserver;
                var document = await BrowsingContext.New(config).OpenAsync(source).WaitUntilAvailable();

                //table selector
                var rows = document.QuerySelectorAll("*[xpath>'//tbody/tr']");
                Debug.Print(wowserver);

                foreach (var row in rows)
                {
                    var EmptyTSM = row.QuerySelector("*[xpath>'//td[1]']").TextContent;
                    if (EmptyTSM == "No results found.")
                    {
                        RowCounter += 1;
                        rngTable = ws.Range("B2:J" + RowCounter);
                        Debug.Print("TSM has no data for this server at the moment, try again later!");
                        ws.Cell("B" + RowCounter).Value = wowserver;
                        ws.Cell("C" + RowCounter).Value = "No results found.";
                        ws.Cell("D" + RowCounter).Value = "N/A";
                        ws.Cell("E" + RowCounter).Value = "N/A";
                        ws.Cell("F" + RowCounter).Value = "N/A";
                        ws.Cell("G" + RowCounter).Value = "N/A";
                        ws.Cell("H" + RowCounter).Value = "N/A";
                        ws.Cell("I" + RowCounter).Value = "N/A";
                        ws.Cell("J" + RowCounter).Value = "N/A";
                    }
                    else
                    {
                        RowCounter += 1;
                        rngTable = ws.Range("B2:J" + RowCounter);
                        ws.Cell("B" + RowCounter).Value = wowserver;
                        var ItemName = row.QuerySelector("*[xpath>'//td[1]/a[1]']").Attributes["title"].Value;
                        ws.Cell("C" + RowCounter).Value = ItemName;
                        var CurrentBid = row.QuerySelector("*[xpath>'//td[2]']").TextContent;
                        ws.Cell("D" + RowCounter).Value = CurrentBid;
                        var MinBid = row.QuerySelector("*[xpath>'//td[3]']").TextContent;
                        ws.Cell("E" + RowCounter).Value = MinBid;
                        var TimeLeft = row.QuerySelector("*[xpath>'//td[4]']").TextContent;
                        ws.Cell("F" + RowCounter).Value = TimeLeft;
                        var NBids = row.QuerySelector("*[xpath>'//td[5]']").TextContent;
                        ws.Cell("G" + RowCounter).Value = NBids;
                        var RealmMarket = row.QuerySelector("*[xpath>'//td[6]']").TextContent;
                        ws.Cell("H" + RowCounter).Value = RealmMarket;
                        var GlobalMarket = row.QuerySelector("*[xpath>'//td[7]']").TextContent;
                        ws.Cell("I" + RowCounter).Value = GlobalMarket;
                        var RealmAHQty = row.QuerySelector("*[xpath>'//td[8]']").TextContent;
                        ws.Cell("J" + RowCounter).Value = RealmAHQty;

                        Debug.Print("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}", ItemName, CurrentBid, MinBid, TimeLeft, NBids, RealmMarket, GlobalMarket, RealmAHQty);
                    }
                }
                RowCounter += 1;    //Experimental, adds a blank row between different servers
            }
            //format title cell
            rngTable.Cell(1, 1).Style.Font.Bold = true;
            rngTable.Cell(1, 1).Style.Fill.BackgroundColor = XLColor.CornflowerBlue;
            rngTable.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngTable.Row(1).Merge();

            //header customization
            var rngHeaders = rngTable.Range("B3:J3");
            rngHeaders.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngHeaders.Style.Font.Bold = true;
            rngHeaders.Style.Fill.BackgroundColor = XLColor.Aqua;

            //table customization
            ws.Columns(2, 10).AdjustToContents();
            rngTable.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
            rngTable.Style.Border.BottomBorder = XLBorderStyleValues.Thin;

            //save the Excel sheet
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel files|*.xlsx"
            };

            var serialVal = "WoM - BMAH.xlsx";

            saveFileDialog.FileName = serialVal;
            if (saveFileDialog.ShowDialog() == true)
            {
                wb.SaveAs(saveFileDialog.FileName);
                wb.Dispose();
                return;
            }
        }
    }
}