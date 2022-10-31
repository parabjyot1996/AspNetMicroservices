using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Models;
using Ordering.Domain.Entities;

namespace Ordering.Application.Features.Orders.Commands.CheckoutOrder;
public class CheckoutOrderHandler : IRequestHandler<CheckoutOrderCommand, int>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;
    private readonly ILogger<CheckoutOrderHandler> _logger;

    public CheckoutOrderHandler(IOrderRepository orderRepository,
                                IMapper mapper,
                                IEmailService emailService,
                                ILogger<CheckoutOrderHandler> logger)
    {
        this._mapper = mapper;
        this._emailService = emailService;
        this._logger = logger;
        this._orderRepository = orderRepository;
    }

    public async Task<int> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
    {
        var order = _mapper.Map<Order>(request);
        var newOrder = await _orderRepository.AddAsync(order);

        _logger.LogInformation($"Order {order.Id} is successfully created");
        await SendMail(newOrder);

        return newOrder.Id;
    }

    private async Task SendMail(Order order)
    {
        var email = new Email()
        { 
            To = "ezozkme@gmail.com", 
            Body = $"Order was created.", 
            Subject = "Order was created"
        };

        try
        {
            await _emailService.SendEmail(email);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Order {order.Id} failed due to an error with the mail service: {ex.Message}");
        }
    }
}
