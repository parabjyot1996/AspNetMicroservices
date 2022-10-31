using AutoMapper;
using MediatR;
using Ordering.Application.Contracts.Persistence;

namespace Ordering.Application.Features.Orders.Queries.GetOrdersList;

public class GetOrderListHandler : IRequestHandler<GetOrdersListQuery, List<OrdersDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;

    public GetOrderListHandler(IOrderRepository orderRepository,
                                IMapper mapper)
    {
        _mapper = mapper;
        _orderRepository = orderRepository;
    }

    public async Task<List<OrdersDto>> Handle(GetOrdersListQuery request,
                                        CancellationToken cancellationToken)
    {
        var orderList = await _orderRepository.GetOrdersByUserName(request.UserName);
        return _mapper.Map<List<OrdersDto>>(orderList);
    }
}