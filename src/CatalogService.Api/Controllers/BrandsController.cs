using CatalogService.Application.Abstractions;
using CatalogService.Application.Common.Exceptions;
using CatalogService.Domain.Catalog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Api.Controllers;

[ApiController]
[Route("api/v1/brands")]
public sealed class BrandsController : ControllerBase
{
    private readonly ICatalogDbContext _context;

    public BrandsController(ICatalogDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<BrandDto>>> Get(CancellationToken cancellationToken)
    {
        var brands = await _context.Brands
            .OrderBy(x => x.Name)
            .Select(x => new BrandDto(x.Id, x.Name, x.Slug, x.IsActive))
            .ToArrayAsync(cancellationToken);

        return Ok(brands);
    }

    [HttpPost]
    public async Task<ActionResult<BrandDto>> Create(BrandRequest request, CancellationToken cancellationToken)
    {
        var brand = Brand.Create(request.Name, request.Slug);
        _context.Brands.Add(brand);
        await _context.SaveChangesAsync(cancellationToken);
        var dto = new BrandDto(brand.Id, brand.Name, brand.Slug, brand.IsActive);
        return CreatedAtAction(nameof(Get), new { id = brand.Id }, dto);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, BrandRequest request, CancellationToken cancellationToken)
    {
        var brand = await _context.Brands.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundException("Brand not found.");

        brand.Update(request.Name, request.Slug, request.IsActive);
        await _context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var brand = await _context.Brands.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundException("Brand not found.");

        _context.Brands.Remove(brand);
        await _context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}

public sealed record BrandRequest(string Name, string Slug, bool IsActive = true);
public sealed record BrandDto(Guid Id, string Name, string Slug, bool IsActive);
