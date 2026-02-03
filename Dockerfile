# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY CvParser.slnx .
COPY src/CvParser.Api/CvParser.Api.csproj src/CvParser.Api/
COPY src/CvParser.Core/CvParser.Core.csproj src/CvParser.Core/
COPY tests/CvParser.Tests/CvParser.Tests.csproj tests/CvParser.Tests/

# Restore dependencies
RUN dotnet restore src/CvParser.Api/CvParser.Api.csproj

# Copy source code
COPY src/ src/

# Build the application
RUN dotnet build src/CvParser.Api/CvParser.Api.csproj -c Release -o /app/build

# Publish the application
RUN dotnet publish src/CvParser.Api/CvParser.Api.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Copy published application
COPY --from=build /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "CvParser.Api.dll"]
