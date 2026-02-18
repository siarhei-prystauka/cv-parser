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

- Timestamp: 2026-02-14T11:15:00+01:00
  Request: Establish test naming conventions across C# and TypeScript.
  Prompt: "Generate test method naming instruction so the naming was <method_under_test>_<scenario>_<expected_result>" followed by "Yes" to update existing tests.
  Actions:
  - Created .github/instructions/testing.instructions.md with comprehensive test naming guidelines.
  - Established C# pattern: `<MethodUnderTest>_<Scenario>_<ExpectedResult>` (PascalCase).
  - Established TypeScript pattern: `<methodUnderTest> - <scenario> - <expected result>` (camelCase/sentence case).
  - Updated InMemoryProfileRepositoryTests: GetAll_ReturnsSeededProfiles → GetAll_OnInitialization_ReturnsSeededProfiles.
  - Updated ProfilesPage.test.tsx: "renders the main heading" → "render - on mount - displays heading".
  - Updated csharp.instructions.md to reference testing naming convention.
  - Updated reactjs.instructions.md to reference testing naming convention.
  - Verified test project builds successfully.
  Notes: Test naming now follows a consistent pattern that clearly describes what is being tested, under what conditions, and what the expected outcome is. This improves test readability and maintainability.

- Timestamp: 2026-02-14T11:20:00+01:00
  Request: Remove remaining Dto references from ProfilesController.
  Prompt: "ProfileController still contains Dto prefix in method names etc. Fix this"
  Actions:
  - Renamed ToSummaryDto() → ToSummary()
  - Renamed ToDetailDto() → ToDetail()
  - Updated XML comments: "Converts a profile into a summary DTO" → "Converts a profile into a summary response model"
  - Updated XML comments: "Converts a profile into a detail DTO" → "Converts a profile into a detail response model"
  - Updated all method invocations in GetProfiles(), GetProfile(), and UpdateSkills()
  - Verified build succeeds with no errors
  Notes: All references to "Dto" have been removed from the controller, completing the transition away from DTO naming conventions to cleaner response model names.

- Timestamp: 2026-02-14T11:25:00+01:00
  Request: Extract converter methods to dedicated class.
  Prompt: "Create Converters folder in the root and move these converter method to a separate classes"
  Actions:
  - Created Converters/ProfileConverter.cs with static ToSummary() and ToDetail() methods.
  - Updated ProfilesController to use ProfileConverter.ToSummary() and ProfileConverter.ToDetail().
  - Added using CvParser.Api.Converters to ProfilesController.
  - Removed private converter methods from ProfilesController.
  - Verified build succeeds after clean and rebuild.
  Notes: Converter methods are now in a dedicated ProfileConverter class, improving separation of concerns and making the controller more focused on HTTP request handling rather than data transformation.

- Timestamp: 2026-02-14T11:40:00+01:00
  Request: Convert ProfileConverter from static to instance-based injectable service.
  Prompt: "Converter should not be static classes with static methods, fix it"
  Actions:
  - Created IProfileConverter interface in Converters/IProfileConverter.cs.
  - Changed ProfileConverter from static class to sealed instance class implementing IProfileConverter.
  - Removed static keywords from ToSummary and ToDetail methods.
  - Updated ProfilesController constructor to inject IProfileConverter.
  - Changed all ProfileConverter.ToSummary/ToDetail() calls to _converter.ToSummary/ToDetail().
  - Registered IProfileConverter as scoped service in Program.cs.
  - Fixed corrupted controller methods (GetProfile and UpdateSkills) with proper null checks.
  - Verified build successful with no errors.
  Notes: Following dependency injection best practices; converters are now testable services. This allows for easier mocking in unit tests and follows SOLID principles (interface segregation, dependency inversion).

