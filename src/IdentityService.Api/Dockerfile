FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/IdentityService.Domain/IdentityService.Domain.csproj src/IdentityService.Domain/
COPY src/IdentityService.Application/IdentityService.Application.csproj src/IdentityService.Application/
COPY src/IdentityService.Infrastructure/IdentityService.Infrastructure.csproj src/IdentityService.Infrastructure/
COPY src/IdentityService.Api/IdentityService.Api.csproj src/IdentityService.Api/
RUN dotnet restore src/IdentityService.Api/IdentityService.Api.csproj

COPY src src
RUN dotnet publish src/IdentityService.Api/IdentityService.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "IdentityService.Api.dll"]
