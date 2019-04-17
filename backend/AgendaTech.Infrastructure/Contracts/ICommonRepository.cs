using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AgendaTech.Infrastructure.Contracts
{
    public interface ICommonRepository<T> where T : class
    {
        List<T> GetAll();        
        T GetById(int id);
        T Insert(T e);
        List<T> Filter(Expression<Func<T, bool>> predicate);
        void Update(int id, T e);
        void Delete(int id);      
    }
}
