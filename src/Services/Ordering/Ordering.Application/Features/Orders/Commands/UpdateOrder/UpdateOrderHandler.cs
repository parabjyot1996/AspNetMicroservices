using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Exceptions;
using Ordering.Domain.Entities;

namespace Ordering.Application.Features.Orders.Commands.UpdateOrder;
public class UpdateOrderHandler : IRequestHandler<UpdateOrderCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateOrderHandler> _logger;

    public UpdateOrderHandler(IOrderRepository orderRepository,
                                IMapper mapper,
                                ILogger<UpdateOrderHandler> logger)
    {
        this._orderRepository = orderRepository;
        this._mapper = mapper;
        this._logger = logger;
    }

    public async Task<Unit> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var orderToUpdate = await _orderRepository.GetByIdAsync(request.Id);

        if (orderToUpdate == null)
        {
            throw new NotFoundException(nameof(Order), request.Id);
        }

        var updatedOrder = _mapper.Map<Order>(request);

        await _orderRepository.UpdateAsync(updatedOrder);

        _logger.LogInformation($"Order {updatedOrder.Id} is updated successfully");

        return Unit.Value;
    }
}