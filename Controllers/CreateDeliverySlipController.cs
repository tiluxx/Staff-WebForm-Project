using Agent_WebForm_Prodject.Models;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Agent_WebForm_Prodject.Controllers
{
    public class CreateDeliverySlipController : Controller
    {
        // GET: CreateDeliverySlip
        public ActionResult Index()
        {
            C_Order order = new C_Order();
            List<C_Order> res = order.SelectOrderQuery();
            ViewBag.OrderList = res;
            return View();
        }

        // GET: CreateDeliverySlip/Create
        public ActionResult Create()
        {
            return View();
        }

        // GET: CreateDeliverySlip/DeliverySlipHistory
        public ActionResult DeliverySlipHistory()
        {
            DeliverySlip wareHouseReceipt = new DeliverySlip();
            List<DeliverySlip> res = wareHouseReceipt.SelectDeliverySlipQuery();
            ViewBag.DeliverySlipList = res;
            return View();
        }

        // GET: CreateDeliverySlip/UpdateOrder
        public ActionResult UpdateOrder(string orderId)
        {
            ViewBag.Data = orderId;
            return View();
        }

        public ActionResult PrintDeliverySlipByOrderId(string orderId, string orderStatus, string agentId)
        {
            DateTime currDateTime = DateTime.Now;
            // Update order status if needed
            if (orderStatus != "Delivery In Progress")
            {
                C_Order order = new C_Order();
                order.UpdateOrderStatusQuery(orderId, "Delivery In Progress");

                // Create delivery slip in database
                DeliverySlip deliverySlip = new DeliverySlip();
                string newDeliverySlipID = deliverySlip.GetNewDeliverySlipID();
                deliverySlip.AddDeliverySlipQuery(newDeliverySlipID, orderId, currDateTime, 0);
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
                PdfTextElement element = new PdfTextElement("ORDER " + orderId, subHeadingFont)
                {
                    Brush = PdfBrushes.White
                };

                // Draws the heading on the page
                PdfLayoutResult result = element.Draw(page, new PointF(10, bounds.Top + 8));
                string currentDate = "Date exported: " + currDateTime.ToString("yyyy-MM-dd HH:mm:ss");

                // Measures the width of the text to place it in the correct location
                SizeF textSize = subHeadingFont.MeasureString(currentDate);
                PointF textPosition = new PointF(graphics.ClientSize.Width - textSize.Width - 10, result.Bounds.Y);
                // Draws the date by using DrawString method
                graphics.DrawString(currentDate, subHeadingFont, element.Brush, textPosition);
                PdfFont timesRoman = new PdfStandardFont(PdfFontFamily.Helvetica, 10f);
                // Creates text elements to add the address and draw it to the page.
                element = new PdfTextElement("Agent " + agentId, timesRoman)
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
                OrderDetail orderDetail = new OrderDetail();
                DataTable productDetails = orderDetail.GetOrderProductByOrderID(orderId);

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

                document.Save("Delivery Slip_" + orderId + ".pdf", HttpContext.ApplicationInstance.Response, HttpReadType.Save);
                document.Close(true);
            }

            ViewBag.Message = "Print delivery slip of order " + orderId + " successfully";
            return View("Result");
        }

        [HttpPost]
        public ActionResult UpdateOrderById()
        {
            string orderStatus = Request.Form["OrderStatus"];
            string orderId = Request.Form["OrderId"];
            string paymentStatus = Request.Form["PaymentStatus"];

            // Update order status
            C_Order order = new C_Order();
            order.UpdateOrderStatusQuery(orderId, orderStatus);

            if (orderStatus.Equals("Delivery In Progress"))
            {
                DeliverySlip deliverySlip = new DeliverySlip();
                string newDeliverySlipID = deliverySlip.GetNewDeliverySlipID();
                deliverySlip.AddDeliverySlipQuery(newDeliverySlipID, orderId, DateTime.Now, 0);
            }

            // Update payment status
            if (paymentStatus.Equals("Payment Received") || paymentStatus.Equals("Completed"))
            {
                order.UpdateOrderPaymentStatusQuery(true, orderId, paymentStatus);
            }
            else
            {
                order.UpdateOrderPaymentStatusQuery(false, orderId, paymentStatus);
            }

            ViewBag.Message = "Update Order " + orderId + " successfully";
            return View("Result");
        }
    }
}