using System;
using System.Collections.Generic;

namespace DropshipCommon.Models
{
    public partial class T_ValueRuleLine : BaseEntity
    {

        public int ValueRuleID { get; set; }
        public string FieldName { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public string Formula { get; set; }
        public System.DateTime CreateTime { get; set; }
        public string CreateBy { get; set; }
        public System.DateTime EditTime { get; set; }
        public string EditBy { get; set; }
        public virtual T_ValueRule T_ValueRule { get; set; }
    }
}
