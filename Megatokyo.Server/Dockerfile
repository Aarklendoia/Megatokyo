#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Megatokyo.Server/Megatokyo.Server.csproj", "Megatokyo.Server/"]
COPY ["Megatokyo.Logic/Megatokyo.Logic.csproj", "Megatokyo.Logic/"]
COPY ["Megatokyo.Domain/Megatokyo.Domain.csproj", "Megatokyo.Domain/"]
COPY ["Megatokyo.Infrastructure/Megatokyo.Infrastructure.csproj", "Megatokyo.Infrastructure/"]
RUN dotnet restore "Megatokyo.Server/Megatokyo.Server.csproj"
COPY . .
WORKDIR "/src/Megatokyo.Server"
RUN dotnet build "Megatokyo.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Megatokyo.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Megatokyo.Server.dll"]