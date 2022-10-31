using AutoMapper;
using Discount.Grpc.Entities;
using Discount.Grpc.Protos;
using Discount.Grpc.Repositories;
using Grpc.Core;

namespace Discount.Grpc.Services;

public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
{
    private readonly IDiscountRepository _discountRepository;
    private readonly ILogger<DiscountService> _logger;
    private readonly IMapper _mapper;

    public DiscountService(IDiscountRepository discountRepository, 
                            ILogger<DiscountService> logger, 
                            IMapper mapper)
    {
        this._discountRepository = discountRepository;
        this._logger = logger;
        this._mapper = mapper;
    }

    public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
    {
        var coupon = await _discountRepository.GetDiscount(request.ProductName);
        if (coupon == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Discount not found for product name {request.ProductName}"));
        }    

        var couponModel = _mapper.Map<CouponModel>(coupon);

        return couponModel;
    }

    public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
    {
        var couponRequest = _mapper.Map<Coupon>(request);
        await _discountRepository.CreateDiscount(couponRequest);

        var couponModel = _mapper.Map<CouponModel>(couponRequest);
        return couponModel;
    }

    public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
    {
        var couponRequest = _mapper.Map<Coupon>(request);
        await _discountRepository.UpdateDiscount(couponRequest);

        var couponModel = _mapper.Map<CouponModel>(couponRequest);
        return couponModel;
    }

    public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
    {
        var isDeleted = await _discountRepository.DeleteDiscount(request.ProductName);

        return new DeleteDiscountResponse
        {
            Success = isDeleted
        };
    }
}