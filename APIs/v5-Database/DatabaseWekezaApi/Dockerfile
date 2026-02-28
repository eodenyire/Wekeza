FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8082

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["DatabaseWekezaApi.csproj", "./"]
RUN dotnet restore "DatabaseWekezaApi.csproj"
COPY . .
RUN dotnet build "DatabaseWekezaApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DatabaseWekezaApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DatabaseWekezaApi.dll"]
