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
    
    public partial class TCGUserGroup
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TCGUserGroup()
        {
            this.UserAccounts = new HashSet<UserAccounts>();
        }
    
        public int IDUserGroup { get; set; }
        public string Descripton { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserAccounts> UserAccounts { get; set; }
    }
}