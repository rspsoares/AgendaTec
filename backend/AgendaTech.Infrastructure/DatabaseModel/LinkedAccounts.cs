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
    
    public partial class LinkedAccounts
    {
        public int Key { get; set; }
        public int ParentKey { get; set; }
        public string ProviderName { get; set; }
        public string ProviderAccountID { get; set; }
        public System.DateTime LastLogin { get; set; }
    
        public virtual UserAccounts UserAccounts { get; set; }
    }
}