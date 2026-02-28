# Wekeza Core API v1 - Production Dockerfile
# Multi-stage build for optimized image size

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

# Copy solution and project files
COPY ["Wekeza.Core.Api/Wekeza.Core.Api.csproj", "Wekeza.Core.Api/"]
COPY ["Wekeza.Core.Application/Wekeza.Core.Application.csproj", "Wekeza.Core.Application/"]
COPY ["Wekeza.Core.Domain/Wekeza.Core.Domain.csproj", "Wekeza.Core.Domain/"]
COPY ["Wekeza.Core.Infrastructure/Wekeza.Core.Infrastructure.csproj", "Wekeza.Core.Infrastructure/"]
COPY ["Wekeza.Nexus.Domain/Wekeza.Nexus.Domain.csproj", "Wekeza.Nexus.Domain/"]
COPY ["Wekeza.Nexus.Application/Wekeza.Nexus.Application.csproj", "Wekeza.Nexus.Application/"]
COPY ["Wekeza.Nexus.Infrastructure/Wekeza.Nexus.Infrastructure.csproj", "Wekeza.Nexus.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "Wekeza.Core.Api/Wekeza.Core.Api.csproj"

# Copy everything else
COPY . .

# Build
WORKDIR "/src/Wekeza.Core.Api"
RUN dotnet build "Wekeza.Core.Api.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "Wekeza.Core.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS final
WORKDIR /app

# Install healthcheck dependency
RUN apk add --no-cache curl

# Create non-root user for security
RUN addgroup -S -g 1001 wekeza && \
    adduser -S -u 1001 -G wekeza wekeza

# Copy published app
COPY --from=publish /app/publish .

# Set ownership
RUN chown -R wekeza:wekeza /app

# Switch to non-root user
USER wekeza

# Expose ports
EXPOSE 8080
EXPOSE 8081

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Set environment
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Start application
ENTRYPOINT ["dotnet", "Wekeza.Core.Api.dll"]
