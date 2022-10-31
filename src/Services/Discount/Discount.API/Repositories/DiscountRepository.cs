using Dapper;
using Discount.API.Entities;
using Npgsql;

namespace Discount.API.Repositories;

public class DiscountRepository : IDiscountRepository
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<DiscountRepository> _logger;

    public DiscountRepository(IConfiguration configuration, ILogger<DiscountRepository> logger)
    {
        this._configuration = configuration;
        this._logger = logger;
    }

    public async Task<bool> CreateDiscount(Coupon coupon)
    {
        using var connection = new NpgsqlConnection
            (_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));

        int affectedRow = 
            await connection.ExecuteAsync(
                "INSERT INTO Coupon (ProductName, Description, Amount) VALUES (@ProductName, @Description, @Amount)", 
                    new { ProductName=coupon.ProductName, Description=coupon.Description, Amount=coupon.Amount });

        if (affectedRow == 0)
            return false;

        return true;
    }

    public async Task<bool> DeleteDiscount(string productName)
    {
        using var connection = new NpgsqlConnection
            (_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));

        int affectedRow = 
            await connection.ExecuteAsync(
                "DELETE FROM Coupon WHERE ProductName = @ProductName", 
                    new { ProductName=productName });

        if (affectedRow == 0)
            return false;

        return true;
    }

    public async Task<Coupon> GetDiscount(string productName)
    {
        string connectionString = _configuration.GetValue<string>("DatabaseSettings:ConnectionString");
        using var connection = new NpgsqlConnection(connectionString);

        var coupon = await connection.QueryFirstOrDefaultAsync<Coupon>(
            "SELECT * FROM Coupon WHERE ProductName = @ProductName", new { ProductName = productName });
        
        if (coupon == null)
        {
            return new Coupon 
            { ProductName="No Discount", Amount=0, Description="No Discount Available" };
        }

        return coupon;
    }

    public async Task<bool> UpdateDiscount(Coupon coupon)
    {
        using var connection = new NpgsqlConnection
            (_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));

        int affectedRow = 
            await connection.ExecuteAsync(
                "UPDATE Coupon SET ProductName = @ProductName, Description = @Description, Amount = @Amount WHERE Id = @Id", 
                    new { ProductName=coupon.ProductName, Description=coupon.Description, Amount=coupon.Amount , Id=coupon.Id });

        if (affectedRow == 0)
            return false;

        return true;              
    }
}