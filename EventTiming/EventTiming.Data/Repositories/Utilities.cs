using System;
using System.Linq.Expressions;

namespace EventTiming.Data.Repositories
{
    public static class Utilities
    {
        public static Expression<Func<TEntity, bool>> BuildLambdaForFindByKey<TEntity>(Guid id)
        {
            var item = Expression.Parameter(typeof(TEntity), "entity");
            var prop = Expression.Property(item, "Id");
            var value = Expression.Constant(id);
            var equal = Expression.Equal(prop, value);
            var lambda = Expression.Lambda<Func<TEntity, bool>>(equal, item);
            return lambda;
        }

        public static Expression<Func<TEntity, bool>> BuildLambdaForFindByExternalId<TEntity>(string externalId)
        {
            var item = Expression.Parameter(typeof(TEntity), "entity");
            var prop = Expression.Property(item, "ExternalId");
            var value = Expression.Constant(externalId);
            var equal = Expression.Equal(prop, value);
            var lambda = Expression.Lambda<Func<TEntity, bool>>(equal, item);
            return lambda;
        }


    }
}
