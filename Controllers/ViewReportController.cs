using Agent_WebForm_Prodject.Models;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Agent_WebForm_Prodject.Controllers
{
    public class ViewReportController : Controller
    {
        // GET: ViewReport
        public ActionResult Index()
        {
            List<string> monthNames = new List<string>();
            DateTimeFormatInfo dateTimeFormat = new DateTimeFormatInfo();
            for (int i = 1; i <= 12; i++)
            {
                monthNames.Add(dateTimeFormat.GetMonthName(i));
            }

            List<int> yearsNum = new List<int>();
            int currYear = DateTime.Now.Year;
            for (int i = currYear; i >= 1900; i--)
            {
                yearsNum.Add(i);
            }
            ViewBag.MonthList = monthNames;
            ViewBag.YearList = yearsNum;
            return View();
        }

        private int GetActualMonthNumber(string monthName)
        {
            switch (monthName)
            {
                case "January":
                    return 1;
                case "February":
                    return 2;
                case "March":
                    return 3;
                case "April":
                    return 4;
                case "May":
                    return 5;
                case "June":
                    return 6;
                case "July":
                    return 7;
                case "August":
                    return 8;
                case "September":
                    return 9;
                case "October":
                    return 10;
                case "November ":
                    return 11;
                case "December":
                    return 12;
                default:
                    return 12;
            }
        }

        [HttpPost]
        public ActionResult ReportViewResult()
        {
            string month = Request.Form["Month"];
            int yearNum = Convert.ToInt32(Request.Form["Year"]);
            int monthNum = GetActualMonthNumber(month);
            string reportType = Request.Form["ReportType"];

            WareHouseReceipt wareHouseReceipt = new WareHouseReceipt();
            DataTable res = new DataTable();
            string title = "";
            if (reportType.Equals("IncomingStock"))
            {
                res = wareHouseReceipt.GetImportProductByMonth(monthNum, yearNum);
                title = "INCOMING STOCK";
            }
            else if (reportType.Equals("OutgoingStock"))
            {
                res = wareHouseReceipt.GetExportProductByMonth(monthNum, yearNum);
                title = "OUTGOING STOCK";
            }
            else if (reportType.Equals("BestSellingReport"))
            {
                res = wareHouseReceipt.GetBestSellingProduct(monthNum, yearNum);
                title = "BEST-SELLING";
            }
            else if (reportType.Equals("RevenueByMonthReport"))
            {
                res = wareHouseReceipt.GetRevenueByMonth(monthNum, yearNum);
                title = "REVENUE IN " + month.ToUpper();
            }
            else if (reportType.Equals("RevenueMonthlyReport"))
            {
                res = wareHouseReceipt.GetRevenueMonthly();
                title = "REVENUE MONTHLY";
            }

            // Generate PDF file
            using (PdfDocument document = new PdfDocument())
            {
                //Adds page settings.
                document.PageSettings.Orientation = PdfPageOrientation.Landscape;
                document.PageSettings.Margins.All = 50;
                // Adds a page to the document.
                PdfPage page = document.Pages.Add();
                PdfGraphics graphics = page.Graphics;

                RectangleF bounds = new RectangleF(176, 0, 390, 130);
                PdfBrush solidBrush = new PdfSolidBrush(new PdfColor(126, 151, 173));
                bounds = new RectangleF(0, bounds.Bottom + 90, graphics.ClientSize.Width, 30);
                // Draws a rectangle to place the heading in that region.
                graphics.DrawRectangle(solidBrush, bounds);
                // Creates a font for adding the heading in the page
                PdfFont subHeadingFont = new PdfStandardFont(PdfFontFamily.TimesRoman, 14);
                //Creates a text element to add the invoice number
                PdfTextElement element = new PdfTextElement(title + " REPORT", subHeadingFont)
                {
                    Brush = PdfBrushes.White
                };

                // Draws the heading on the page
                PdfLayoutResult result = element.Draw(page, new PointF(10, bounds.Top + 8));
                string currentDate = "Date Created: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                // Measures the width of the text to place it in the correct location
                SizeF textSize = subHeadingFont.MeasureString(currentDate);
                PointF textPosition = new PointF(graphics.ClientSize.Width - textSize.Width - 10, result.Bounds.Y);
                // Draws the date by using DrawString method
                graphics.DrawString(currentDate, subHeadingFont, element.Brush, textPosition);
                PdfFont timesRoman = new PdfStandardFont(PdfFontFamily.Helvetica, 10f);
                // Creates text elements to add the address and draw it to the page.
                element = new PdfTextElement("Staff ID: " + Session["StaffID"], timesRoman)
                {
                    Brush = new PdfSolidBrush(new PdfColor(126, 155, 203))
                };
                result = element.Draw(page, new PointF(10, result.Bounds.Bottom + 25));
                PdfPen linePen = new PdfPen(new PdfColor(126, 151, 173), 0.70f);
                PointF startPoint = new PointF(0, result.Bounds.Bottom + 3);
                PointF endPoint = new PointF(graphics.ClientSize.Width, result.Bounds.Bottom + 3);
                // Draws a line at the bottom of the address
                graphics.DrawLine(linePen, startPoint, endPoint);

                // Creates a PDF grid
                PdfGrid grid = new PdfGrid
                {
                    DataSource = res
                };
                // Creates the grid cell styles
                PdfGridCellStyle cellStyle = new PdfGridCellStyle();
                cellStyle.Borders.All = PdfPens.White;
                PdfGridRow header = grid.Headers[0];
                // Creates the header style
                PdfGridCellStyle headerStyle = new PdfGridCellStyle();
                headerStyle.Borders.All = new PdfPen(new PdfColor(126, 151, 173));
                headerStyle.BackgroundBrush = new PdfSolidBrush(new PdfColor(126, 151, 173));
                headerStyle.TextBrush = PdfBrushes.White;
                headerStyle.Font = new PdfStandardFont(PdfFontFamily.Helvetica, 10f, PdfFontStyle.Regular);

                // Adds cell customizations
                for (int i = 0; i < header.Cells.Count; i++)
                {
                    if (i == 0 || i == 1)
                        header.Cells[i].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
                    else
                        header.Cells[i].StringFormat = new PdfStringFormat(PdfTextAlignment.Right, PdfVerticalAlignment.Middle);
                }

                // Applies the header style.
                header.ApplyStyle(headerStyle);
                cellStyle.Borders.Bottom = new PdfPen(new PdfColor(217, 217, 217), 0.70f);
                cellStyle.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 10f);
                cellStyle.TextBrush = new PdfSolidBrush(new PdfColor(131, 130, 136));
                // Creates the layout format for grid.
                PdfGridLayoutFormat layoutFormat = new PdfGridLayoutFormat
                {
                    Layout = PdfLayoutType.Paginate
                };
                // Draws the grid to the PDF page.
                PdfGridLayoutResult gridResult = grid.Draw(page, new RectangleF(new PointF(0, result.Bounds.Bottom + 40), new SizeF(graphics.ClientSize.Width, graphics.ClientSize.Height - 100)), layoutFormat);

                document.Save(title + " REPORT.pdf", HttpContext.ApplicationInstance.Response, HttpReadType.Save);
                document.Close(true);
            }

            ViewBag.Message = "Print " + title + " REPORT successfully";
            return View("Result");
        }

    }
}