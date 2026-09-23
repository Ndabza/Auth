# 1. Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
# Port 5000 requires overriding .NET's default 8080 environment variable
ENV ASPNETCORE_HTTP_PORTS=5000
EXPOSE 5000

# 2. SDK image used to compile and build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy all .csproj files preserving their folder structure for caching
COPY ["src/Auth.Api/Auth.Api.csproj", "src/Auth.Api/"]
COPY ["src/Auth.Infrastructure/Auth.Infrastructure.csproj", "src/Auth.Infrastructure/"]
COPY ["src/Auth.Domain/Auth.Domain.csproj", "src/Auth.Domain/"]
COPY ["src/Auth.Application/Auth.Application.csproj", "src/Auth.Application/"]

# Restore packages using the main entry API project
RUN dotnet restore "src/Auth.Api/Auth.Api.csproj"

# Copy the rest of the source code
COPY . .

# Build from the specific API directory
WORKDIR "/src/src/Auth.Api"
RUN dotnet build "Auth.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# 3. Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Auth.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# 4. Final runtime stage
FROM base AS final
# Kept as /app to match the workdir in 'base' and let USER $APP_UID write to it safely
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Auth.Api.dll"]
