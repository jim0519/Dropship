using System;
using System.Collections.Generic;

namespace DropshipCommon.Models
{
    public partial class T_ListingChannel : BaseEntity
    {
        public T_ListingChannel()
        {
            this.Listings = new List<D_Listing>();
        }

        public int UserID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Ref1 { get; set; }
        public string Ref2 { get; set; }
        public string Ref3 { get; set; }
        public string Ref4 { get; set; }
        public string Ref5 { get; set; }
        public System.DateTime CreateTime { get; set; }
        public string CreateBy { get; set; }
        public System.DateTime EditTime { get; set; }
        public string EditBy { get; set; }

        public virtual T_User User { get; set; }
        public virtual ICollection<D_Listing> Listings { get; set; }
    }
}
