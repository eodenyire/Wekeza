# Wekeza Core API v1 - Production Dockerfile
# Multi-stage build for optimized image size

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["Wekeza.Core.Api/Wekeza.Core.Api.csproj", "Wekeza.Core.Api/"]
COPY ["Wekeza.Core.Application/Wekeza.Core.Application.csproj", "Wekeza.Core.Application/"]
COPY ["Wekeza.Core.Domain/Wekeza.Core.Domain.csproj", "Wekeza.Core.Domain/"]
COPY ["Wekeza.Core.Infrastructure/Wekeza.Core.Infrastructure.csproj", "Wekeza.Core.Infrastructure/"]

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
# Use Debian-based image (wget/curl pre-installed, no apk needed)
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

# Expose ports
EXPOSE 8080
EXPOSE 8081

# Health check using /proc/net/tcp (port 8080 = 0x1F90) — no curl/wget needed
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=5 \
    CMD sh -c 'cat /proc/net/tcp6 | grep -q " 00000000000000000000000000001F90 " && exit 0 || exit 1'

# Set environment
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Start application
ENTRYPOINT ["dotnet", "Wekeza.Core.Api.dll"]
