using System;
using System.Collections.Generic;

namespace DropshipCommon.Models
{
    public partial class D_Listing : BaseEntity
    {
        public int ItemID { get; set; }
        public int ListingChannelID { get; set; }
        //public int UserID { get; set; }
        public string ListingID { get; set; }
        public string ListingSKU { get; set; }
        public string ListingTitle { get; set; }
        public string ListingDescription { get; set; }
        public decimal ListingPrice { get; set; }
        public int ListingInventoryQty { get; set; }
        public int ListingStatusID { get; set; }
        public int ListingPriceRuleID { get; set; }
        public int ListingInventoryQtyRuleID { get; set; }
        public int ListingPostageRuleID { get; set; }
        public int ListingDescriptionTemplateID { get; set; }
        public System.DateTime LastUpdateTime { get; set; }
        public string Ref1 { get; set; }
        public string Ref2 { get; set; }
        public string Ref3 { get; set; }
        public string Ref4 { get; set; }
        public string Ref5 { get; set; }
        public System.DateTime CreateTime { get; set; }
        public string CreateBy { get; set; }
        public System.DateTime EditTime { get; set; }
        public string EditBy { get; set; }

        public virtual T_ListingChannel ListingChannel { get; set; }
        public virtual D_Item Item { get; set; }
        public virtual T_ValueRule ListingInventoryQtyRule { get; set; }
        public virtual T_ValueRule ListingPriceRule { get; set; }
        public virtual T_PostageRule ListingPostageRule { get; set; }
    }
}
