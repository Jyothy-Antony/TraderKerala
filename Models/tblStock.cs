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
    
    public partial class tblStock
    {
        public int id { get; set; }
        public Nullable<int> ProductId { get; set; }
        public string Product { get; set; }
        public string Category { get; set; }
        public Nullable<int> CategoryId { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<decimal> Dollar__Price { get; set; }
        public string Brand { get; set; }
        public Nullable<int> Qty { get; set; }
        public Nullable<double> Discount { get; set; }
        public Nullable<decimal> Totalamount { get; set; }
        public Nullable<decimal> DollarAmount { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string Type { get; set; }
    }
}
