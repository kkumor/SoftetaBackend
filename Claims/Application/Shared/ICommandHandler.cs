namespace Claims.Application.Shared;

public interface ICommand;

public interface ICommandResult;

public interface ICommandHandler<in TCommand, TCommandResult>
    where TCommand : ICommand
    where TCommandResult : ICommandResult
{
    Task<TCommandResult> Handle(TCommand query, CancellationToken cancellationToken = default);
}