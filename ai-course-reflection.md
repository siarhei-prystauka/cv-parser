# AI-Assisted Development Reflection

**Project**: CV Parser — a full-stack web application that reads a new employee's CV, extracts hard and soft skills using an LLM, and saves them in a structured format.  
**Course**: AI Tools Practicum (Company Internal)  
**Author**: Siarhei Prystauka

---

## 1. Project Overview

The goal of this exercise was to build a full-stack application from scratch using AI-assisted development tools as the primary driver of implementation. The project combined a .NET 10 Web API backend with a React TypeScript frontend, a Groq-hosted LLM for skill extraction, and an Azure-hosted deployment pipeline.

The completed application allows HR staff to browse employee profiles, upload a PDF CV, preview and edit the AI-extracted skills, and save the result. An Admin page exposes runtime settings including the LLM model selection and whether to use the LLM on every extraction or only as a fallback to the local skills taxonomy.

---

## 2. Context Provided to GitHub Copilot

A structured context layer was established upfront to guide Copilot across all sessions. The key components:

**`.github/copilot-instructions.md`** — defined the technology stack (.NET 10, React 18, TypeScript, Vite), architectural rules (controller-based APIs, repository pattern, SOLID), a strict XML documentation policy (only `/// <summary>` on public controllers — no `<param>`, `<returns>`, or `<response>` tags), a ban on alpha/preview packages, and a prompt history logging convention that required Copilot itself to append a timestamped entry to `prompt-history.md` after every session.

**Five glob-scoped instruction files** in `.github/instructions/` covered C# conventions, ASP.NET REST API design, React/TypeScript standards, test naming patterns (`<MethodUnderTest>_<Scenario>_<ExpectedResult>`), and GitHub Actions CI/CD best practices. Because these files are scoped to file patterns, Copilot applied the rules automatically without being reminded in each prompt.

The prompt-history logging rule in particular proved valuable: it created a full audit trail across sessions spanning multiple days, making it possible to reconstruct the complete development narrative for this reflection.

---

## 3. Tools and Models Used

**GitHub Copilot in VS Code** was the sole implementation tool, used in two modes:

- **Plan mode** — for architecture discussions and reviewing proposals before committing to changes (e.g. the conversation that led to the Minimal API → Controllers refactor).
- **Agent mode** — for all file creation, editing, terminal execution, and multi-file refactors. The agent read existing code, ran builds and tests, and iterated until the build was clean.

**Model**: Claude Sonnet 4.6 (Anthropic), accessed via GitHub Copilot Chat. It demonstrated consistent adherence to the project-specific `.github/` instructions across all sessions and handled cross-cutting refactors (rename class, update all references, controllers, and tests in one turn) reliably.

---

## 4. MCP Servers Used

**GitHub MCP** was used in the following ways: (1) to fetch all 22 review comments on PR #6, identify the 5 unresolved ones, and use them as the task list for a fix session — avoiding manual copy-paste of comment context into the prompt; (2) to create GitHub issue #7 ("Configurable LLM fallback mode") directly from the agent session, then use that issue as the acceptance criteria for the next implementation session.

**Azure MCP** was used during CI/CD design to retrieve Azure best practices: it guided the choice of OIDC federated credentials over long-lived service principal secrets, provided correct `azure/webapps-deploy@v3` configuration, confirmed the correct placement of `UseForwardedHeaders` middleware behind the Azure App Service reverse proxy, and explained why `UseHttpsRedirection` must be disabled in production behind a TLS-terminating proxy (it causes redirect loops when Azure Static Web Apps terminates TLS upstream).

---

## 5. Prompt Log — Phase by Phase

### Phase 1 — Initial Full-Stack Scaffold

**Prompt**:

> "We need to create a web application using the plan below and the context above: Scan workspace structure; Define UI layouts (MainLayout and AdminLayout); Build newcomer list UI with filtering/sorting/pagination; CV upload UI with PDF-only input; Skills extraction preview with edit/confirm; Mock API contracts; Implement .NET API (in-memory) with repository pattern and seeded profiles; Wire frontend to API; Config & hygiene (CORS, no secrets); Testing placeholders."

