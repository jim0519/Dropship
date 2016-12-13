using System;
using System.Collections.Generic;

namespace DropshipCommon.Models
{
    public partial class D_Image:BaseEntity
    {
        public D_Image()
        {
            this.ItemImages = new List<M_ItemImage>();
        }

        public string ImagePath { get; set; }
        public System.DateTime CreateTime { get; set; }
        public string CreateBy { get; set; }
        public System.DateTime EditTime { get; set; }
        public string EditBy { get; set; }
        public virtual ICollection<M_ItemImage> ItemImages { get; set; }
    }
}
