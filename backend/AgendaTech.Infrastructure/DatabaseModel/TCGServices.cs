//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AgendaTech.Infrastructure.DatabaseModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class TCGServices
    {
        public int IDService { get; set; }
        public int IDCustomer { get; set; }
        public string Description { get; set; }
        public decimal Value { get; set; }
        public int Time { get; set; }
    
        public virtual TCGCustomers TCGCustomers { get; set; }
    }
}
