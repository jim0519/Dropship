using DropshipFramework.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dropship.Models.Item
{
    public class ItemGridViewModel : BaseEntityViewModel
    {
        public string SKU { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        //public string ListingTitle { get; set; }
        //public string ListingTitle { get; set; }
        public decimal Price { get; set; }
        public int InventoryQty { get; set; }
        public string Status { get; set; }
        public string Supplier { get; set; }
        public string Ref1 { get; set; }
        public string Ref2 { get; set; }
        public string Ref3 { get; set; }
        public string Ref4 { get; set; }
        public string Ref5 { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateBy { get; set; }
        public DateTime EditTime { get; set; }
        public string EditBy { get; set; }
    }
}