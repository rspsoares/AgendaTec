//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AgendaTec.Infrastructure.DatabaseModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class TCGCustomersAspNetUsers
    {
        public int IDCustomersAspNetUsers { get; set; }
        public string IDAspNetUsers { get; set; }
        public int IDCustomer { get; set; }
    
        public virtual AspNetUsers AspNetUsers { get; set; }
        public virtual TCGCustomers TCGCustomers { get; set; }
    }
}