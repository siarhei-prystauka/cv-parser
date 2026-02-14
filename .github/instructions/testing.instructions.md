---
description: 'Guidelines for writing tests in C# and TypeScript'
applyTo: '**/*.test.ts, **/*.test.tsx, **/*Tests.cs'
---

# Testing Guidelines

## Test Naming Conventions

### C# (NUnit, xUnit, MSTest)
- Use the pattern: `<MethodUnderTest>_<Scenario>_<ExpectedResult>`
- Use PascalCase for all parts of the name
- Be descriptive but concise

**Examples:**
```csharp
[Test]
public void GetById_ProfileExists_ReturnsProfile() { }

[Test]
public void GetById_ProfileDoesNotExist_ReturnsNull() { }

[Test]
public void UpdateSkills_EmptySkillsList_ThrowsArgumentException() { }

[Test]
public void ExtractSkillsAsync_ValidPdf_ReturnsNonEmptyList() { }
```

### TypeScript (Vitest, Jest, React Testing Library)
- Use the pattern: `<methodUnderTest> - <scenario> - <expected result>`
- Use camelCase for method names, sentence case for scenario and result
- Wrap in `describe` and `it`/`test` blocks

**Examples:**
```typescript
describe('ProfilesPage', () => {
  it('render - on mount - displays heading', () => {});
  
  it('searchFilter - empty search - shows all profiles', () => {});
  
  it('uploadCv - invalid file type - shows error message', () => {});
  
  it('sortProfiles - by name ascending - displays alphabetically', () => {});
});
```

## General Testing Principles

- **Arrange-Act-Assert (AAA)**: Structure tests clearly with setup, execution, and verification
- **One assertion per test**: Focus each test on a single behavior
- **Test behavior, not implementation**: Avoid coupling tests to internal details
- **Use meaningful test data**: Make test data representative of real scenarios
- **Isolate tests**: Each test should be independent and not rely on test execution order

## NUnit-Specific

- Use `[TestFixture]` for test classes
- Use `[Test]` for test methods
- Use `[SetUp]` and `[TearDown]` for test initialization and cleanup
- Use `[TestCase]` for parameterized tests
- Prefer `Assert.That()` over classic assertions

## React Testing Library

- Query by accessibility roles and labels first (`getByRole`, `getByLabelText`)
- Use `userEvent` for user interactions instead of `fireEvent`
- Test user-facing behavior, not implementation details
- Use `waitFor` for async assertions
- Mock API calls at the network boundary

## Coverage Goals

- Aim for meaningful coverage of critical paths
- Prioritize business logic and edge cases
- Don't write tests just to increase percentage
- Focus on valuable tests that catch real bugs
