#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["SaleService.API/SaleService.API.csproj", "SaleService.API/"]
COPY ["SaleService.BL/SaleService.BL.csproj", "SaleService.BL/"]
COPY ["SaleService.DAL/SaleService.DAL.csproj", "SaleService.DAL/"]
RUN dotnet restore "SaleService.API/SaleService.API.csproj"
COPY . .
WORKDIR "/src/SaleService.API"
RUN dotnet build "SaleService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SaleService.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SaleService.API.dll"]
