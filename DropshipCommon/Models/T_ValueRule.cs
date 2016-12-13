using System;
using System.Collections.Generic;

namespace DropshipCommon.Models
{
    public partial class T_ValueRule : BaseEntity
    {
        public T_ValueRule()
        {
            this.T_ValueRuleLine = new List<T_ValueRuleLine>();
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public System.DateTime CreateTime { get; set; }
        public string CreateBy { get; set; }
        public System.DateTime EditTime { get; set; }
        public string EditBy { get; set; }
        public virtual ICollection<T_ValueRuleLine> T_ValueRuleLine { get; set; }
    }
}
