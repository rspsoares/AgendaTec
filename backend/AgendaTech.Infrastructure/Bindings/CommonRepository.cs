using AgendaTech.Infrastructure.Contracts;
using System;
using System.Linq;
using System.Transactions;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq.Expressions;
using AgendaTech.Infrastructure.DatabaseModel;

namespace AgendaTech.Infrastructure.Repositories
{
    public class CommonRepository<T> : ICommonRepository<T> where T : class
    {
        private readonly TransactionOptions _readNoLock = new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted };
        private readonly AgendaTechEntities _context = null;
        private DbSet<T> _table = null;

        public CommonRepository()
        {
            _context = new AgendaTechEntities();
            _table = _context.Set<T>();
        }

        public CommonRepository(AgendaTechEntities context)
        {
            _context = context;
            _table = _context.Set<T>();
        }

        public List<T> GetAll()
        {
            var result = new List<T>();
            using (var scope = new TransactionScope(TransactionScopeOption.Required, _readNoLock))
            {
                result = _table.ToList();
                scope.Complete();
            }

            return result;
        }

        public T GetById(object id)
        {
            var result = default(T);
            result = Activator.CreateInstance<T>();

            using (var scope = new TransactionScope(TransactionScopeOption.Required, _readNoLock))
            {
                result = _table.Find(id);
                scope.Complete();
            }

            return result;
        }

        public List<T> Filter(Expression<Func<T, bool>> predicate)
        {
            var result = new List<T>();
            using (var scope = new TransactionScope(TransactionScopeOption.Required, _readNoLock))
            {
                result = _table.Where(predicate).ToList();
                scope.Complete();
            }

            return result;
        }
        
        public T Insert(T e)
        {   
            using (var scope = new TransactionScope(TransactionScopeOption.Required))
            {
                _table.Add(e);
                _context.SaveChanges();

                scope.Complete();
            }

            return e;
        }

        public void Update(T e)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required))
            {                
                _table.Attach(e);
                _context.Entry(e).State = EntityState.Modified;
                _context.SaveChanges();

                scope.Complete();
            }
        }

        public void Delete(object id)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, _readNoLock))
            {
                var existing = _table.Find(id);
                _table.Remove(existing);

                _context.SaveChanges();

                scope.Complete();
            }
        }       
    }
}
