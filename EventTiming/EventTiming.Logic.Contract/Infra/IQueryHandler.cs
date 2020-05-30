using System.Threading.Tasks;

namespace EventTiming.Logic.Contract.Infra
{
    public interface IQueryHandler<TQuery, TResult>
            where TQuery : IQuery
            where TResult : IQueryResult
    {
        Task<TResult> Execute(TQuery query);

    }
}
