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
    
    public partial class TCGProfessionals
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TCGProfessionals()
        {
            this.TSchedules = new HashSet<TSchedules>();
        }
    
        public int IDProfessional { get; set; }
        public int IDCustomer { get; set; }
        public System.Guid IDUser { get; set; }
        public string Name { get; set; }
        public System.DateTime Birthday { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    
        public virtual TCGCustomers TCGCustomers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TSchedules> TSchedules { get; set; }
    }
}