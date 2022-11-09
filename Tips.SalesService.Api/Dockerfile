#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Tips.SalesService.Api/Tips.SalesService.Api.csproj", "Tips.SalesService.Api/"]
COPY ["LoggerService/LoggerService.csproj", "LoggerService/"]
COPY ["Contracts/Contracts.csproj", "Contracts/"]
COPY ["CommonModels/Entities.csproj", "CommonModels/"]
RUN dotnet restore "Tips.SalesService.Api/Tips.SalesService.Api.csproj"
COPY . .
WORKDIR "/src/Tips.SalesService.Api"
RUN dotnet build "Tips.SalesService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tips.SalesService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tips.SalesService.Api.dll"]