- Timestamp: 2026-02-14T16:30:00+01:00
  Request: Implement CV extraction pipeline with PDF parsing and Groq LLM integration.
  Prompt: "We need to implement CV extraction functionality. User should be able upload newcomer's CV in PDF format. For example, extract hard IT skills: .NET, MS SQL Server, soft skills: team management etc. Research .NET libraries from trust sources. Let's use Groq. Start implementation"
  Actions:
  - Added NuGet packages: UglyToad.PdfPig (0.1.9), DocumentFormat.OpenXml (3.2.0), Microsoft.Extensions.Http.Polly (8.0.0).
  - Created text extraction layer: ICvTextExtractor, PdfTextExtractor (uses PdfPig ContentOrderTextExtractor), DocxTextExtractor (skeleton for future), CvTextExtractorFactory.
  - Created Groq LLM integration: ILlmSkillExtractor, GroqSkillExtractor with OpenAI-compatible API, JSON mode, retry policy.
  - Created skills-taxonomy.json with 47 common IT skills (languages, frameworks, databases, cloud, tools) plus aliases.
  - Implemented HybridCvSkillExtractor: extracts text → matches taxonomy first → LLM fallback → merges/deduplicates results.
  - Updated appsettings.json with Groq config (BaseUrl, Model: llama-3.1-8b-instant, Timeouts, MaxTokens) and FileValidation (10MB limit, PDF only).
  - Updated ProfilesController: added IConfiguration injection, replaced ValidatePdfFile with ValidateCvFile (10MB check), updated PreviewCvSkills signature (contentType parameter).
  - Updated ICvSkillExtractor interface: changed signature to include contentType parameter instead of CancellationToken.
  - Updated Program.cs: registered PdfTextExtractor, CvTextExtractorFactory, configured HttpClient for GroqSkillExtractor with retry policy, replaced MockCvSkillExtractor with HybridCvSkillExtractor.
  - Updated frontend CvUploadDialog: added MAX_FILE_SIZE_MB (10), handleFileChange with client-side validation (size + PDF type), displays selected file size, improved error messaging.
  - Created GitHub issue template for DOCX support at /tmp/github-issue-docx.md (medium priority, deferred for now).
  - Removed DOCX support from current implementation (PDF only): commented out DocxTextExtractor registration, updated validation to PDF-only, reverted frontend accept attribute.
  Notes: DocxTextExtractor.cs kept in codebase for future GitHub issue. API key stored in Groq:ApiKey config (environment variable in production). Hybrid approach prioritizes taxonomy for speed/consistency, uses LLM for edge cases. All error handling with proper logging at each layer.

- Timestamp: 2026-02-15T18:25:00+01:00
  Request: Check and prepare fixes for all unresolved comments to PR #6
  Actions:
  - Reviewed all 22 PR review comments; identified 5 unresolved.
  - Created GitHub issue #7 for configurable LLM fallback mode (comment #1 — design decision deferred to Admin page).
  - Fixed HttpResponseMessage disposal in GroqSkillExtractor (added `using var response`).
  - Refactored ValidateCvFile in ProfilesController to read SupportedContentTypes from appsettings.json instead of hard-coding PDF. Added content-type-to-extension map for fallback validation.
  - Updated CvUploadDialog client-side validation to also check file extension as fallback (not just MIME type).
  - Fixed file input not clearing on validation failure in CvUploadDialog (added `event.currentTarget.value = ''`).
  Notes: Build succeeded, 2/2 tests pass. Changes not committed per user request.

- Timestamp: 2026-02-15T19:00:00+01:00
  Request: Refactor IConfiguration usage to Options Pattern.
  Prompt: "Analyze IConfiguration usage in the project and suggest better alternatives" followed by "yes"
  Actions:
  - Created Models/Options/GroqOptions.cs with data annotations validation (Required, Url, Range).
  - Created Models/Options/FileValidationOptions.cs with data annotations validation.
  - Updated Program.cs to register both options with BindConfiguration, ValidateDataAnnotations, and ValidateOnStart.
  - Refactored GroqSkillExtractor: replaced IConfiguration injection with IOptions<GroqOptions>, removed manual config parsing and validation, simplified code.
  - Refactored ProfilesController: replaced IConfiguration injection with IOptions<FileValidationOptions>, updated ValidateCvFile method.
  - Refactored ServiceCollectionExtensions: updated HttpClient configuration to use IOptions<GroqOptions> from service provider.
  - Verified build succeeds with no errors.
  Notes: All configuration now uses strongly-typed Options Pattern with compile-time safety, IntelliSense support, and fail-fast validation at startup. Eliminates magic strings, improves testability, and follows .NET best practices.

