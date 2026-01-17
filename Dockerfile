# Wekeza Bank - Production Dockerfile
# Multi-stage build for optimized image size

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["Core/Wekeza.Core.Api/Wekeza.Core.Api.csproj", "Core/Wekeza.Core.Api/"]
COPY ["Core/Wekeza.Core.Application/Wekeza.Core.Application.csproj", "Core/Wekeza.Core.Application/"]
COPY ["Core/Wekeza.Core.Domain/Wekeza.Core.Domain.csproj", "Core/Wekeza.Core.Domain/"]
COPY ["Core/Wekeza.Core.Infrastructure/Wekeza.Core.Infrastructure.csproj", "Core/Wekeza.Core.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "Core/Wekeza.Core.Api/Wekeza.Core.Api.csproj"

# Copy everything else
COPY . .

# Build
WORKDIR "/src/Core/Wekeza.Core.Api"
RUN dotnet build "Wekeza.Core.Api.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "Wekeza.Core.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Create non-root user for security
RUN addgroup --system --gid 1001 wekeza && \
    adduser --system --uid 1001 --ingroup wekeza wekeza

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
