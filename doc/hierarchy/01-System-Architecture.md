# System Architecture

Client
 -> API Gateway
 -> Identity
 -> Catalog
 -> Inventory
 -> Cart
 -> Checkout
 -> Payment
 -> Order
 -> Shipping
 -> Notification
 -> Reporting

Shared: Redis, MySQL per service, Message Broker, Object Storage, Observability.
