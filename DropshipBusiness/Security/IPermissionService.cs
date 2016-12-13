using DropshipCommon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropshipBusiness.Security
{
    public interface IPermissionService
    {
        void InsertPermission(T_Permission permission);

        void UpdatePermission(T_Permission permission);

        bool Authorize(string permissionRecordSystemName);
    }
}
