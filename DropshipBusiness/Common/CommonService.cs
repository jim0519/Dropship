using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DropshipCommon.Models;
using DropshipData;
using DropshipCommon;

namespace DropshipBusiness.Common
{
    public class CommonService:ICommonService
    {
        private IRepository<T_Status> _statusRepository;
        private IRepository<T_Supplier> _supplierRepository;

        public CommonService(IRepository<T_Status> statusRepository,
            IRepository<T_Supplier> supplierRepository)
        {
            _statusRepository = statusRepository;
            _supplierRepository = supplierRepository;
        }

        public IList<T_Status> GetStatusByEntityType(DropshipEntityType entityType)
        {
            var strEntityType=entityType.ToString();
            var statusList = _statusRepository.Table.Where(s => s.EntityType.Equals(strEntityType));
            return statusList.ToList();
        }

        public IList<T_Supplier> GetAllSuppliers()
        {
            var supplierList = _supplierRepository.Table;
            return supplierList.ToList();

        }
    }
}
