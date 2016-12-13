using System;
using System.Collections.Generic;

namespace DropshipCommon.Models
{
    public partial class M_UserRole : BaseEntity
    {
        public int UserID { get; set; }
        public int RoleID { get; set; }
        public System.DateTime CreateTime { get; set; }
        public string CreateBy { get; set; }
        public System.DateTime EditTime { get; set; }
        public string EditBy { get; set; }

        public virtual T_User User { get; set; }
        public virtual T_Role Role { get; set; }
    }
}
