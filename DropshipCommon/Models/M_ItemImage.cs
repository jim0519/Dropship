using System;
using System.Collections.Generic;

namespace DropshipCommon.Models
{
    public partial class M_ItemImage : BaseEntity
    {
        public int ItemID { get; set; }
        public int ImageID { get; set; }
        public int DisplayOrder { get; set; }
        public int StatusID { get; set; }
        public System.DateTime CreateTime { get; set; }
        public string CreateBy { get; set; }
        public System.DateTime EditTime { get; set; }
        public string EditBy { get; set; }
        public virtual D_Image Image { get; set; }
        public virtual D_Item Item { get; set; }
    }
}
