#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Tips.Master.Api/Tips.Master.Api.csproj", "Tips.Master.Api/"]
RUN dotnet restore "Tips.Master.Api/Tips.Master.Api.csproj"
COPY . .
WORKDIR "/src/Tips.Master.Api"
RUN dotnet build "Tips.Master.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tips.Master.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tips.Master.Api.dll"]