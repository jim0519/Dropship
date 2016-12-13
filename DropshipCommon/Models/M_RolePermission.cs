using System;
using System.Collections.Generic;

namespace DropshipCommon.Models
{
    public partial class M_RolePermission : BaseEntity
    {
        public int RoleID { get; set; }
        public int PermissionID { get; set; }
        public System.DateTime CreateTime { get; set; }
        public string CreateBy { get; set; }
        public System.DateTime EditTime { get; set; }
        public string EditBy { get; set; }

        public virtual T_Permission Permission { get; set; }
        public virtual T_Role Role { get; set; }
    }
}
