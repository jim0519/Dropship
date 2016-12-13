using System;
using System.Collections.Generic;

namespace DropshipCommon.Models
{
    public partial class T_Category : BaseEntity
    {
        public string CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string ParentCategoryID { get; set; }
        public int SupplierID { get; set; }
        public int StatusID { get; set; }
        public System.DateTime CreateTime { get; set; }
        public string CreateBy { get; set; }
        public System.DateTime EditTime { get; set; }
        public string EditBy { get; set; }
    }
}
