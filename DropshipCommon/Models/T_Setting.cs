using System;
using System.Collections.Generic;

namespace DropshipCommon.Models
{
    public partial class T_Setting : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public System.DateTime CreateTime { get; set; }
        public string CreateBy { get; set; }
        public System.DateTime EditTime { get; set; }
        public string EditBy { get; set; }
    }
}
