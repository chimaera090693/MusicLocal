using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using music.local.Models;

namespace music.local.Repository
{
    public abstract class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DbSet<T> _dbset;
        private readonly IUnitOfWork _entities;


        protected GenericRepository(IUnitOfWork entities)
        {
            _entities = entities;
            _dbset = _entities.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await Task.Run(() => _dbset.AsEnumerable<T>());
        }

        public async Task<IEnumerable<T>> FindBy(Expression<Func<T, bool>> predicate)
        {
            return await Task.Run(() => (IEnumerable<T>)_dbset.Where(predicate).AsEnumerable());
        }

        public async Task<T> Create(T entity)
        {
            _dbset.Add(entity);
            Save();
            return await Task.Run(() => entity);
        }
        public async Task<int> Update(T entity)
        {
            //_entities.Set<Entity>();
            return await Task.Run(() => Save());

        }
        public async Task<int> Delete(T entity)
        {
            _dbset.Remove(entity);
            return await Task.Run(() => Save());
        }
       
        public int Save()
        {
            try
            {
                return _entities.Commit();
            }
            catch (Exception ex)
            {

                return -1;
                //return _entities.Commit();
            }
          
        }
    }
}
