using System.Net;
using AutoMapper;
using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repositories;
using EventBus.Messages.Events;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Basket.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class BasketController : Controller
{
    private readonly IBasketRepository _basketRepository;
    private readonly DiscountGrpcService _discountGrpcService;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _eventBus;

    public BasketController(IBasketRepository basketRepository,
                            DiscountGrpcService discountGrpcService,
                            IMapper mapper,
                            IPublishEndpoint eventBus)
    {
        this._basketRepository = basketRepository;
        this._discountGrpcService = discountGrpcService;
        this._mapper = mapper;
        this._eventBus = eventBus;
    }

    [HttpGet("{userName}", Name = "GetBasket")]
    [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<ShoppingCart>> GetBasketAsync(string userName)
    {
        var basket = await _basketRepository.GetBasketAsync(userName);
        return Ok(basket ?? new ShoppingCart(userName));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<ShoppingCart>> UpdateBasketAsync([FromBody]ShoppingCart cart)
    {
        foreach (var item in cart.Items)
        {
            var coupon = await _discountGrpcService.GetDiscount(item.ProductName);
            item.Price -= coupon.Amount;
        }

        var basket = await _basketRepository.UpdateBasketAsync(cart);
        return Ok(basket);
    }

    [HttpDelete("{userName}", Name = "DeleteBasket")]
    [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<ShoppingCart>> DeleteBasketAsync(string userName)
    {
        await _basketRepository.DeleteBasketAsync(userName);
        return Ok();
    }

    [HttpPost]
    [Route("[action]")]
    [ProducesResponseType((int)HttpStatusCode.Accepted)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
    {
        var basket = await _basketRepository.GetBasketAsync(basketCheckout.UserName);
        
        if (basket == null)
        {
            return BadRequest($"No basket data available for {basketCheckout.UserName}");
        }

        var basketEventMessage = _mapper.Map<BasketCheckoutEvent>(basketCheckout);
        basketEventMessage.TotalPrice = basket.TotalPrice;

        await _eventBus.Publish<BasketCheckoutEvent>(basketEventMessage);

        await _basketRepository.DeleteBasketAsync(basketCheckout.UserName);

        return Accepted();
    }
}