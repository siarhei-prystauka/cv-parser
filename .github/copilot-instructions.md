# GitHub Copilot Instructions

## Project Overview
This is a full-stack web application with a .NET 10 Web API backend and React TypeScript frontend that reads a new employee’s CV, extracts skills, and saves them in a structured format.

## Technology Stack
- **Backend**: .NET 10 Web API (C#)
- **Frontend**: React 18 with TypeScript, Vite
- **Solution Format**: slnx (XML-based solution file)

## Code Style and Best Practices

## Architecture Guidelines
- Keep business logic separate from presentation layer
- Use repository pattern for data access in .NET
- Use controller-based APIs for structured, multi-endpoint RESTful services
- Implement proper API versioning
- Use environment-specific configuration files
- Follow SOLID principles
- Write clean, self-documenting code

## Comments Policy
- Code should be readable without comments; prefer clear naming over explanatory comments
- Add comments for very complex logic only
- **Do** add concise `/// <summary>` on classes and public API controller actions (used by Swagger)
- **Do not** add XML doc comments on properties, private methods, or records unless they are non-obvious
- **Do** add inline comments only when explaining **why**, never **what**
- Keep TODO comments for actionable future work
- Never add `/// <param>`, `/// <returns>`, `/// <response>`, or `/// <inheritdoc />` tags

## Dependencies and Package Management
- **Never use alpha, beta, or preview versions** of NuGet packages or npm packages
- Always use stable, production-ready versions
- Check package stability and maintenance status before adoption
- Prefer well-established libraries with active communities
- Review package licenses for compatibility with project requirements
- Keep dependencies up to date with stable releases

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

## Prompt History
- Maintain prompt-history.md in the repository root.
- For every Copilot command or request in plan or agent mode, append a new entry with a timestamp and a short, readable summary.
- Use ISO 8601 timestamps with timezone (for example: 2026-02-07T14:30:00+01:00).
- Do not rewrite or reorder existing entries; only append.
