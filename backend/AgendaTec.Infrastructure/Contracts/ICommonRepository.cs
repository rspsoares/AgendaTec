using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AgendaTec.Infrastructure.Contracts
{
    public interface ICommonRepository<T> where T : class
    {
        List<T> GetAll();        
        T GetById(object id);
        T Insert(T e);
        List<T> Filter(Expression<Func<T, bool>> predicate);
        void Update(object id, T e);
        void Delete(object id);
    }
}
