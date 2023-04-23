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
    
    public partial class C_User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public C_User()
        {
            this.AgentAccounts = new HashSet<AgentAccount>();
            this.CustomerAccounts = new HashSet<CustomerAccount>();
            this.StaffAccounts = new HashSet<StaffAccount>();
            this.WareHouseReceipts = new HashSet<WareHouseReceipt>();
        }
    
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserPhone { get; set; }
        public string UserAddress { get; set; }
        public Nullable<bool> UserDeleted { get; set; }
    
        public virtual Agent Agent { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AgentAccount> AgentAccounts { get; set; }
        public virtual Customer Customer { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CustomerAccount> CustomerAccounts { get; set; }
        public virtual Staff Staff { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StaffAccount> StaffAccounts { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WareHouseReceipt> WareHouseReceipts { get; set; }
    }
}
