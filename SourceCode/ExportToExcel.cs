using ClosedXML.Excel;
using Microsoft.Win32;
using System;

namespace BMAH_WoM.SourceCode
{
    public class ExportToExcel
    {
        public void ToExcel()
        {
            var wb = new XLWorkbook();

            //prepare time string
            DateTime localDate = DateTime.Now;
            string timestamp = localDate.ToString();

            //define Table and it's child names
            var ws = wb.Worksheets.Add("WoM-BMAH scan");

            ws.Cell("B2").Value = "WoM-BMAH " + timestamp;
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

            //format title cell
            rngTable.Cell(1, 1).Style.Font.Bold = true;
            rngTable.Cell(1, 1).Style.Fill.BackgroundColor = XLColor.CornflowerBlue;
            rngTable.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngTable.Row(1).Merge();

            //header customization
            var rngHeaders = rngTable.Range("B3:J3");   //The address is relative to rngTable (NOT the worksheet)
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