- Timestamp: 2026-02-15T19:30:00+01:00
  Request: Fix invisible navigation link to Profiles page on Admin page.
  Actions:
  - Fixed AdminLayout nav link classes: changed `text-ink` to `text-bone` so links are visible on the dark (`bg-ink`) background.
  - Renamed "Main" nav link to "Profiles" for clarity.

- Timestamp: 2026-02-15T19:45:00+01:00
  Request: Change Admin page background to a color similar to Profiles page but darker.
  Actions:
  - Added `sand` color (`#d9d2c5`) to Tailwind config — a darker warm tone of `bone`.
  - Updated AdminLayout background from `bg-ink` to `bg-sand` with `text-ink` for dark text.
  - Updated AdminPage card to use `bg-bone` with `text-ink` and `bg-sand/50` for inner container.
  - Adjusted all opacity/border classes for legibility on the lighter background.

- Timestamp: 2026-02-15T20:00:00+01:00
  Request: Centralize options registration by moving it into AddApplicationServices extension method.
  Actions:
  - Moved GroqOptions and FileValidationOptions registration (BindConfiguration, ValidateDataAnnotations, ValidateOnStart) from Program.cs into ServiceCollectionExtensions.AddApplicationServices.
  - Removed unused IConfiguration parameter from AddApplicationServices method signature.
  - Removed CvParser.Api.Models.Options using and options registration block from Program.cs.
  - Updated call site in Program.cs to parameterless AddApplicationServices().
  Notes: Build succeeds with 0 warnings, 0 errors. DI setup is now fully centralized in the extension method.

- Timestamp: 2026-02-15T20:30:00+01:00
  Request: Implement frontend for issue #7 - configurable LLM skill extraction.
  Prompt: "You are implementing the frontend for issue #7 - configurable LLM skill extraction. The backend is complete, now implement the React frontend."
  Actions:
  - Updated src/CvParser.Web/src/api/client.ts: Added settings types import, created settingsApi with getSettings(), updateSettings(), and getTaxonomy() methods following profileApi pattern.
  - Created src/CvParser.Web/src/components/SettingsForm.tsx: Form with LlmFallbackOnly toggle switch, Groq config inputs (baseUrl, model, timeoutSeconds, maxTokens), form validation with error messages, save button with loading state, success/error status messages. Uses sand/bone/ink Tailwind theme.
  - Created src/CvParser.Web/src/components/TaxonomyViewer.tsx: Displays TaxonomySkill[] grouped by category with collapsible sections, search/filter input, expand/collapse all buttons, shows skill names and aliases. Clean read-only design with sand/bone/ink theme.
  - Updated src/CvParser.Web/src/pages/AdminPage.tsx: Replaced placeholder with full implementation. Fetches settings and taxonomy on mount using Promise.all, shows loading/error states, renders SettingsForm and TaxonomyViewer in two-column layout (responsive), handles save with success/error feedback, refreshes settings after successful save.
  Notes: All components follow existing patterns from ProfilesPage and CvUploadDialog. TypeScript types use no 'any', proper error handling with user-friendly messages, responsive design with Tailwind, loading states for all async operations. Frontend verified with 0 TypeScript errors.

