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

