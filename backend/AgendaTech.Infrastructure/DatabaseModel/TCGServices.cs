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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TCGServices()
        {
            this.TSchedules = new HashSet<TSchedules>();
        }
    
        public int IDService { get; set; }
        public int IDCustomer { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Time { get; set; }
    
        public virtual TCGCustomers TCGCustomers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TSchedules> TSchedules { get; set; }
    }
}