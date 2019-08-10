using AgendaTec.Infrastructure.Contracts;
using System;
using System.Linq;
using System.Transactions;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq.Expressions;
using AgendaTec.Infrastructure.DatabaseModel;

namespace AgendaTec.Infrastructure.Repositories
{
    public class CommonRepository<T> : ICommonRepository<T> where T : class
    {
        private readonly TransactionOptions _readNoLock = new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted };
        private readonly AgendaTecEntities _context = null;
        private DbSet<T> _table = null;

        public CommonRepository()
        {
            _context = new AgendaTecEntities();
            _table = _context.Set<T>();
        }

        public CommonRepository(AgendaTecEntities context)
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

        public void Update(object id, T e)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, _readNoLock))
            {
                var entity = GetById(id);
                _context.Entry(entity).CurrentValues.SetValues(e);
                _context.SaveChanges();

                scope.Complete();
            }
        }

        public void Delete(object id)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, _readNoLock))
            {
                var existing = _table.Find(id);

                if(existing != null)
                {
                    _table.Remove(existing);
                    _context.SaveChanges();
                }

                scope.Complete();
            }
        }
    }
}
