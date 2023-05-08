using Agent_WebForm_Prodject.Models;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Agent_WebForm_Prodject.Controllers
{
    public class CreateWarehouseReceiptController : Controller
    {
        // GET: CreateWarehouseReceipt
        public ActionResult Index()
        {
            WareHouseReceipt wareHouseReceipt = new WareHouseReceipt();
            List<WareHouseReceipt> res = wareHouseReceipt.SelectWarehouseReceiptQuery();
            ViewBag.WarehouseReceiptList = res;
            return View();
        }

        // GET: CreateWarehouseReceipt/Create
        public ActionResult Create()
        {
            List<string> countryList = new List<string>();
            CultureInfo[] getCultureInfo = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            foreach (CultureInfo getCulture in getCultureInfo)
            {
                RegionInfo getRegionInfo = new RegionInfo(getCulture.LCID);
                if (!countryList.Contains(getRegionInfo.EnglishName))
                {
                    countryList.Add(getRegionInfo.EnglishName);
                }
            }
            countryList.Sort();

            ViewBag.CountryList = countryList;
            return View();
        }

        // GET: CreateWarehouseReceipt/PrintWarehouseReceipt
        public ActionResult PrintWarehouseReceipt(
            string warehouseReceiptID,
            string staffId,
            DateTime importDate,
            decimal warehouseTotalBill)
        {
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
                PdfTextElement element = new PdfTextElement("WAREHOUSE RECEIPT " + warehouseReceiptID, subHeadingFont)
                {
                    Brush = PdfBrushes.White
                };

                // Draws the heading on the page
                PdfLayoutResult result = element.Draw(page, new PointF(10, bounds.Top + 8));
                string currentDate = "Import Date: " + importDate.ToString("yyyy-MM-dd HH:mm:ss");

                // Measures the width of the text to place it in the correct location
                SizeF textSize = subHeadingFont.MeasureString(currentDate);
                PointF textPosition = new PointF(graphics.ClientSize.Width - textSize.Width - 10, result.Bounds.Y);
                // Draws the date by using DrawString method
                graphics.DrawString(currentDate, subHeadingFont, element.Brush, textPosition);
                PdfFont timesRoman = new PdfStandardFont(PdfFontFamily.Helvetica, 10f);
                // Creates text elements to add the address and draw it to the page.
                element = new PdfTextElement("Staff ID: " + staffId, timesRoman)
                {
                    Brush = new PdfSolidBrush(new PdfColor(126, 155, 203))
                };
                result = element.Draw(page, new PointF(10, result.Bounds.Bottom + 25));
                element = new PdfTextElement("Total Bill: " + warehouseTotalBill, timesRoman)
                {
                    Brush = new PdfSolidBrush(new PdfColor(126, 155, 203))
                };
                result = element.Draw(page, new PointF(10, result.Bounds.Bottom + 25));

                PdfPen linePen = new PdfPen(new PdfColor(126, 151, 173), 0.70f);
                PointF startPoint = new PointF(0, result.Bounds.Bottom + 3);
                PointF endPoint = new PointF(graphics.ClientSize.Width, result.Bounds.Bottom + 3);
                // Draws a line at the bottom of the address
                graphics.DrawLine(linePen, startPoint, endPoint);

                // Creates the datasource for the table
                WareHouseReceiptDetail wareHouseReceiptDetail = new WareHouseReceiptDetail();
                DataTable productDetails = wareHouseReceiptDetail.GetWarehoueReceiptProductDetail(warehouseReceiptID);

                // Creates a PDF grid
                PdfGrid grid = new PdfGrid
                {
                    DataSource = productDetails
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

                document.Save("Warehouse Receipt_" + warehouseReceiptID + ".pdf", HttpContext.ApplicationInstance.Response, HttpReadType.Save);
                document.Close(true);
            }

            ViewBag.Message = "Print warehouse receipt " + warehouseReceiptID + " successfully";
            return View("Result");
        }

        [HttpPost]
        public ActionResult CreateWarehouseReceiptAsync()
        {
            WareHouseReceipt wareHouseReceipt = new WareHouseReceipt();
            string newWarehouseReceiptID = wareHouseReceipt.GetNewWarehouseReceiptID();
            wareHouseReceipt.AddWarehouseReceiptQuery(newWarehouseReceiptID, Session["StaffID"].ToString(), DateTime.Now, 0);

            decimal totalBill = 0;
            // Get the acutal number of groups of product's information
            int formCount = Request.Form.Count / 8;
            for (int i = 0; i < formCount; i++)
            {
                string productID = Request.Form["ProductID[" + i + "]"];
                string productName = Request.Form["ProductName[" + i + "]"];
                string productSize = Request.Form["ProductSize[" + i + "]"];
                string productUnitSize = Request.Form["ProductUnitSize[" + i + "]"];
                string productBrand = Request.Form["ProductBrand[" + i + "]"];
                string productOrigin = Request.Form["ProductOrigin[" + i + "]"];
                int productQuantity = Convert.ToInt32(Request.Form["ProductQuantity[" + i + "]"]);
                decimal productPrice = Convert.ToDecimal(Request.Form["ProductPrice[" + i + "]"]);
                totalBill += productPrice;

                Product currNewProduct = new Product();
                currNewProduct.AddProductQuery(productID, productName, productSize, productUnitSize, productBrand, productOrigin, productQuantity, productPrice);
                WareHouseReceiptDetail currNewReceiptDetail = new WareHouseReceiptDetail();
                currNewReceiptDetail.AddWarehouseReceiptDetailQuery(newWarehouseReceiptID, productID, productQuantity);
            }
            wareHouseReceipt.UpdateWarehouseReceiptQuery(newWarehouseReceiptID, "WarehouseTotalBill", totalBill.ToString());

            ViewBag.Message = "Create warehouse receipt successfully";
            return View("Result");
        }
    }
}