using EventTiming.Data.Repositories.Interfaces;
using EventTiming.Framework.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EventTiming.Data.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        protected DbContext _context;
        protected DbSet<TEntity> _dbSet;

        public IQueryable<TEntity> Records => _dbSet;

        public GenericRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public IEnumerable<TEntity> All()
        {
            return _dbSet.AsNoTracking().ToList();
        }

        public IEnumerable<TEntity> AllInclude
        (params Expression<Func<TEntity, object>>[] includeProperties)
        {
            return GetAllIncluding(false, includeProperties).ToList();
        }

        public IEnumerable<TEntity> FindByInclude
          (Expression<Func<TEntity, bool>> predicate,
          params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var query = GetAllIncluding(false, includeProperties);
            IEnumerable<TEntity> results = query.Where(predicate).ToList();
            return results;
        }

        public async Task<IEnumerable<TEntity>> FindByIncludeAsync
         (Expression<Func<TEntity, bool>> predicate,
         params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var query = GetAllIncluding(false, includeProperties);
            IEnumerable<TEntity> results = await query.Where(predicate).ToListAsync();
            return results;
        }

        private IQueryable<TEntity> GetAllIncluding
        (bool trackingEnabled, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> queryable = trackingEnabled ? _dbSet : _dbSet.AsNoTracking();

            return includeProperties.Aggregate
              (queryable, (current, includeProperty) => current.Include(includeProperty));
        }
        public IEnumerable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate)
        {
            IEnumerable<TEntity> results = _dbSet.AsNoTracking()
              .Where(predicate).ToList();
            return results;
        }

        public async Task<IEnumerable<TEntity>> FindByAsync(Expression<Func<TEntity, bool>> predicate)
        {
           return await _dbSet.AsNoTracking().Where(predicate).ToListAsync();
        }

        public IEnumerable<TEntity> FindByWithTracking(Expression<Func<TEntity, bool>> predicate)
        {
            IEnumerable<TEntity> results = _dbSet
              .Where(predicate).ToList();
            return results;
        }

        public TEntity FindByKey(Guid id)
        {
            Expression<Func<TEntity, bool>> lambda = Utilities.BuildLambdaForFindByKey<TEntity>(id);
            var entity = _dbSet.AsNoTracking().SingleOrDefault(lambda);

            if (entity == null)
                throw new EntityNotFoundException<TEntity>(id);
            return entity;
        }

        public TEntity FindByKeyWithInclude(Guid id, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var query = GetAllIncluding(true, includeProperties);
            Expression<Func<TEntity, bool>> lambda = Utilities.BuildLambdaForFindByKey<TEntity>(id);
            var entity = query.AsNoTracking().SingleOrDefault(lambda);

            if (entity == null)
                throw new EntityNotFoundException<TEntity>(id);
            return entity;
        }

        public TEntity FindByExternalId(string externalId)
        {
            Expression<Func<TEntity, bool>> lambda = Utilities.BuildLambdaForFindByExternalId<TEntity>(externalId);
            var entity = _dbSet.AsNoTracking().SingleOrDefault(lambda);

            if (entity == null)
                throw new EntityNotFoundException<TEntity>(externalId);
            return entity;
        }

        public TEntity FindByKeyWithLocalCheck(Guid id)
        {
            Expression<Func<TEntity, bool>> lambda = Utilities.BuildLambdaForFindByKey<TEntity>(id);
            return _dbSet.Find(id);

        }

        public void Create(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        public void CreateRange(IEnumerable<TEntity> entities)
        {
            _dbSet.AddRange(entities);
        }

        public void Update(TEntity entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void UpdateRange(IEnumerable<TEntity> entities)
        {
            foreach(var entity in entities)
            {
                _dbSet.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;

            }
        }


        public void Delete(Guid id)
        {
            var entity = FindByKey(id);
            _dbSet.Remove(entity);
        }

        public void Delete(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public void Delete(IEnumerable<TEntity> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public IEnumerable<TEntity> FindByIncludeWithTracking(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var query = GetAllIncluding(true, includeProperties);
            IEnumerable<TEntity> results = query.Where(predicate).ToList();
            return results;
        }

        public async Task<TEntity> FindByKeyAsync(Guid id)
        {
            Expression<Func<TEntity, bool>> lambda = Utilities.BuildLambdaForFindByKey<TEntity>(id);
            var entity = await _dbSet.AsNoTracking().SingleOrDefaultAsync(lambda);

            if (entity == null)
                throw new EntityNotFoundException<TEntity>(id);
            return entity;
        }

        public async Task<TEntity> FindByKeyWithIncludeAsync(Guid id, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var query = GetAllIncluding(true, includeProperties);
            Expression<Func<TEntity, bool>> lambda = Utilities.BuildLambdaForFindByKey<TEntity>(id);
            var entity = await query.AsNoTracking().SingleOrDefaultAsync(lambda);

            if (entity == null)
                throw new EntityNotFoundException<TEntity>(id);
            return entity;
        }
    }
}
