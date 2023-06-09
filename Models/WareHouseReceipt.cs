//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Agent_WebForm_Prodject.Models
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;

    public partial class WareHouseReceipt
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public WareHouseReceipt()
        {
            this.WareHouseReceiptDetails = new HashSet<WareHouseReceiptDetail>();
        }
    
        public string WarehouseReceiptID { get; set; }
        public string StaffID { get; set; }
        public Nullable<System.DateTime> ImportDate { get; set; }
        public Nullable<decimal> WarehouseTotalBill { get; set; }
        public Nullable<bool> WarehouseDeleted { get; set; }
    
        public virtual C_User C_User { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WareHouseReceiptDetail> WareHouseReceiptDetails { get; set; }

        public List<WareHouseReceipt> SelectWarehouseReceiptQuery()
        {
            List<WareHouseReceipt> res = new List<WareHouseReceipt>();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DBConn"].ToString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("select * from WarehouseReceipt", conn);

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    WareHouseReceipt wareHouseReceipt = new WareHouseReceipt();
                    wareHouseReceipt.WarehouseReceiptID = dr["WarehouseReceiptID"].ToString();
                    wareHouseReceipt.StaffID = dr["StaffID"].ToString();
                    wareHouseReceipt.ImportDate = Convert.ToDateTime(dr["ImportDate"]);
                    wareHouseReceipt.WarehouseTotalBill = Convert.ToDecimal(dr["WarehouseTotalBill"]);
                    res.Add(wareHouseReceipt);
                }
            }
            return res;
        }

        public void AddWarehouseReceiptQuery(
            string WarehouseReceiptID,
            string StaffID,
            DateTime ImportDate,
            decimal WarehouseReceiptTotalBill)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DBConn"].ToString()))
            {
                conn.Open();
                string inportDate = ImportDate.ToString("yyyy-MM-dd HH:mm:ss");
                string sql = "insert into WarehouseReceipt values ('" +
                    WarehouseReceiptID +
                    "', '" + StaffID +
                    "', '" + inportDate +
                    "', " + WarehouseReceiptTotalBill +
                    ", " + 0 + ")";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateWarehouseReceiptQuery(string WarehouseReceiptID, string attribute, string value)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DBConn"].ToString()))
            {
                conn.Open();
                string sql = "";
                if (attribute == "WarehouseTotalBill")
                {
                    sql = "update WarehouseReceipt set" +
                    " WarehouseTotalBill = " + Convert.ToDecimal(value) +
                    " where WarehouseReceiptID = '" + WarehouseReceiptID + "'";
                }
                else
                {
                    sql = "update WarehouseReceipt set" +
                    " " + attribute + " = '" + value +
                    "' where WarehouseReceiptID = '" + WarehouseReceiptID + "'";
                }
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
        }

        private string GetWarehouseReceiptDesc()
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DBConn"].ToString()))
            {
                conn.Open();
                string sql = "select top 1 WarehouseReceiptID from WarehouseReceipt order by WarehouseReceiptID desc";
                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();
                string res = "";
                while (dr.Read())
                {
                    res = dr["WarehouseReceiptID"].ToString();
                }
                return res;
            }
        }

        public string GetNewWarehouseReceiptID()
        {
            string res = GetWarehouseReceiptDesc();
            if (res != null && !res.Equals(""))
            {
                int order = int.Parse(res.Substring(4)) + 1;
                if (order < 10)
                {
                    res = "WHMP00000" + order.ToString();
                }
                else if (order < 100)
                {
                    res = "WHMP0000" + order.ToString();
                }
                else if (order < 1000)
                {
                    res = "WHMP000" + order.ToString();
                }
                else if (order < 10000)
                {
                    res = "WHMP00" + order.ToString();
                }
                else if (order < 100000)
                {
                    res = "WHMP0" + order.ToString();
                }
                else
                {
                    res = "WHMP" + order.ToString();
                }
                return res;
            }
            else
            {
                return "WHMP000001";
            }
        }

        public DataTable GetImportProductByMonth(int month, int year)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DBConn"].ToString()))
            {
                string sql = "select P.ProductID, P.ProductName, P.ProductSize, P.ProductUnitSize, P.ProductBrand, P.ProductOrigin, P.ProductPrice, WD.Quantity" +
                            " from WareHouseReceiptDetail WD, Product P" +
                            " where WD.WareHouseReceiptID IN(" +
                                " select W.WarehouseReceiptID" +
                                " from WareHouseReceipt W" +
                                " where Month(W.ImportDate) = " + month +
                                " and Year(W.ImportDate) = " + year +
                            " ) and WD.ProductID = P.ProductID";
                SqlDataAdapter dataAdapter = new SqlDataAdapter(sql, conn);
                DataTable res = new DataTable();
                dataAdapter.Fill(res);
                return res;
            }
        }

        public DataTable GetExportProductByMonth(int month, int year)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DBConn"].ToString()))
            {
                string sql = "select P.ProductID, P.ProductName, P.ProductSize, P.ProductUnitSize, P.ProductBrand, P.ProductOrigin, P.ProductPrice, O.Quantity" +
                        " from OrderDetail O, Product P" +
                        " where O.OrderID IN (" +
                            " select D.OrderID" +
                            " from DeliverySlip D" +
                            " where Month(D.DeliveryDate) = " + month +
                            " and Year(D.DeliveryDate) = " + year +
                        " ) and O.ProductID = P.ProductID";
                SqlDataAdapter dataAdapter = new SqlDataAdapter(sql, conn);
                DataTable res = new DataTable();
                dataAdapter.Fill(res);
                return res;
            }
        }

        public DataTable GetBestSellingProduct(int month, int year)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DBConn"].ToString()))
            {
                string sql = "select top(3) P.ProductID, P.ProductName, P.ProductSize, P.ProductUnitSize, P.ProductBrand, P.ProductOrigin, P.ProductPrice, SUM(O.Quantity) AS TotalQuantity" +
                        " from OrderDetail O, Product P" +
                        " where O.OrderID IN ( select D.OrderID from DeliverySlip D where Month(D.DeliveryDate) = " + month + " and Year(D.DeliveryDate) = " + year + ") and O.ProductID = P.ProductID" +
                        " group by P.ProductID, P.ProductName, P.ProductSize, P.ProductUnitSize, P.ProductBrand, P.ProductOrigin, P.ProductPrice" +
                        " order by SUM(O.Quantity) DESC";
                SqlDataAdapter dataAdapter = new SqlDataAdapter(sql, conn);
                DataTable res = new DataTable();
                dataAdapter.Fill(res);
                return res;
            }
        }

        public DataTable GetRevenueByMonth(int month, int year)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DBConn"].ToString()))
            {
                string sql = "select M.MonthName AS 'Month', Year(D.DeliveryDate) AS 'Year', SUM(D.TotalBill) AS 'Revenue'" +
                       " from DeliverySlip D, __Months M" +
                       " where Month(D.DeliveryDate) = " + month + " and Month(D.DeliveryDate) = M.MonthNumber" +
                       " and Year(D.DeliveryDate) = " + year +
                       " group by M.MonthName, Year(D.DeliveryDate)";
                SqlDataAdapter dataAdapter = new SqlDataAdapter(sql, conn);
                DataTable res = new DataTable();
                dataAdapter.Fill(res);
                return res;
            }
        }

        public DataTable GetRevenueMonthly()
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DBConn"].ToString()))
            {
                string sql = "select M.MonthName AS 'Month', ISNULL(SUM(D.TotalBill), 0) AS 'Revenue'" +
                       " from __Months M LEFT JOIN DeliverySlip D ON Month(D.DeliveryDate) = M.MonthNumber" +
                       " group by M.MonthName";
                SqlDataAdapter dataAdapter = new SqlDataAdapter(sql, conn);
                DataTable res = new DataTable();
                dataAdapter.Fill(res);
                return res;
            }
        }
    }
}
