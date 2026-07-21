using CatalogService.Application.Abstractions;
using CatalogService.Application.Common.Exceptions;
using CatalogService.Domain.Catalog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Api.Controllers;

[ApiController]
[Route("api/v1/categories")]
public sealed class CategoriesController : ControllerBase
{
    private readonly ICatalogDbContext _context;

    public CategoriesController(ICatalogDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<CategoryDto>>> Get(CancellationToken cancellationToken)
    {
        var categories = await _context.Categories
            .OrderBy(x => x.Name)
            .Select(x => new CategoryDto(x.Id, x.Name, x.Slug, x.IsActive))
            .ToArrayAsync(cancellationToken);

        return Ok(categories);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create(CategoryRequest request, CancellationToken cancellationToken)
    {
        var category = Category.Create(request.Name, request.Slug);
        _context.Categories.Add(category);
        await _context.SaveChangesAsync(cancellationToken);
        var dto = new CategoryDto(category.Id, category.Name, category.Slug, category.IsActive);
        return CreatedAtAction(nameof(Get), new { id = category.Id }, dto);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, CategoryRequest request, CancellationToken cancellationToken)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundException("Category not found.");

        category.Update(request.Name, request.Slug, request.IsActive);
        await _context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundException("Category not found.");

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}

public sealed record CategoryRequest(string Name, string Slug, bool IsActive = true);
public sealed record CategoryDto(Guid Id, string Name, string Slug, bool IsActive);
