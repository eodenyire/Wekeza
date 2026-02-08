FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8084

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ComprehensiveWekezaApi.csproj", "./"]
RUN dotnet restore "ComprehensiveWekezaApi.csproj"
COPY . .
RUN dotnet build "ComprehensiveWekezaApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ComprehensiveWekezaApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ComprehensiveWekezaApi.dll"]
