namespace Claims.Application.Shared;

public interface IQuery;

public interface IQueryResult;

public interface IQueryHandler<in TQuery, TQueryResult>
{
    Task<TQueryResult> Handle(TQuery query, CancellationToken cancellationToken = default);
}