- Timestamp: 2026-02-15T21:00:00+01:00
  Request: Write backend tests for issue #7 - configurable LLM skill extraction.
  Prompt: "You are writing backend tests for issue #7 - configurable LLM skill extraction."
  Actions:
  - Added NSubstitute 5.3.0 NuGet package to CvParser.Api.Tests.csproj for mocking dependencies.
  - Created src/CvParser.Api.Tests/TestData/Data/skills-taxonomy.json: Minimal test taxonomy with 3 skills (C#, Python, React) for HybridCvSkillExtractor tests. Configured to copy to output directory.
  - Created src/CvParser.Api.Tests/Repositories/InMemorySettingsRepositoryTests.cs: 5 tests covering GetSkillExtractionOptionsAsync returns initial values, GetGroqOptionsAsync returns initial values, UpdateSkillExtractionOptionsAsync persists changes, UpdateGroqOptionsAsync persists changes, concurrent updates with 50 threads to verify thread safety. All async/await with NUnit 4.
  - Created src/CvParser.Api.Tests/Services/HybridCvSkillExtractorTests.cs: 7 tests covering LlmFallbackOnly=false always calls LLM, LlmFallbackOnly=true with taxonomy matches skips LLM, LlmFallbackOnly=true without taxonomy matches calls LLM (fallback), taxonomy alias matching returns canonical names, duplicate skills are deduplicated, empty text returns empty list. Mocks ICvTextExtractorFactory, ILlmSkillExtractor, IWebHostEnvironment, ISettingsRepository, ILogger using NSubstitute. Verifies LLM extractor calls with Received()/DidNotReceive().
  Notes: All 13 tests pass (5 InMemorySettingsRepositoryTests + 7 HybridCvSkillExtractorTests + 1 existing InMemoryProfileRepositoryTests). Followed C# instructions with summaries on test classes only, proper NUnit attributes ([Test], [TestFixture], [SetUp]), naming convention <Method>_<Scenario>_<ExpectedResult>. Tests validate core issue #7 logic: configurable LLM fallback behavior and thread-safe settings updates.

- Timestamp: 2026-02-15T21:15:00+01:00
  Request: Rewrite SettingsForm.test.tsx after component refactoring.
  Prompt: "Rewrite the frontend test file for SettingsForm component. The component has been refactored: 'Groq Configuration' renamed to 'LLM Configuration', only the 'model' field is editable (via dropdown), baseUrl/timeoutSeconds/maxTokens are NO LONGER user-editable (application config). Settings structure changed: groq → llm. LlmSettings now has: { model: string, availableModels: string[] }. UpdateSettingsRequest now has: { skillExtraction: { llmFallbackOnly: boolean }, llm: { model: string } }."
  Actions:
  - Completely rewrote src/CvParser.Web/src/components/__tests__/SettingsForm.test.tsx: Removed all 14 tests for timeout/maxTokens/baseUrl validation (fields no longer editable). Kept 8 core tests (render mount, render initial values, render model dropdown contents, toggle llmFallbackOnly switch false→true and true→false, submit with valid data, submit success message, submit error message, submit loading state). Added 1 new test: change model selection updates form data. Updated mockSettings to match new structure with llm.model and llm.availableModels. All tests validate the LLM Configuration dropdown (not input fields).
  - Verified all 10 tests pass with vitest.
  Notes: Test suite now matches simplified SettingsForm component where only model selection (via dropdown) is user-editable. Removed 15 obsolete tests for fields moved to application config. All tests follow TypeScript naming convention (<methodUnderTest> - <scenario> - <expected result>).

- Timestamp: 2026-02-16T21:46:22+01:00
  Request: Start implementation
  Actions:
  - Added GitHub Actions workflows for CI, API deploy, and web deploy with Azure free-tier targets.
  - Updated API CORS configuration to read AllowedOrigins from configuration with development defaults.
  - Documented Azure deployment setup and required secrets in README.

- Timestamp: 2026-02-17T12:00:00+01:00
  Request: Update CORS for production — origin https://happy-ocean-0900fd003.4.azurestaticapps.net was blocked
  Actions:
  - Added production Static Web App origin to AllowedOrigins in appsettings.json.

- Timestamp: 2026-02-17T21:45:00+01:00
  Request: Analyze production issue — HTTPS redirection warning and forwarded headers
  Actions:
  - Added UseForwardedHeaders middleware to support Azure App Service reverse proxy.
  - Moved UseHttpsRedirection to development-only to avoid warning behind TLS-terminating proxy.

- Timestamp: 2026-02-18T00:00:00+01:00
  Request: Pass VITE_API_BASE_URL environment variable to the deploy-web GitHub Actions workflow.
  Actions:
  - Added env block to the Deploy to Azure Static Web Apps step in deploy-web.yml, referencing the repository variable vars.VITE_API_BASE_URL.
  Notes: The VITE_API_BASE_URL repository variable must be created in GitHub repo Settings → Secrets and variables → Actions → Variables.
