using System;
using System.Collections.Generic;

namespace DropshipCommon.Models
{
    public partial class T_PostageRuleLine : BaseEntity
    {
        public int PostageRuleID { get; set; }
        public string PostcodeFrom { get; set; }
        public string PostcodeTo { get; set; }
        public string Formula { get; set; }
        public System.DateTime CreateTime { get; set; }
        public string CreateBy { get; set; }
        public System.DateTime EditTime { get; set; }
        public string EditBy { get; set; }
        public virtual T_PostageRule T_PostageRule { get; set; }
    }
}
