//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TradeKerala2017.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblProdImage
    {
        public int id { get; set; }
        public Nullable<int> Prodid { get; set; }
        public string Image { get; set; }
        public Nullable<int> position { get; set; }
    
        public virtual tblProduct tblProduct { get; set; }
    }
}
