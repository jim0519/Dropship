using DropshipFramework.MVC;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Dropship.Models.Listing
{
    public class ListingGridViewModel : BaseEntityViewModel
    {
        public string ListingID { get; set; }
        public string ListingSKU { get; set; }
        public string ListingTitle { get; set; }
        //public string ListingDescription { get; set; }
        //public string ListingTitle { get; set; }
        //public string ListingTitle { get; set; }
        //public string ListingTitle { get; set; }
        public decimal ListingPrice { get; set; }
        public int ListingInventoryQty { get; set; }
        public string ListingStatus { get; set; }
        public string ItemTitle { get; set; }
        public decimal ItemPrice { get; set; }
        public int ItemInventoryQty { get; set; }
        public string ItemStatus { get; set; }
        public string ListingPostageRule { get; set; }
        public string ListingPriceRule { get; set; }
        public string ListingDescriptionTemplate { get; set; }
        [UIHint("DateTimeNullable")]
        public DateTime? LastUpdateTime { get; set; }
        public string Ref1 { get; set; }
        public string Ref2 { get; set; }
        public string Ref3 { get; set; }
        public string Ref4 { get; set; }
        public string Ref5 { get; set; }
        [UIHint("DateTimeNullable")]
        public DateTime? CreateTime { get; set; }
        public string CreateBy { get; set; }
        [UIHint("DateTimeNullable")]
        public DateTime? EditTime { get; set; }
        public string EditBy { get; set; }
    }
}