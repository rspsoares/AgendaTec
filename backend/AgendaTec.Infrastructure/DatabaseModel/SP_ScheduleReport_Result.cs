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
    
    public partial class SP_ScheduleReport_Result
    {
        public int IDCustomer { get; set; }
        public System.DateTime Date { get; set; }
        public string CustomerName { get; set; }
        public string ServiceDescription { get; set; }
        public decimal Price { get; set; }
        public System.DateTime Date1 { get; set; }
        public bool Attended { get; set; }
    }
}
