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
    
    public partial class DeliveryCustomerReceipt
    {
        public string DeliveryCustomerReceiptID { get; set; }
        public string CartID { get; set; }
        public string CustomerName { get; set; }
        public Nullable<int> CustomerPhone { get; set; }
        public string CustomerAddress { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentData { get; set; }
    
        public virtual Cart Cart { get; set; }
    }
}
