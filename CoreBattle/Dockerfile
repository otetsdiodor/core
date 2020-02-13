#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["CoreBattle/CoreBattle.csproj", "CoreBattle/"]
COPY ["Infrastructure/CoreBattle.Infrastructure.Data/CoreBattle.Infrastructure.Data.csproj", "Infrastructure/CoreBattle.Infrastructure.Data/"]
COPY ["Domain/CoreBattle.Domain.Interfaces/CoreBattle.Domain.Interfaces.csproj", "Domain/CoreBattle.Domain.Interfaces/"]
COPY ["Domain/CoreBattle.Domain.Core/CoreBattle.Domain.Core.csproj", "Domain/CoreBattle.Domain.Core/"]
RUN dotnet restore "CoreBattle/CoreBattle.csproj"
COPY . .
WORKDIR "/src/CoreBattle"
RUN dotnet build "CoreBattle.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CoreBattle.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CoreBattle.dll"]