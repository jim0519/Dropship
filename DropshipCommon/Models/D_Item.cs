using System;
using System.Collections.Generic;

namespace DropshipCommon.Models
{
    public partial class D_Item : BaseEntity
    {
        public D_Item()
        {
            this.ItemImages = new List<M_ItemImage>();
        }

        public string SKU { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int InventoryQty { get; set; }
        public int StatusID { get; set; }
        public int PostageRuleID { get; set; }
        public int SupplierID { get; set; }
        public string SupplierItemID { get; set; }
        public string Ref1 { get; set; }
        public string Ref2 { get; set; }
        public string Ref3 { get; set; }
        public string Ref4 { get; set; }
        public string Ref5 { get; set; }
        public System.DateTime CreateTime { get; set; }
        public string CreateBy { get; set; }
        public System.DateTime EditTime { get; set; }
        public string EditBy { get; set; }
        public virtual ICollection<M_ItemImage> ItemImages { get; set; }
    }
}
