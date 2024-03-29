﻿using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Io;
using ClosedXML.Excel;
using Microsoft.Win32;  // needed for the saving dialog. need to be changed for Linux support.
using System;
using System.Diagnostics;
using System.Globalization;

// ReSharper disable StringLiteralTypo

namespace BMAH_WoM.SourceCode
{
    public class Scraper
    {
        public static async void ScrapeData()
        {
            var wb = new XLWorkbook();

            // preparing time string
            // don't be fooled - this really is the Pacific time zone,
            // not just standard time...
            var zone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            var utcNow = DateTime.UtcNow;
            var pacificNow = TimeZoneInfo.ConvertTimeFromUtc(utcNow, zone);
            var timestamp = pacificNow.ToString(CultureInfo.InvariantCulture);

            // define Table and it's child names
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

            // define ranges
            var rngTable = ws.Range("B2:J3");   // change the second value of the range to something adaptive based on the amount of data received

            // world of warcraft server array
            var wowservers = new[]
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
                "US-feathermoon",
                "US-earthen-ring",
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
                "US-moon-guard",
                "US-kelthuzad"
            };
            var rowCounter = 3;
            foreach (var wowserver in wowservers)
            {
                // user-agent
                var requester = new DefaultHttpRequester();
                requester.Headers["User-Agent"] = "Mozilla/5.0 (compatible; Googlebot/2.1; +http://www.google.com/bot.html)";

                var config = Configuration.Default
                    .With(requester)
                    .WithDefaultLoader()
                    .WithXPath()
                    .WithJs();
                var source = "https://tradeskillmaster.com/black-market?realm=" + wowserver;
                var document = await BrowsingContext.New(config).OpenAsync(source).WaitUntilAvailable();

                // table selector
                var rows = document.QuerySelectorAll("*[xpath>'//tbody/tr']");
                Debug.Print(wowserver);

                foreach (var row in rows)
                {
                    var emptyTsm = row.QuerySelector("*[xpath>'//td[1]']").TextContent;
                    if (emptyTsm == "No results found.")
                    {
                        rowCounter += 1;
                        rngTable = ws.Range("B2:J" + rowCounter);
                        Debug.Print("TSM has no data for this server at the moment, try again later!");
                        ws.Cell("B" + rowCounter).Value = wowserver;
                        ws.Cell("C" + rowCounter).Value = "No results found.";
                        ws.Cell("D" + rowCounter).Value = "N/A";
                        ws.Cell("E" + rowCounter).Value = "N/A";
                        ws.Cell("F" + rowCounter).Value = "N/A";
                        ws.Cell("G" + rowCounter).Value = "N/A";
                        ws.Cell("H" + rowCounter).Value = "N/A";
                        ws.Cell("I" + rowCounter).Value = "N/A";
                        ws.Cell("J" + rowCounter).Value = "N/A";
                    }
                    else
                    {
                        rowCounter += 1;
                        rngTable = ws.Range("B2:J" + rowCounter);
                        ws.Cell("B" + rowCounter).Value = wowserver;
                        var itemName = row.QuerySelector("*[xpath>'//td[1]/a[1]']").Attributes["title"].Value;
                        ws.Cell("C" + rowCounter).Value = itemName;
                        var currentBid = row.QuerySelector("*[xpath>'//td[2]']").TextContent;
                        ws.Cell("D" + rowCounter).Value = currentBid;
                        var minBid = row.QuerySelector("*[xpath>'//td[3]']").TextContent;
                        ws.Cell("E" + rowCounter).Value = minBid;
                        var timeLeft = row.QuerySelector("*[xpath>'//td[4]']").TextContent;
                        ws.Cell("F" + rowCounter).Value = timeLeft;
                        var nBids = row.QuerySelector("*[xpath>'//td[5]']").TextContent;
                        ws.Cell("G" + rowCounter).Value = nBids;
                        var realmMarket = row.QuerySelector("*[xpath>'//td[6]']").TextContent;
                        ws.Cell("H" + rowCounter).Value = realmMarket;
                        var globalMarket = row.QuerySelector("*[xpath>'//td[7]']").TextContent;
                        ws.Cell("I" + rowCounter).Value = globalMarket;
                        var realmAhQty = row.QuerySelector("*[xpath>'//td[8]']").TextContent;
                        ws.Cell("J" + rowCounter).Value = realmAhQty;

                        Debug.Print("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}", itemName, currentBid, minBid, timeLeft, nBids, realmMarket, globalMarket, realmAhQty);
                    }
                }
                rowCounter += 1;    // adds a blank row between different server rows
            }
            // format title cell
            rngTable.Cell(1, 1).Style.Font.Bold = true;
            rngTable.Cell(1, 1).Style.Fill.BackgroundColor = XLColor.CornflowerBlue;
            rngTable.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngTable.Row(1).Merge();

            // header customization
            var rngHeaders = rngTable.Range("B3:J3");
            rngHeaders.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngHeaders.Style.Font.Bold = true;
            rngHeaders.Style.Fill.BackgroundColor = XLColor.Aqua;

            // table customization
            ws.Columns(2, 10).AdjustToContents();
            rngTable.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
            rngTable.Style.Border.BottomBorder = XLBorderStyleValues.Thin;

            // save the Excel sheet
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel files|*.xlsx"
            };

            const string serialVal = "WoM - BMAH.xlsx";

            saveFileDialog.FileName = serialVal;
            if (saveFileDialog.ShowDialog() != true) return;
            wb.SaveAs(saveFileDialog.FileName);
            wb.Dispose();
        }
    }
}