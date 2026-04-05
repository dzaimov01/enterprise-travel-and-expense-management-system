# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files
COPY ["enterprise-travel-and-expense-management-system/enterprise-travel-and-expense-management-system.csproj", "enterprise-travel-and-expense-management-system/"]

# Restore dependencies
RUN dotnet restore "enterprise-travel-and-expense-management-system/enterprise-travel-and-expense-management-system.csproj"

# Copy source code
COPY . .

# Build the application
RUN dotnet build "enterprise-travel-and-expense-management-system/enterprise-travel-and-expense-management-system.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "enterprise-travel-and-expense-management-system/enterprise-travel-and-expense-management-system.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080

# Install curl for container healthcheck
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

# Copy published application from publish stage
COPY --from=publish /app/publish .

# Create logs directory
RUN mkdir -p /app/logs

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=40s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Run the application
ENTRYPOINT ["dotnet", "enterprise-travel-and-expense-management-system.dll"]
