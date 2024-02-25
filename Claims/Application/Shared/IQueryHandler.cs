namespace Claims.Application.Shared;

public interface IQuery;

public interface IQueryResult;

public interface IQueryHandler<in TQuery, TQueryResult>
    where TQuery : IQuery
    where TQueryResult : IQueryResult
{
    Task<TQueryResult> Handle(TQuery query, CancellationToken cancellationToken = default);
}