using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EventTiming.Data.Repositories.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> Records { get; }
        IEnumerable<TEntity> All();
        IEnumerable<TEntity> AllInclude(params Expression<Func<TEntity, object>>[] includeProperties);
        void Delete(Guid id);
        void Delete(TEntity entity);
        void Delete(IEnumerable<TEntity> entity);
        Task<IEnumerable<TEntity>> FindBy(Expression<Func<TEntity, bool>> predicate);
        Task<IEnumerable<TEntity>> FindByWithTracking(Expression<Func<TEntity, bool>> predicate);
        IEnumerable<TEntity> FindByInclude(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties);
        Task<IEnumerable<TEntity>> FindByIncludeAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties);
        IEnumerable<TEntity> FindByIncludeWithTracking(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties);
        TEntity FindByKey(Guid id);
        Task<TEntity> FindByKeyAsync(Guid id);
        TEntity FindByKeyWithInclude(Guid id, params Expression<Func<TEntity, object>>[] includeProperties);
        Task<TEntity> FindByKeyWithIncludeAsync(Guid id, params Expression<Func<TEntity, object>>[] includeProperties);
        TEntity FindByExternalId(string externalId);
        TEntity FindByKeyWithLocalCheck(Guid id);
        void Create(TEntity entity);
        void CreateRange(IEnumerable<TEntity> entities);
        void Update(TEntity entity);
        void UpdateRange(IEnumerable<TEntity> entities);
    }
}