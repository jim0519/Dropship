using System;
using System.Collections.Generic;

namespace DropshipCommon.Models
{
    public partial class T_PostageRule : BaseEntity
    {
        public T_PostageRule()
        {
            this.T_PostageRuleLine = new List<T_PostageRuleLine>();
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public System.DateTime CreateTime { get; set; }
        public string CreateBy { get; set; }
        public System.DateTime EditTime { get; set; }
        public string EditBy { get; set; }
        public virtual ICollection<T_PostageRuleLine> T_PostageRuleLine { get; set; }
    }
}