**Result**: Copilot produced the entire initial application in a single agent turn. It installed Tailwind CSS, Headless UI, React Router, and Vitest; created MainLayout and AdminLayout; built ProfilesPage with client-side search, sort (name / department / DOB), and pagination (5 per page); implemented CvUploadDialog with PDF file validation, skills preview, and an edit-then-confirm flow; created a .NET 10 API using Minimal API endpoints, an in-memory repository with 8 seeded profiles, a `MockCvSkillExtractor`, and CORS configured for `localhost:5173`.

**Accepted / Changed**: The prompt was deliberately written as a comprehensive plan rather than a single vague request. Almost everything was accepted. The one notable gap was that Copilot used Minimal API endpoints (`ProfilesEndpoints.cs`) rather than controllers — this was caught and corrected in Phase 2.

---

### Phase 2 — Architecture Refactor: Minimal API → Controllers

**Prompt** (plan mode conversation, then agent):

> "Why have you used ProfilesEndpoints instead of Controllers?" — then after the explanation — "Refactor as you suggested and update project requirements/instructions accordingly."

**Result**: Copilot created `ProfilesController` with `[ApiController]` attribute routing, replaced `Microsoft.AspNetCore.OpenApi` with `Swashbuckle.AspNetCore 7.2.0`, enabled XML documentation generation in the `.csproj`, added `ProducesResponseType` attributes, updated `Program.cs` to use `AddControllers()` and `MapControllers()`, deleted `ProfilesEndpoints.cs`, and updated `aspnet-rest-api.instructions.md` to record the preference for controllers. All in one agent turn.

**Accepted / Changed**: Accepted entirely. The instruction file update was particularly valuable — by writing the decision into `.github/instructions/aspnet-rest-api.instructions.md`, the preference was automatically respected in all future sessions without needing to re-explain it.

---

### Phase 3 — Code Quality Cleanup

This phase comprised five short sessions, each fixing a specific convention issue:

**Dto suffix removal**: "Remove dto prefix for model returned by ASP.NET controllers." Copilot renamed `ProfileSummaryDto` → `ProfileSummary`, `ProfileDetailDto` → `ProfileDetail`, `CvPreviewResponseDto` → `CvPreviewResponse`, updated all references, and deleted the old files. The frontend TypeScript types already used names without the `Dto` suffix, so no frontend changes were required.

**Models folder reorganisation**: "Create Response folder in Models folder, move files from Dtos folder to Response folder." Copilot created `Models/Requests` and `Models/Responses` folders, moved the files, updated namespaces throughout, and deleted the old `Dtos` folder.

**Converter extraction**: "Create Converters folder in the root and move these converter methods to a separate class." Copilot extracted the private `ToSummaryDto()` / `ToDetailDto()` methods from `ProfilesController` into a static `ProfileConverter` class, then on the follow-up — "Converter should not be static classes with static methods, fix it" — converted it to `IProfileConverter` / `ProfileConverter` as a scoped injectable service. This makes the converter mockable in tests.

**Test naming conventions**: "Generate test method naming instruction so the naming was `<method_under_test>_<scenario>_<expected_result>`." Copilot created `testing.instructions.md`, updated `csharp.instructions.md` and `reactjs.instructions.md` to cross-reference it, and renamed all existing tests to follow the new pattern.

**Accepted / Changed**: All accepted. The pattern of issuing small, focused correction prompts rather than accumulating technical debt proved effective — each session was short, the diff was clean, and the build stayed green throughout.

---

### Phase 4 — CV Extraction Pipeline with Groq LLM

**Prompt**:

> "We need to implement CV extraction functionality. User should be able to upload newcomer's CV in PDF format. Extract hard IT skills: .NET, MS SQL Server, soft skills: team management etc. Research .NET libraries from trusted sources. Let's use Groq. Start implementation."

**Result**: Copilot designed and implemented a complete extraction pipeline: `PdfTextExtractor` using `UglyToad.PdfPig` for text extraction; `GroqSkillExtractor` calling the Groq API (OpenAI-compatible endpoint) with JSON mode and a Polly retry policy (1 s, 2 s, 5 s + 429 handling); a `skills-taxonomy.json` data file with 47 canonical IT skills and their aliases; `HybridCvSkillExtractor` implementing a taxonomy-first strategy with LLM fallback; and `ICvTextExtractorFactory` for format dispatch. The `appsettings.json` was updated with Groq configuration (base URL, model, timeout, max tokens) and file validation settings (10 MB limit, PDF only). The frontend `CvUploadDialog` gained client-side file size and MIME type validation.

