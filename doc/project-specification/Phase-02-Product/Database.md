# Product Database

Database name: `catalog_service`

## Tables
- `Categories`
- `Brands`
- `Products`
- `ProductImages`
- `__EFMigrationsHistory`

## Indexes
- Unique category slug
- Unique brand slug
- Unique product slug
- Unique product SKU
- Product category index
- Product brand index

## Migration SQL
- `src/CatalogService.Infrastructure/Persistence/Migrations/202607210001_InitialCatalogSchema.mysql.sql`
