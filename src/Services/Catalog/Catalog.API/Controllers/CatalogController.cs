using System.Net;
using Catalog.API.Entities;
using Catalog.API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CatalogController : Controller
{
    private readonly IProductRepository _productRepo;
    private readonly ILogger<CatalogController> _logger;

    public CatalogController(IProductRepository productRepo, ILogger<CatalogController> logger)
    {
        this._productRepo = productRepo;
        this._logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Product>), statusCode: (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        var products = await _productRepo.GetAllProducts();
        return Ok(products);
    }

    [HttpGet("{id:length(24)}", Name = "GetProductById")]
    [ProducesResponseType(statusCode: (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(Product), statusCode: (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<Product>>> GetProductById(string id)
    {
        var product = await _productRepo.GetProductById(id);

        if (product == null)
        {
            return NotFound();
        }
        return Ok(product);
    }

    [HttpGet]
    [Route("[action]/{category}")]
    [ProducesResponseType(statusCode: (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(IEnumerable<Product>), statusCode: (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<Product>>> GetProductByCategory(string category)
    {
        var products = await _productRepo.GetProductByCategory(category);

        if (products == null)
        {
            return NotFound();
        }
        return Ok(products);
    }

    [HttpGet("{name}", Name = "GetProductByName")]
    [ProducesResponseType(statusCode: (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(Product), statusCode: (int)HttpStatusCode.OK)]
    public async Task<ActionResult<Product>> GetProductByName(string name)
    {
        var product = await _productRepo.GetProductByName(name);

        if (product == null)
        {
            return NotFound();
        }
        return Ok(product);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Product), statusCode: (int)HttpStatusCode.OK)]
    public async Task<ActionResult<Product>> CreateProduct([FromBody]Product product)
    {
        await _productRepo.CreateProduct(product);
        return CreatedAtRoute("GetProduct", new { id = product.Id }, product);
    }

    [HttpPut]
    [ProducesResponseType(typeof(Product), statusCode: (int)HttpStatusCode.OK)]
    public async Task<IActionResult> UpdateProduct([FromBody]Product product)
    {
        return Ok(await _productRepo.UpdateProduct(product));
    }

    [HttpDelete("{id:length(24)}", Name = "DeleteProduct")]
    [ProducesResponseType(typeof(Product), statusCode: (int)HttpStatusCode.OK)]
    public async Task<IActionResult> DeleteProduct(string id)
    {
        return Ok(await _productRepo.DeleteProduct(id));
    }
}