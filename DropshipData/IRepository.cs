using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DropshipCommon.Models;
using System.Linq.Expressions;

namespace DropshipData
{
    public interface IRepository<T> where T : BaseEntity
    {
        T GetById(params object[] id);
        void Insert(T entity);
        void Update(T entity);
        void Delete(T entity);
        IQueryable<T> Table { get; }

        void Update(T entity, params Expression<Func<T, object>>[] properties);
        void Update(IEnumerable<T> entities, params Expression<Func<T, object>>[] properties);
        void Insert(IEnumerable<T> entities);
    }
}
