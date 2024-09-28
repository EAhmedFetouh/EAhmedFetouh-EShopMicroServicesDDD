using MediatR;

namespace BuildingBlocks.CQRS;

// no return any
public interface ICommand : ICommand<Unit>
{

}

//return response
public interface ICommand<out TResponse> :IRequest<TResponse>
{
}
