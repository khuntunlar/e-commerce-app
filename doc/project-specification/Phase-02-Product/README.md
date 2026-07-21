# Phase 02: Product
Service: Catalog Service

## Purpose
Build the product catalog boundary for the ecommerce platform. Catalog owns product browsing data, including products, categories, brands, and product images.

## Database Ownership
- Identity Service database: `identity_service`
- Catalog Service database: `catalog_service`

Each service owns its own database. Product data must not be stored in the Identity database.

## First Backend Slice
- Scaffold Catalog Service with Clean Architecture style projects
- Add Category, Brand, Product, and ProductImage domain entities
- Add MySQL schema for `catalog_service`
- Add CRUD APIs for categories, brands, and products
- Add Docker Compose services for `catalog-api` and `catalog-mysql`
- Add initial unit/API/integration smoke tests

## Acceptance Criteria
- Catalog Service builds independently
- Catalog Service tests pass
- `catalog_service` schema can be created separately from `identity_service`
- Product/category/brand APIs are available under `/api/v1`
