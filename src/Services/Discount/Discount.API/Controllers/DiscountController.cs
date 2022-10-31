using System.Net;
using Discount.API.Entities;
using Discount.API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Discount.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class DiscountController : Controller
{
    private readonly IDiscountRepository _discountRepository;

    public DiscountController(IDiscountRepository discountRepository)
    {
        this._discountRepository = discountRepository;
    }

    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<ActionResult<bool>> CreateDiscountAsync(Coupon coupon)
    {
        return Ok(await _discountRepository.CreateDiscount(coupon));  
    }

    [HttpDelete("{productName}", Name = "DeleteDiscount")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<ActionResult<bool>> DeleteDiscountAsync(string productName)
    {
        return Ok(await _discountRepository.DeleteDiscount(productName));  
    }

    [HttpGet("{productName}", Name = "GetDiscount")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<ActionResult<Coupon>> GetDiscountAsync(string productName)
    {
        return Ok(await _discountRepository.GetDiscount(productName));  
    }

    [HttpPut]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<ActionResult<bool>> UpdateDiscountAsync(Coupon coupon)
    {
        return Ok(await _discountRepository.UpdateDiscount(coupon));  
    }
}