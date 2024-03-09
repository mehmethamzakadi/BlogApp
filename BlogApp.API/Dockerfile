#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BlogApp.API/BlogApp.API.csproj", "BlogApp.API/"]
COPY ["BlogApp.Infrastructure/BlogApp.Infrastructure.csproj", "BlogApp.Infrastructure/"]
COPY ["BlogApp.Application/BlogApp.Application.csproj", "BlogApp.Application/"]
COPY ["BlogApp.Domain/BlogApp.Domain.csproj", "BlogApp.Domain/"]
COPY ["BlogApp.Persistence/BlogApp.Persistence.csproj", "BlogApp.Persistence/"]
RUN dotnet restore "./BlogApp.API/BlogApp.API.csproj"
COPY . .
WORKDIR "/src/BlogApp.API"
RUN dotnet build "./BlogApp.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BlogApp.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlogApp.API.dll"]