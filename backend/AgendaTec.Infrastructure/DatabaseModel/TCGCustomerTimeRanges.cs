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
    
    public partial class TCGCustomerTimeRanges
    {
        public int IDCustomerTimeRanges { get; set; }
        public int IDCustomer { get; set; }
        public System.DateTime StartTime { get; set; }
        public System.DateTime EndTime { get; set; }
    
        public virtual TCGCustomers TCGCustomers { get; set; }
    }
}
