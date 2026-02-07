# CV Parser

Web application that reads a new employee's CV and IFU, extracts skills, and saves them in a structured format.

## Technology Stack

- **Backend**: .NET 10 Web API
- **Frontend**: React 18 with TypeScript (Vite)
- **Solution**: slnx format

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

The API will be available at `http://localhost:5000`

### Frontend

```bash
cd src/CvParser.Web
npm install
npm run dev
```

The frontend will be available at `http://localhost:5173`

## Development

- Backend API: [http://localhost:5000](http://localhost:5000)
- Frontend Dev Server: [http://localhost:5173](http://localhost:5173)
- Swagger UI: [http://localhost:5000/swagger](http://localhost:5000/swagger)

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

## License

See [LICENSE](LICENSE) file for details.
