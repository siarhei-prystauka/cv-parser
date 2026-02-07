# GitHub Copilot Instructions

## Project Overview
This is a full-stack web application with a .NET 10 Web API backend and React TypeScript frontend that reads a new employee’s CV and IFU, extracts skills, and saves them in a structured format.

## Technology Stack
- **Backend**: .NET 10 Web API (C#)
- **Frontend**: React 18 with TypeScript, Vite
- **Solution Format**: slnx (XML-based solution file)

## Code Style and Best Practices

### .NET Backend
- Follow Microsoft's official C# coding conventions
- Use async/await for all I/O operations
- Implement proper dependency injection using built-in DI container
- Use minimal APIs for simple endpoints, controllers for complex logic
- Apply proper exception handling and logging
- Follow RESTful API design principles
- Use record types for DTOs
- Implement proper validation using Data Annotations or FluentValidation
- Use LINQ for data querying
- Follow the principle of separation of concerns

### React TypeScript Frontend
- Use functional components with hooks
- Prefer arrow functions for component definitions
- Use TypeScript strict mode
- Define proper interfaces/types for all props and state
- Use async/await for API calls
- Implement proper error handling and loading states
- Follow React best practices for performance (useMemo, useCallback when needed)
- Keep components small and focused
- Use CSS modules or styled-components for styling

## Architecture Guidelines
- Keep business logic separate from presentation layer
- Use repository pattern for data access in .NET
- Implement proper API versioning
- Use environment-specific configuration files
- Follow SOLID principles
- Write clean, self-documenting code
- Add comments for complex logic only

## Testing
- Write unit tests for business logic
- Use NUnit for .NET testing
- Use React Testing Library for frontend tests
- Aim for meaningful test coverage, not just high percentage

## Security
- Never commit secrets or API keys
- Use appsettings.json for configuration with proper overrides
- Implement proper CORS configuration
- Validate all user inputs
- Use HTTPS in production

## Git Workflow
- Write clear, concise commit messages
- Keep commits atomic and focused
- Use conventional commit format when possible