**Accepted / Changed**: Accepted, with one notable addition: the prompt did not mention a skeleton `DocxTextExtractor` for future DOCX support, but Copilot created one anyway and noted in the prompt history entry that a GitHub issue should be raised. This was accepted because it preserved future extensibility without adding runtime complexity. The agent also correctly chose `UglyToad.PdfPig` (stable, pure .NET, no native dependencies) over alternatives like `iTextSharp` (LGPL licensing concerns) and `PdfSharpCore` (less maintained) — demonstrating that the instruction to avoid preview packages and prefer well-maintained libraries influenced library selection.

---

### Phase 5 — Robustness, Configuration, and Feature Completeness

This phase addressed three concerns in sequence:

**PR review fix session**: Using the GitHub MCP, the agent read all 22 comments from PR #6, identified 5 unresolved issues, and fixed: `HttpResponseMessage` disposal (`using var response`), hardcoded MIME types moved to config (`SupportedContentTypes` in `FileValidationOptions`), MIME-to-extension fallback map in `ValidateCvFile`, frontend extension fallback validation, and file input clearing on validation failure.

**Options Pattern refactor**: "Analyse IConfiguration usage in the project and suggest better alternatives." Copilot identified three places where `IConfiguration` was injected and manually parsed with string keys, then replaced them with strongly-typed options classes (`GroqOptions`, `FileValidationOptions`, `SkillExtractionOptions`) registered with `ValidateDataAnnotations()` and `ValidateOnStart()` — meaning invalid configuration fails at startup rather than at first request. The `IConfiguration` parameter was also removed from `AddApplicationServices` entirely.

**Issue #7 implementation — configurable LLM fallback**: Using GitHub issue #7 as the specification, Copilot implemented a `SettingsController` with GET `/api/v1/settings`, PUT `/api/v1/settings`, and GET `/api/v1/settings/taxonomy` endpoints; an `InMemorySettingsRepository` with thread-safe updates via `SemaphoreSlim`; a `SettingsForm` React component with an `LlmFallbackOnly` toggle and LLM model dropdown; a `TaxonomyViewer` component with category grouping, search, and collapsible sections; and a complete test suite (NSubstitute mocks, 13 tests total) covering repository thread safety and the hybrid extractor's branching logic.

**Accepted / Changed**: The Options Pattern refactor was accepted entirely — this is one of the prompts where Copilot's suggestion was architecturally superior to what the developer had written. The thread-safety implementation in `InMemorySettingsRepository` (using `SemaphoreSlim`) was also generated without being prompted; the developer had not specified this requirement, but the agent inferred it correctly from the concurrent-access context.

---

### Phase 6 — Deployment and CI/CD

**Prompt** (initial):

> "Start implementation" (with Azure deployment as the implied goal from the project README).

**Result**: Copilot designed four GitHub Actions workflows: `api-ci.yml` and `web-ci.yml` for build/test on pull requests and main pushes; `deploy-api.yml` for publishing to Azure App Service using OIDC federated credentials; `deploy-web.yml` for deploying to Azure Static Web Apps. The CORS configuration was updated to read `AllowedOrigins` from configuration rather than being hardcoded. The README was updated with deployment setup instructions and required secrets.

**Post-deploy fixes**: Two production issues were discovered after the first deployment:
1. The Static Web App origin (`https://happy-ocean-0900fd003.4.azurestaticapps.net`) was blocked by CORS — fixed by adding it to `AllowedOrigins` in `appsettings.json`.
2. An HTTPS redirection warning appeared in App Service logs. Using the Azure MCP, the agent confirmed this was caused by `UseHttpsRedirection` firing behind a TLS-terminating proxy. The fix: move HTTPS redirection to development-only and add `UseForwardedHeaders` middleware.
3. A final refinement scoped all four workflows to path filters, so API workflow runs don't trigger on frontend changes and vice versa.

