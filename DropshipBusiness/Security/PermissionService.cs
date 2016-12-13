using DropshipBusiness.eBay;
using DropshipBusiness.Item;
using DropshipCommon.Models;
using DropshipData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DropshipCommon;
using eBay.Service.Core.Soap;

namespace DropshipBusiness.Security
{
    public class PermissionService : IPermissionService
    {
        private IRepository<T_Permission> _permissionRepository;


        public PermissionService(IRepository<T_Permission> permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }


        public void InsertPermission(T_Permission permission)
        {
            throw new NotImplementedException();
        }

        public void UpdatePermission(T_Permission permission)
        {
            throw new NotImplementedException();
        }

        public bool Authorize(string permissionRecordSystemName)
        {
            throw new NotImplementedException();
        }
    }
}
