# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["practo-backend.csproj", "./"]
RUN dotnet restore "practo-backend.csproj"
COPY . .
RUN dotnet build "practo-backend.csproj" -c Release -o /app/build
RUN dotnet publish "practo-backend.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "practo-backend.dll"]
