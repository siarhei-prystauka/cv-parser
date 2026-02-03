# cv-parser

Web application that reads a new employee's CV and IFU, extracts skills, and saves them in a structured format.

## Features

- **CV Upload & Parsing**: Upload CV documents (text files) and automatically extract structured information
- **Skill Extraction**: Identifies technical skills and categorizes them
- **RESTful API**: Clean API endpoints for document management
- **In-Memory Storage**: Fast, simple storage for development and testing
- **Health Monitoring**: Built-in health check endpoint
- **Docker Support**: Containerized deployment ready

## Tech Stack

- **.NET 10.0**: Latest .NET framework
- **ASP.NET Core Web API**: RESTful API framework
- **xUnit**: Testing framework
- **Docker**: Containerization
- **OpenAPI/Swagger**: API documentation

## Project Structure

```
cv-parser/
├── src/
│   ├── CvParser.Api/          # Web API project
│   │   ├── Controllers/       # API controllers
│   │   ├── DTOs/             # Data transfer objects
│   │   └── Program.cs        # Application entry point
│   └── CvParser.Core/         # Core business logic
│       ├── Interfaces/        # Service interfaces
│       ├── Models/           # Domain models
│       └── Services/         # Service implementations
├── tests/
│   └── CvParser.Tests/       # Unit tests
├── Dockerfile                 # Docker configuration
├── docker-compose.yml        # Docker Compose configuration
└── CvParser.slnx            # Solution file
```

## Getting Started

### Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker](https://www.docker.com/get-started) (optional, for containerized deployment)

### Local Development

1. **Clone the repository**
   ```bash
   git clone https://github.com/siarhei-prystauka/cv-parser.git
   cd cv-parser
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Build the solution**
   ```bash
   dotnet build
   ```

4. **Run tests**
   ```bash
   dotnet test
   ```

5. **Run the application**
   ```bash
   dotnet run --project src/CvParser.Api/CvParser.Api.csproj
   ```

   The API will be available at `https://localhost:5001` or `http://localhost:5000`

### Docker Deployment

1. **Build and run with Docker Compose**
   ```bash
   docker-compose up --build
   ```

   The API will be available at `http://localhost:8080`

2. **Build Docker image manually**
   ```bash
   docker build -t cv-parser:latest .
   docker run -p 8080:8080 cv-parser:latest
   ```

## API Endpoints

### Health Check
- **GET** `/api/health` - Check API health status

### CV Management
- **POST** `/api/cv/upload` - Upload and parse a CV document
  - Request: `multipart/form-data` with file
  - Response: Document ID and extracted skills

- **GET** `/api/cv` - Get all uploaded CV documents
  - Response: List of all CV documents

- **GET** `/api/cv/{id}` - Get a specific CV document by ID
  - Response: CV document details

- **DELETE** `/api/cv/{id}` - Delete a CV document
  - Response: 204 No Content

### API Documentation

When running in development mode, access the OpenAPI documentation at:
- OpenAPI JSON: `/openapi/v1.json`

## Usage Example

### Upload a CV

```bash
curl -X POST http://localhost:8080/api/cv/upload \
  -F "file=@path/to/cv.txt"
```

Response:
```json
{
  "documentId": "123e4567-e89b-12d3-a456-426614174000",
  "fileName": "cv.txt",
  "uploadedAt": "2026-02-03T09:00:00Z",
  "extractedSkills": [
    {
      "name": "C#",
      "category": "Programming",
      "yearsOfExperience": 0,
      "level": "Intermediate"
    }
  ]
}
```

### Get All Documents

```bash
curl http://localhost:8080/api/cv
```

### Check Health

```bash
curl http://localhost:8080/api/health
```

## Development

### Running Tests

```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --verbosity detailed

# Run specific test
dotnet test --filter "FullyQualifiedName~CvParserServiceTests"
```

### Build Configuration

```bash
# Debug build
dotnet build --configuration Debug

# Release build
dotnet build --configuration Release
```

## CI/CD

The project includes a GitHub Actions workflow (`.github/workflows/ci-cd.yml`) that:
- Runs on push to `main` or `develop` branches
- Executes all tests
- Builds the application
- Creates a Docker image

## Future Enhancements

- [ ] Add support for PDF and DOCX file formats
- [ ] Implement advanced NLP/ML-based skill extraction
- [ ] Add persistent database storage (PostgreSQL/MongoDB)
- [ ] Implement authentication and authorization
- [ ] Add batch processing capabilities
- [ ] Create frontend UI for CV upload and management
- [ ] Add export functionality (JSON, CSV, XML)
- [ ] Implement skill categorization and matching algorithms

## License

See [LICENSE](LICENSE) file for details.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.
