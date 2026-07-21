using CatalogService.Application.Abstractions;
using CatalogService.Application.Common.Exceptions;
using CatalogService.Domain.Catalog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Api.Controllers;

[ApiController]
[Route("api/v1/products")]
public sealed class ProductsController : ControllerBase
{
    private readonly ICatalogDbContext _context;

    public ProductsController(ICatalogDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<ProductDto>>> Get(
        [FromQuery] string? search,
        [FromQuery] Guid? categoryId,
        [FromQuery] Guid? brandId,
        CancellationToken cancellationToken)
    {
        var query = _context.Products.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => x.Name.Contains(search) || x.Sku.Contains(search));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(x => x.CategoryId == categoryId.Value);
        }

        if (brandId.HasValue)
        {
            query = query.Where(x => x.BrandId == brandId.Value);
        }

        var products = await query
            .OrderBy(x => x.Name)
            .Select(x => new ProductDto(x.Id, x.CategoryId, x.BrandId, x.Name, x.Slug, x.Description, x.Price, x.Sku, x.IsActive))
            .ToArrayAsync(cancellationToken);

        return Ok(products);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundException("Product not found.");

        return Ok(new ProductDto(product.Id, product.CategoryId, product.BrandId, product.Name, product.Slug, product.Description, product.Price, product.Sku, product.IsActive));
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(ProductRequest request, CancellationToken cancellationToken)
    {
        await EnsureCategoryAndBrandExistAsync(request.CategoryId, request.BrandId, cancellationToken);

        var product = Product.Create(
            request.CategoryId,
            request.BrandId,
            request.Name,
            request.Slug,
            request.Description,
            request.Price,
            request.Sku);

        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = new ProductDto(product.Id, product.CategoryId, product.BrandId, product.Name, product.Slug, product.Description, product.Price, product.Sku, product.IsActive);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, dto);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, ProductRequest request, CancellationToken cancellationToken)
    {
        await EnsureCategoryAndBrandExistAsync(request.CategoryId, request.BrandId, cancellationToken);
        var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundException("Product not found.");

        product.Update(
            request.CategoryId,
            request.BrandId,
            request.Name,
            request.Slug,
            request.Description,
            request.Price,
            request.Sku,
            request.IsActive);

        await _context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundException("Product not found.");

        _context.Products.Remove(product);
        await _context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private async Task EnsureCategoryAndBrandExistAsync(Guid categoryId, Guid brandId, CancellationToken cancellationToken)
    {
        if (!await _context.Categories.AnyAsync(x => x.Id == categoryId, cancellationToken))
        {
            throw new NotFoundException("Category not found.");
        }

        if (!await _context.Brands.AnyAsync(x => x.Id == brandId, cancellationToken))
        {
            throw new NotFoundException("Brand not found.");
        }
    }
}

public sealed record ProductRequest(
    Guid CategoryId,
    Guid BrandId,
    string Name,
    string Slug,
    string Description,
    decimal Price,
    string Sku,
    bool IsActive = true);

public sealed record ProductDto(
    Guid Id,
    Guid CategoryId,
    Guid BrandId,
    string Name,
    string Slug,
    string Description,
    decimal Price,
    string Sku,
    bool IsActive);
