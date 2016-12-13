using System;
using System.Collections.Generic;

namespace DropshipCommon.Models
{
    public partial class T_Role : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int StatusID { get; set; }
        public System.DateTime CreateTime { get; set; }
        public string CreateBy { get; set; }
        public System.DateTime EditTime { get; set; }
        public string EditBy { get; set; }

        public virtual ICollection<M_UserRole> UserRoles { get; set; }
        public virtual ICollection<M_RolePermission> RolePermissions { get; set; }
    }
}
