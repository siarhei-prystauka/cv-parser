# Prompt History

## Template

- Timestamp: YYYY-MM-DDTHH:MM:SS+TZ
  Request: <The Copilot command>
  Actions:
  - <key action>
  Notes: <optional>

## Entries

- Timestamp: 2026-02-07T15:05:00+01:00
  Request: Create prompt history file and update Copilot instructions.
  Actions:
  - Added prompt-history.md template.
  - Documented prompt history logging rules in Copilot instructions.

- Timestamp: 2026-02-07T15:42:00+01:00
  Request: Implement CV parser web application with full-stack features.
  Prompt: "We need to create a web application using the plan below and the context above: Scan workspace structure; Define UI layouts (MainLayout and AdminLayout); Build newcomer list UI with filtering/sorting/pagination; CV upload UI with PDF-only input; Skills extraction preview with edit/confirm; Mock API contracts; Implement .NET API (in-memory) with repository pattern and seeded profiles; Wire frontend to API; Config & hygiene (CORS, no secrets); Testing placeholders."
  Actions:
  - Installed Tailwind CSS, Headless UI, React Router, and testing dependencies.
  - Created MainLayout and AdminLayout components with navigation.
  - Built ProfilesPage with client-side filtering, sorting (by name/department/DOB), and pagination (5 per page).
  - Implemented CvUploadDialog with PDF-only file input, skills preview, and edit/confirm flow.
  - Created API client layer with TypeScript types (ProfileSummary, ProfileDetail, CvPreviewResponse, UpdateSkillsRequest).
  - Implemented .NET 10 Minimal API with versioned endpoints (/api/v1/profiles).
  - Added repository pattern (IProfileRepository, InMemoryProfileRepository) with 8 seeded profiles.
  - Created MockCvSkillExtractor service for deterministic skill extraction.
  - Configured CORS for localhost:5173, added validation and problem details responses.
  - Added minimal test scaffolds (Vitest for frontend, NUnit for backend).
  - Updated README with correct port (5028).
  Notes: All implementations follow repository patterns for future DB integration, use proper DTO separation, and include XML documentation comments per C# 14 guidelines.

- Timestamp: 2026-02-07T16:15:00+01:00
  Request: Start servers and create feature branch for implementation.
  Prompt: "Do all next steps. Open browser so I can see what was implemented" followed by "Create a new feature branch for the changes made and move the latest commit to main to this branch commit"
  Actions:
  - Installed frontend dependencies (npm install).
  - Started .NET API server on port 5028 (background terminal).
  - Started Vite dev server on port 5173 (background terminal).
  - Opened Simple Browser to localhost:5173.
  - Created feature/cv-parser-ui-and-api branch.
  - Reset main branch to align with origin/main (74f5966).
  - Moved implementation commit (f1497d1) to feature branch.
  - Updated prompt history and committed changes.
  Notes: Both dev servers remain running. Feature branch is now 2 commits ahead of main.

- Timestamp: 2026-02-07T16:30:00+01:00
  Request: Refactor Minimal API to controller-based architecture.
  Prompt: "why have you used ProfilesEndpoints instead of Controllers?" followed by "Refactor as you suggested and update project requirements/instructions accordingly"
  Actions:
  - Created ProfilesController with attribute routing and XML documentation.
  - Replaced Minimal API endpoints with controller actions (GetProfiles, GetProfile, PreviewCvSkills, UpdateSkills).
  - Updated Program.cs to use AddControllers() and MapControllers().
  - Replaced Microsoft.AspNetCore.OpenApi with Swashbuckle.AspNetCore 7.2.0.
  - Enabled XML documentation generation in CvParser.Api.csproj.
  - Added ProducesResponseType attributes for OpenAPI documentation.
  - Updated validation to use ModelState and ValidationProblem().
  - Deleted obsolete Endpoints/ProfilesEndpoints.cs file.
  - Updated aspnet-rest-api.instructions.md to prefer controllers for multi-endpoint APIs.
  - Updated README.md with architecture section and API endpoint documentation.
  - Updated .github/copilot-instructions.md with controller-based guidance.
  Notes: Controller approach provides better OpenAPI documentation, automatic model validation via [ApiController], and conventional routing patterns. API functionality remains identical—same routes, validation, and responses.

- Timestamp: 2026-02-14T10:45:00+01:00
  Request: Remove "Dto" suffix from API model names.
  Prompt: "remove dto prefix for model returned by asp.net controllers, update or delete corresponding instruction in the csharp.instructions.md if exists."
  Actions:
  - Renamed ProfileSummaryDto → ProfileSummary
  - Renamed ProfileDetailDto → ProfileDetail
  - Renamed CvPreviewResponseDto → CvPreviewResponse
  - Updated all references in ProfilesController.cs
  - Deleted old DTO files with Dto suffix
  - Verified build succeeds with no errors
  Notes: Frontend TypeScript types already used names without Dto suffix, so no frontend changes needed. No explicit DTO naming guidance existed in csharp.instructions.md to remove.

- Timestamp: 2026-02-14T11:00:00+01:00
  Request: Reorganize API models into Models folder structure.
  Prompt: "Create Response folder in Models folder, move files from Dtos folder to Response folder and remove Dtos folder. Update namespaces accordingly"
  Actions:
  - Created Models/Response folder with ProfileSummary, ProfileDetail, and CvPreviewResponse.
  - Created Models/Request folder with UpdateSkillsRequest.
  - Updated ProfilesController namespaces from CvParser.Api.Dtos to CvParser.Api.Models.Response and CvParser.Api.Models.Request.
  - Deleted Dtos folder and all its contents.
  - Verified build succeeds with no errors.
  Notes: API models now follow a clearer organization with separate Request and Response folders under Models, improving code structure and discoverability.

- Timestamp: 2026-02-14T11:05:00+01:00
  Request: Update folder names to plural convention.
  Prompt: "Update folder name to plural: Request -> Requests, Response -> responses"
  Actions:
  - Renamed Models/Request → Models/Requests
  - Renamed Models/Response → Models/Responses
  - Updated namespaces: CvParser.Api.Models.Request → CvParser.Api.Models.Requests
  - Updated namespaces: CvParser.Api.Models.Response → CvParser.Api.Models.Responses
  - Updated ProfilesController using statements.
  - Deleted old singular folders.
  - Verified build succeeds with no errors.
  Notes: Folder names now follow plural convention commonly used in ASP.NET Core projects for better consistency.

