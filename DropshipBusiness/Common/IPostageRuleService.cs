using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DropshipCommon.Models;
using DropshipCommon;


namespace DropshipBusiness.Common
{
    public interface IPostageRuleService
    {
        T_PostageRule InsertPostageRule(T_PostageRule rule);

        T_PostageRule UpdatePostageRule(T_PostageRule rule);

        void DeletePostageRule(T_PostageRule rule);

        T_PostageRule GetPostageRuleByName(string name);
    }
}
