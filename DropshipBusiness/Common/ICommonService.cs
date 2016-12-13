using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DropshipCommon.Models;
using DropshipCommon;


namespace DropshipBusiness.Common
{
    public interface ICommonService
    {
        IList<T_Status> GetStatusByEntityType(DropshipEntityType entityType);

        IList<T_Supplier> GetAllSuppliers();
    }
}