**Accepted / Changed**: The OIDC credential approach (suggested by Azure MCP guidance) was accepted over the simpler but less secure publish-profile approach. The path scoping refactor (replacing a single `ci.yml` with separate `api-ci.yml` and `web-ci.yml`) was accepted as it reduced unnecessary workflow runs in a monorepo layout.

---

## 6. Insights

### What worked well

**Rich upfront context pays off throughout**: The time spent writing `.github/copilot-instructions.md` and the five instruction files was recovered immediately. Across 20+ agent sessions, Copilot never generated a Minimal API when the instructions said to use controllers, never added `/// <param>` tags after being told not to, and never suggested alpha NuGet packages. Context established once propagated automatically to every subsequent session.

**"Architecture first, then implementation per file" pattern**: The most productive sessions followed a clear pattern: start in plan mode to discuss and agree on the approach, then switch to agent mode to implement. The Phase 4 LLM pipeline session worked this way — a brief plan-mode conversation agreed on the hybrid taxonomy-first strategy before implementation began, and the agent built it correctly in one turn.

**Small, focused correction prompts**: Short targeted prompts like "Converter should not be static classes, fix it" worked much better than large compound prompts when correcting a single concern. The agent understood the scope immediately and the diff was clean.

**Using MCP servers to avoid context copy-paste**: Directing Copilot to read PR review comments via the GitHub MCP rather than pasting them into the chat window produced cleaner sessions. The agent could reference the original comment text, link its changes to specific comments, and report which comments had been resolved.

**Letting the agent infer unstated requirements**: In several cases the agent added correctness improvements that were not explicitly requested — thread safety in `InMemorySettingsRepository`, a `DocxTextExtractor` skeleton for future extensibility, a Polly retry policy for the Groq HTTP client. These inferences were consistently correct and appropriate given the project context.

### What did not work well

**Vague one-word prompts**: The "Start implementation" prompt in Phase 6 was too open-ended. The agent produced working workflows, but it had to make many assumptions about Azure targets, environment names, and secret names. A more specific prompt would have reduced the number of post-deploy correction cycles.

**Compound multi-concern prompts can lose details**: The initial scaffold prompt in Phase 1 was long and comprehensive, which was mostly good, but the agent silently chose Minimal API over controllers — a choice buried in the combination of many requirements. For large initial prompts, a post-generation review pass is essential.

**Missing immediate test verification**: In Phase 3, several refactoring sessions (Dto removal, folder reorganisation) were completed without the agent being instructed to run the test suite after each change. Relying on "build succeeds" as the only gate meant that tests providing coverage of the changed code would have caught regressions earlier if explicitly invoked.

**Code quality is not consistently high**: Even with detailed instruction files, the generated code is not always production-grade on the first attempt. The agent sometimes produced overly verbose methods, inconsistent error handling styles between files, or missed edge cases that only surfaced during PR review (as evidenced by 22 review comments on PR #6, 5 of which required explicit fixes). AI-generated code still requires careful human review — treating the first output as "done" leads to technical debt that is harder to spot than hand-written code because it looks polished on the surface.

### Best prompting patterns observed

1. **Establish global context before the first implementation session.** Instruction files and `.github/copilot-instructions.md` are more effective than repeating constraints in every prompt.
2. **Plan mode for architecture, agent mode for implementation.** Using plan mode to agree on approach before switching to agent mode produces cleaner, better-targeted diffs.
3. **Reference external artefacts via MCP rather than pasting them.** PR comments, GitHub issues, and Azure documentation retrieved live through MCP servers give the agent accurate, current context.
4. **One concern per correction prompt.** When something is wrong, isolate it. "Remove Dto suffix from models" and "move converter to injectable service" are each one clean session. Combining them risks partial application.
5. **Ask the agent to update instruction files after architecture decisions.** "Update the instructions accordingly" at the end of a refactoring prompt ensures the decision is encoded permanently and not repeated in future sessions.
6. **Iterate on production issues with an MCP-informed agent.** Post-deploy bugs like CORS origin mismatches and reverse-proxy behaviour are well-documented. Pointing the agent to Azure MCP documentation rather than guessing produces correct, first-principles fixes rather than trial-and-error patches.