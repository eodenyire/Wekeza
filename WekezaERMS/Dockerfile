# Wekeza ERMS API - Production Dockerfile
# Multi-stage build for optimized image size

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files for layer-cached restore
COPY ["API/WekezaERMS.API.csproj", "API/"]
COPY ["Application/WekezaERMS.Application.csproj", "Application/"]
COPY ["Domain/WekezaERMS.Domain.csproj", "Domain/"]
COPY ["Infrastructure/WekezaERMS.Infrastructure.csproj", "Infrastructure/"]

# Restore dependencies
RUN dotnet restore "API/WekezaERMS.API.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/API"
RUN dotnet build "WekezaERMS.API.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "WekezaERMS.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Create non-root user for security
RUN groupadd -r -g 1001 wekeza && \
    useradd -r -u 1001 -g wekeza wekeza

# Copy published app
COPY --from=publish /app/publish .

# Set ownership
RUN chown -R wekeza:wekeza /app

# Switch to non-root user
USER wekeza

# Expose HTTP port
EXPOSE 5000

# Health check using /proc/net/tcp and tcp6 (port 5000 = 0x1388) — no curl/wget needed
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=5 \
    CMD sh -c 'grep -q ":1388 " /proc/net/tcp6 2>/dev/null || grep -q ":1388 " /proc/net/tcp 2>/dev/null'

# Configure URLs and environment
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

# Start application
ENTRYPOINT ["dotnet", "WekezaERMS.API.dll"]
