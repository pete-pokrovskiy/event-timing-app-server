using EventTiming.Data;
using EventTiming.Logic.Contract.Infra;
using EventTiming.Logic.Services.Auth;
using System;
using System.Threading.Tasks;

namespace Croc.CFB.Logic.Queries
{
    public abstract class QueryHandler<TQuery, TResult> : IQueryHandler<TQuery, TResult>
        where TQuery : IQuery
        where TResult : IQueryResult
    {
        protected readonly IUow _uow;
        protected readonly ICurrentUserDataService _currentUserDataService;

        public QueryHandler(IUow uow, ICurrentUserDataService currentUserDataService)
        {

            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _currentUserDataService = currentUserDataService;
        }

        public abstract Task<TResult> Execute(TQuery query);
    }
}
