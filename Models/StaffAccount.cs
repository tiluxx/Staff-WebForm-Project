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
    
    public partial class StaffAccount
    {
        public string StaffACID { get; set; }
        public string StaffID { get; set; }
    
        public virtual C_User C_User { get; set; }
        public virtual UserAccount UserAccount { get; set; }
    }
}
