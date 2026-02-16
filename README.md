# CV Parser

Web application that reads a new employee's CV and IFU, extracts skills, and saves them in a structured format.

## Technology Stack

- **Backend**: .NET 10 Web API (Controller-based)
- **Frontend**: React 18 with TypeScript (Vite)
- **Solution**: slnx format

## Architecture

### Backend (.NET 10 Web API)
- **Controllers**: RESTful endpoints using attribute routing
- **Repository Pattern**: `IProfileRepository` abstraction for data access
- **Service Layer**: `ICvSkillExtractor` for CV processing
- **In-Memory Storage**: Development implementation with seeded profiles
- **Validation**: Model state validation with problem details responses
- **Documentation**: Swagger/OpenAPI with XML comments

### Frontend (React + TypeScript)
- **Layouts**: Main and Admin layout components
- **Routing**: React Router v6 with nested routes
- **Styling**: Tailwind CSS with custom theme
- **UI Components**: Headless UI for accessible components
- **State Management**: React hooks for local state
- **API Client**: Fetch-based client with error handling

## Project Structure

```
cv-parser/
├── src/
│   ├── CvParser.Api/          # .NET 10 Web API
│   ├── CvParser.Api.Tests/    # NUnit test project
│   └── CvParser.Web/          # React + TypeScript frontend
├── CvParser.slnx              # Solution file
└── .github/
    └── copilot-instructions.md
```

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js 20+](https://nodejs.org/) (for frontend)
- IDE: Visual Studio 2026, JetBrains Rider, or Visual Studio Code

## Getting Started

### Backend

```bash
cd src/CvParser.Api
dotnet restore
dotnet run
```

The API will be available at `http://localhost:5028`

#### Groq API Key

Set the key using User Secrets (recommended):

```bash
cd src/CvParser.Api
dotnet user-secrets set "Groq:ApiKey" "YOUR_KEY"
```

Or set an environment variable:

```bash
export Groq__ApiKey=YOUR_KEY
```

### Frontend

```bash
cd src/CvParser.Web
npm install
npm run dev
```

The frontend will be available at `http://localhost:5173`

## API Endpoints

All endpoints are versioned under `/api/v1`:

- `GET /api/v1/profiles` - List all profiles
- `GET /api/v1/profiles/{id}` - Get profile details
- `POST /api/v1/profiles/{id}/cv/preview` - Preview extracted skills (PDF upload)
- `PUT /api/v1/profiles/{id}/skills` - Save confirmed skills

## Development

- Backend API: [http://localhost:5028](http://localhost:5028)
- Frontend Dev Server: [http://localhost:5173](http://localhost:5173)
- Swagger UI: [http://localhost:5028/swagger](http://localhost:5028/swagger)

## Build

### Backend
```bash
dotnet build CvParser.slnx
```

### Frontend
```bash
cd src/CvParser.Web
npm run build
```

## CI/CD and Azure Deployment (Free Tier)

GitHub Actions runs build and test checks on every pull request and on pushes to `main`.

### Azure Resources

- **API**: Azure App Service (Free F1)
- **Frontend**: Azure Static Web Apps (Free)

### GitHub Environments and Secrets

Create GitHub environments and store secrets there for least-privilege access.

**Environment: `api-production`**

- `AZURE_CLIENT_ID`
- `AZURE_TENANT_ID`
- `AZURE_SUBSCRIPTION_ID`
- `AZURE_WEBAPP_NAME`

**Environment: `web-production`**

- `AZURE_STATIC_WEB_APPS_API_TOKEN`

### Azure App Settings (API)

Set these in the App Service configuration:

- `Groq__ApiKey`: your Groq API key
- `AllowedOrigins__0`: your Static Web App URL (for example, `https://<app>.azurestaticapps.net`)

### Deployment Workflows

- **CI**: `.github/workflows/ci.yml` (build + test on PRs and `main`)
- **API**: `.github/workflows/deploy-api.yml` (deploys App Service on `main`)
- **Web**: `.github/workflows/deploy-web.yml` (deploys Static Web App on `main`)

## License

See [LICENSE](LICENSE) file for details.
