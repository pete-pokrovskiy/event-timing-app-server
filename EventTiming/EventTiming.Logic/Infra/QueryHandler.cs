using EventTiming.Data;
using EventTiming.Logic.Contract.Infra;
using System;
using System.Threading.Tasks;

namespace Croc.CFB.Logic.Queries
{
    public abstract class QueryHandler<TQuery, TResult> : IQueryHandler<TQuery, TResult>
        where TQuery : IQuery
        where TResult : IQueryResult
    {
        protected readonly IUow _uow;

        public QueryHandler(IUow uow)
        {

            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        }

        public abstract Task<TResult> Execute(TQuery query);
    }
}
