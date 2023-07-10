using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("/api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext dbContext;

    public ProductsController(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct(CreateProductDto product)
    {
        var created = dbContext.Products.Add(new Product
        {
            Id = Guid.NewGuid(),
            Name = product.Name,
            Price = product.Price,
            CreatedAt = product.CreatedAt,
            ModifiedAt = product.ModifiedAt,
            Status = product.Status,
        });

        await dbContext.SaveChangesAsync();

        return Ok(created.Entity.Id);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct([FromRoute] Guid id)
    {
        var product = await dbContext.Products
            .Where(p => p.Id == id)
            .Include(p => p.ProductDetails)
            .FirstOrDefaultAsync();

        if (product is null)
            return NotFound();

        return Ok(new GetProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            CreatedAt = product.CreatedAt,
            ModifiedAt = product.ModifiedAt,
            Status = product.Status,
            ProductDetails = product.ProductDetails is null
                ? null
                : new GetProductDetailsDto
                {
                    Id = product.ProductDetails.Id,
                    Description = product.ProductDetails.Description,
                    Color = product.ProductDetails.Color,
                    Material = product.ProductDetails.Material,
                    Weight = product.ProductDetails.Weight,
                    QuantityInStock = product.ProductDetails.QuantityInStock,
                    ManufactureDate = product.ProductDetails.ManufactureDate,
                    ExpiryDate = product.ProductDetails.ExpiryDate,
                    Size = product.ProductDetails.Size,
                    Manufacturer = product.ProductDetails.Manufacturer,
                    CountryOfOrigin = product.ProductDetails.CountryOfOrigin
                }
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] string search)
    {
        var productsQuery = dbContext.Products.AsQueryable();

        if (false == string.IsNullOrWhiteSpace(search))
            productsQuery = productsQuery.Where(u =>
                u.Name.ToLower().Contains(search.ToLower()));

        var products = await productsQuery
            .Select(u => new GetProductDto
            {
                Id = u.Id,
                Name = u.Name,
                Price = u.Price,
                CreatedAt = u.CreatedAt,
                ModifiedAt = u.ModifiedAt,
                Status = u.Status,
            })
            .ToListAsync();

        return Ok(products);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct([FromRoute] Guid id, UpdateProductDto updateProduct)
    {
        var product = await dbContext.Products
            .FirstOrDefaultAsync(u => u.Id == id);

        if (product is null)
            return NotFound();

        product.Name = updateProduct.Name;
        product.Price = updateProduct.Price;
        product.CreatedAt = updateProduct.CreatedAt;
        product.ModifiedAt = updateProduct.ModifiedAt;
        product.Status = updateProduct.Status;

        await dbContext.SaveChangesAsync();
        return Ok(product.Id);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct([FromRoute] Guid id)
    {
        var product = await dbContext.Products
            .FirstOrDefaultAsync(u => u.Id == id);

        if (product is null)
            return NotFound();

        dbContext.Products.Remove(product);
        await dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("{id}/details")]
    public async Task<IActionResult> CreateProductDetails(Guid id, CreateProductDetailsDto productDetails)
    {
        var product = await dbContext.Products
            .Where(u => u.Id == id)
            .Include(u => u.ProductDetails)
            .FirstOrDefaultAsync();

        if (product is null)
            return BadRequest($"Product with id {id} does not exist");

        if (product.ProductDetails is not null)
            return BadRequest($"Product already has details");

        product.ProductDetails = new ProductDetails
        {
            Description = productDetails.Description,
            Color = productDetails.Color,
            Material = productDetails.Material,
            Weight = productDetails.Weight,
            QuantityInStock = productDetails.QuantityInStock,
            ManufactureDate = productDetails.ManufactureDate,
            ExpiryDate = productDetails.ExpiryDate,
            Size = productDetails.Size,
            Manufacturer = productDetails.Manufacturer,
            CountryOfOrigin = productDetails.CountryOfOrigin
        };

        await dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpGet("{id}/details")]
    public async Task<IActionResult> GetProductDetails([FromRoute] Guid id)
    {
        var product = await dbContext.Products
            .Where(u => u.Id == id)
            .Include(u => u.ProductDetails)
            .FirstOrDefaultAsync();

        if (product is null || product.ProductDetails is null)
            return NotFound();

        return Ok(new GetProductDetailsDto
        {
            Id = product.ProductDetails.Id,
            Description = product.ProductDetails.Description,
            Color = product.ProductDetails.Color,
            Material = product.ProductDetails.Material,
            Weight = product.ProductDetails.Weight,
            QuantityInStock = product.ProductDetails.QuantityInStock,
            ManufactureDate = product.ProductDetails.ManufactureDate,
            ExpiryDate = product.ProductDetails.ExpiryDate,
            Size = product.ProductDetails.Size,
            Manufacturer = product.ProductDetails.Manufacturer,
            CountryOfOrigin = product.ProductDetails.CountryOfOrigin
        });
    }

    [HttpPut("{id}/details")]
    public async Task<IActionResult> UpdateProductDetails([FromRoute] Guid id, UpdateProductDetailsDto updateProductDetails)
    {
        var product = await dbContext.Products
            .Where(p => p.Id == id)
            .Include(p => p.ProductDetails)
            .FirstOrDefaultAsync();

        if (product is null)
            return NotFound();

        if (product.ProductDetails is null)
            return Conflict("Product does not have details");

        product.ProductDetails.Description = updateProductDetails.Description;
        product.ProductDetails.Color = updateProductDetails.Color;
        product.ProductDetails.Material = updateProductDetails.Material;
        product.ProductDetails.Weight = updateProductDetails.Weight;
        product.ProductDetails.QuantityInStock = updateProductDetails.QuantityInStock;
        product.ProductDetails.ManufactureDate = updateProductDetails.ManufactureDate;
        product.ProductDetails.ExpiryDate = updateProductDetails.ExpiryDate;
        product.ProductDetails.Size = updateProductDetails.Size;
        product.ProductDetails.Manufacturer = updateProductDetails.Manufacturer;
        product.ProductDetails.CountryOfOrigin = updateProductDetails.CountryOfOrigin;

        await dbContext.SaveChangesAsync();

        return Ok(product.ProductDetails.Id);
    }

    [HttpDelete("{id}/details")]
    public async Task<IActionResult> DeleteProductDetails([FromRoute] Guid id)
    {
        var product = await dbContext.Products
            .Where(p => p.Id == id)
            .Include(p => p.ProductDetails)
            .FirstOrDefaultAsync();

        if (product is null)
            return NotFound();

        if (product.ProductDetails is null)
            return Conflict("Product does not have details");

        dbContext.ProductDetails.Remove(product.ProductDetails);
        await dbContext.SaveChangesAsync();

        return Ok();
    }
}