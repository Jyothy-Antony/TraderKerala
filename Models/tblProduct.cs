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
    
    public partial class tblProduct
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblProduct()
        {
            this.tblProdImage = new HashSet<tblProdImage>();
        }
    
        public int pid { get; set; }
        public string ProdName { get; set; }
        public Nullable<int> Catid { get; set; }
        public string CatName { get; set; }
        public Nullable<int> Vendor { get; set; }
        public Nullable<decimal> ProdNewPrice { get; set; }
        public Nullable<decimal> ProdOldPrice { get; set; }
        public Nullable<decimal> Dollar_NewPrice { get; set; }
        public Nullable<decimal> Dollar_OldPrice { get; set; }
        public string ProdTitle { get; set; }
        public string ProdDesc { get; set; }
        public string ProdInfo { get; set; }
        public string Brand { get; set; }
        public Nullable<int> Qty { get; set; }
        public string Color { get; set; }
        public string Weight { get; set; }
        public string Size { get; set; }
        public string Height { get; set; }
        public string Width { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<bool> Bestsell { get; set; }
        public Nullable<bool> Isoffer { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public string IsRelatedTo { get; set; }
        public string ParCategories { get; set; }
        public string Organic { get; set; }
        public string Hide { get; set; }
    
        public virtual tblCategory tblCategory { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblProdImage> tblProdImage { get; set; }
    }
}
