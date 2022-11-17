#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Tips.Grin.Api/Tips.Grin.Api.csproj", "Tips.Grin.Api/"]
RUN dotnet restore "Tips.Grin.Api/Tips.Grin.Api.csproj"
COPY . .
WORKDIR "/src/Tips.Grin.Api"
RUN dotnet build "Tips.Grin.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tips.Grin.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tips.Grin.Api.dll"]