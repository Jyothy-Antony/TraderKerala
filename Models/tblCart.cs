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
    
    public partial class tblCart
    {
        public int id { get; set; }
        public string Userid { get; set; }
        public Nullable<int> ProdId { get; set; }
        public string Image { get; set; }
        public Nullable<int> Qty { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public Nullable<decimal> Total { get; set; }
        public Nullable<decimal> Shipping { get; set; }
        public Nullable<decimal> Tax { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
    }
}