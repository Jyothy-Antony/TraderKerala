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
    
    public partial class tbl_cart
    {
        public int id { get; set; }
        public string user_id { get; set; }
        public Nullable<int> prod_id { get; set; }
        public Nullable<int> cat_id { get; set; }
        public string pro_name { get; set; }
        public string pro_price { get; set; }
        public string pro_image { get; set; }
        public string pro_qty { get; set; }
        public string sub_total { get; set; }
        public string total { get; set; }
        public string Type { get; set; }
        public string Discount { get; set; }
        public string Tax { get; set; }
        public string shipping { get; set; }
        public string Gift_wrap { get; set; }
        public Nullable<int> address_id { get; set; }
    }
}
