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

    public partial class OrderDetail
    {
        public string OrderID { get; set; }
        public string ProductID { get; set; }
        public Nullable<int> Quantity { get; set; }
    
        public virtual C_Order C_Order { get; set; }
        public virtual Product Product { get; set; }

        public DataTable GetOrderProductByOrderID(string orderId)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DBConn"].ToString()))
            {
                string sql = "select P.ProductID, P.ProductName, P.ProductSize, P.ProductUnitSize, P.ProductBrand, P.ProductOrigin, P.ProductPrice, O.Quantity as 'OrderQuantity'" +
                    " from OrderDetail O, Product P where O.OrderID = '" + orderId + "' and O.ProductID = P.ProductID";
                SqlDataAdapter dataAdapter = new SqlDataAdapter(sql, conn);
                DataTable res = new DataTable();
                dataAdapter.Fill(res);
                return res;
            }
        }
    }
}
