# Product Backend

## Architecture
- Catalog Service uses separate Api, Application, Domain, and Infrastructure projects.
- EF Core + Pomelo MySQL provider persists catalog data.
- Catalog owns product, category, brand, and product image entities.
- Controllers use the Catalog DbContext abstraction for the first slice.

## Projects
- `src/CatalogService.Api`
- `src/CatalogService.Application`
- `src/CatalogService.Domain`
- `src/CatalogService.Infrastructure`
- `tests/CatalogService.UnitTests`
- `tests/CatalogService.IntegrationTests`
- `tests/CatalogService.ApiTests`
