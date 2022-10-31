using Catalog.API.Entities;

namespace Catalog.API.Repositories;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllProducts();

    Task<Product> GetProductById(string id);

    Task<Product> GetProductByName(string name);

    Task<IEnumerable<Product>> GetProductByCategory(string categoryName);

    Task CreateProduct(Product product);

    Task<bool> DeleteProduct(string id);

    Task<bool> UpdateProduct(Product product);
}