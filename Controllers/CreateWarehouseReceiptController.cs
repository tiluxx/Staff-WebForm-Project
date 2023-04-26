using Agent_WebForm_Prodject.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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

        [HttpPost]
        public ActionResult CreateWarehouseReceiptAsync()
        {
            WareHouseReceipt wareHouseReceipt = new WareHouseReceipt();
            string newWarehouseReceiptID = wareHouseReceipt.GetNewWarehouseReceiptID();
            wareHouseReceipt.AddWarehouseReceiptQuery(newWarehouseReceiptID, Session["StaffID"].ToString(), DateTime.Now, 0);

            Decimal totalBill = 0;
            for (int i = 0; i <= Request.Form.Count; i++)
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
            wareHouseReceipt.UpdateWarehouseReceiptQuery(newWarehouseReceiptID, "TotalBill", totalBill.ToString());

            return View("Index");
        }
    }
}