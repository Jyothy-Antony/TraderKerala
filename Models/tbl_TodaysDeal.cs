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
    
    public partial class tbl_TodaysDeal
    {
        public int Id { get; set; }
        public Nullable<int> Pid { get; set; }
        public string Offer_Name { get; set; }
        public string ProdName { get; set; }
        public Nullable<int> Discount { get; set; }
        public Nullable<decimal> NewPrice { get; set; }
        public Nullable<decimal> DollarPrice { get; set; }
    }
}
