# CV Parser Project Structure

## Overview
This project consists of a .NET 10 Web API backend and a React frontend (with Vite) for processing CVs and extracting skills.

## Directory Structure
```
cv-parser/
├── CvParser.slnx                    # Visual Studio solution file
├── copilot-instructions.md          # Copilot instructions (to be expanded)
├── .gitignore                       # Git ignore patterns for VS, Rider, VS Code
├── README.md                        # Project documentation
└── src/
    ├── CvParser.Api/                # .NET 10 Web API backend
    │   ├── CvParser.Api.csproj      # Project file
    │   ├── Program.cs               # Main entry point
    │   ├── appsettings.json         # Application settings
    │   └── Properties/
    │       └── launchSettings.json  # Launch profiles
    └── CvParser.Web/                # React frontend with Vite
        ├── package.json             # NPM dependencies
        ├── vite.config.js           # Vite configuration
        ├── index.html               # Main HTML file
        └── src/
            ├── main.jsx             # React entry point
            ├── App.jsx              # Main App component
            └── assets/              # Static assets
```

## Technologies Used

### Backend
- **.NET 10** - Latest .NET framework
- **ASP.NET Core Web API** - RESTful API framework
- **OpenAPI/Swagger** - API documentation (built-in)

### Frontend
- **React 18** - Modern React with hooks
- **Vite** - Fast build tool and dev server
- **JavaScript (JSX)** - Component language

## Getting Started

### Prerequisites
- .NET 10 SDK
- Node.js 18+ and npm

### Running the Backend
```bash
cd src/CvParser.Api
dotnet restore
dotnet run
# API will be available at https://localhost:5001 or http://localhost:5000
```

### Running the Frontend
```bash
cd src/CvParser.Web
npm install
npm run dev
# Frontend will be available at http://localhost:5173
```

### Building for Production

Backend:
```bash
cd src/CvParser.Api
dotnet build -c Release
dotnet publish -c Release -o ./publish
```

Frontend:
```bash
cd src/CvParser.Web
npm run build
# Output will be in dist/ directory
```

## Development Tools Supported
- ✅ Visual Studio 2022+
- ✅ JetBrains Rider
- ✅ Visual Studio Code

All IDE-specific files are properly ignored via .gitignore.

## Next Steps
1. Implement CV upload functionality
2. Add skill extraction logic
3. Create data models and database
4. Integrate frontend with backend API
5. Add authentication and authorization
