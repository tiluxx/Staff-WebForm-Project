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
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Configuration;
    using System.Data.SqlClient;

    public partial class UserAccount
    {
        [DisplayName("Username")]
        [Required(ErrorMessage="Please enter your username")]
        public string UserName { get; set; }
        [DisplayName("Password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Please enter your password")]
        public string UserPassword { get; set; }
        public Nullable<bool> Activated { get; set; }
        public Nullable<bool> UserDeleted { get; set; }

        public string LoginMessageError { get; set; }
    
        public virtual AgentAccount AgentAccount { get; set; }
        public virtual CustomerAccount CustomerAccount { get; set; }
        public virtual StaffAccount StaffAccount { get; set; }

        public List<UserAccount> SelectAgentAcByIDQuery(string agentID)
        {
            List<UserAccount> res = new List<UserAccount>();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DBConn"].ToString()))
            {
                conn.Open();
                string sql = "select U.UserName as 'AgentUsername', U.UserPassword as 'AgentPassword', U.Activated as 'Activation'" +
                " from AgentAccount A, UserAccount U where A.AgentID = '" + agentID + "' and A.AgentACID = U.UserName";
                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    UserAccount agentAccount = new UserAccount();
                    agentAccount.UserName = dr["AgentUsername"].ToString();
                    agentAccount.UserPassword = dr["AgentPassword"].ToString();
                    agentAccount.Activated = Convert.ToInt32(dr["Activation"]) == 0 ? false : true;
                    res.Add(agentAccount);
                }
                conn.Close();
            }
            return res;
        }
    }
}
