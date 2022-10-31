using Catalog.API.Data;
using Catalog.API.Entities;
using MongoDB.Driver;

namespace Catalog.API.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ICatalogContext _dbContext;

    public ProductRepository(ICatalogContext dbContext)
    {
        this._dbContext = dbContext;
    }

    public async Task CreateProduct(Product product)
    {
        await _dbContext.Products.InsertOneAsync(product);
    }

    public async Task<bool> DeleteProduct(string id)
    {
        var deleteResult = await _dbContext.Products.DeleteOneAsync(p => p.Id == id);
        return deleteResult.IsAcknowledged 
                && deleteResult.DeletedCount > 0;
    }

    public async Task<IEnumerable<Product>> GetAllProducts()
    {
        return await _dbContext
                            .Products
                            .Find(p => true)
                            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductByCategory(string categoryName)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.Category, categoryName);

        return await _dbContext
                            .Products
                            .Find(filter)
                            .ToListAsync();
    }

    public async Task<Product> GetProductById(string id)
    {
        return await _dbContext
                            .Products
                            .Find(p => p.Id == id)
                            .FirstOrDefaultAsync();
    }

    public async Task<Product> GetProductByName(string name)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.Name, name);

        return await _dbContext
                            .Products
                            .Find(filter)
                            .FirstOrDefaultAsync();
    }

    public async Task<bool> UpdateProduct(Product product)
    {
        var updateResult = await _dbContext
                                .Products
                                .ReplaceOneAsync(filter: p => p.Id == product.Id, replacement: product);

        return updateResult.IsAcknowledged 
                && updateResult.ModifiedCount > 0;
    }
}