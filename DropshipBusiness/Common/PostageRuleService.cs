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
    public class PostageRuleService:IPostageRuleService
    {
        private IRepository<T_PostageRule> _postageRuleRepository;

        public PostageRuleService(IRepository<T_PostageRule> postageRuleRepository)
        {
            _postageRuleRepository = postageRuleRepository;
        }



        public T_PostageRule InsertPostageRule(T_PostageRule rule)
        {
            if (rule != null)
                _postageRuleRepository.Insert(rule);

            return rule;
        }

        public T_PostageRule UpdatePostageRule(T_PostageRule rule)
        {
            if (rule != null)
                _postageRuleRepository.Update(rule);

            return rule;
        }

        public void DeletePostageRule(T_PostageRule rule)
        {
            if (rule != null)
                _postageRuleRepository.Delete(rule);
        }


        public T_PostageRule GetPostageRuleByName(string name)
        {
            return _postageRuleRepository.Table.FirstOrDefault(r=>r.Name.Equals(name));
        }
    }
}
