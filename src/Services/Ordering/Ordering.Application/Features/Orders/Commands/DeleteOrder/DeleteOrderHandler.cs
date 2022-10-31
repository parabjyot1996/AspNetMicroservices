using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Exceptions;
using Ordering.Domain.Entities;

namespace Ordering.Application.Features.Orders.Commands.DeleteOrder;
public class DeleteOrderHandler : IRequestHandler<DeleteOrderCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<DeleteOrderHandler> _logger;

    public DeleteOrderHandler(IOrderRepository orderRepository,
                                ILogger<DeleteOrderHandler> logger)
    {
        this._orderRepository = orderRepository;
        this._logger = logger;
    }

    public async Task<Unit> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        var orderToDelete = await _orderRepository.GetByIdAsync(request.Id);
        if (orderToDelete == null)
        {
            throw new NotFoundException(nameof(Order), request.Id);
        }

        await _orderRepository.DeleteAsync(orderToDelete);
        _logger.LogInformation($"Order {orderToDelete.Id} deleted successfully.");

        return Unit.Value;
    }
}