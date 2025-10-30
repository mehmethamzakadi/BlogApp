# BlogApp Test Suite

## Test Coverage

### Domain.UnitTests ✅ (22 tests)
**Coverage**: Value Objects + Entities

#### Value Objects
- **EmailTests** (6 tests)
  - ✅ Valid email creation
  - ✅ Empty/null email validation
  - ✅ Invalid format validation
  - ✅ Equality comparison

- **UserNameTests** (8 tests)
  - ✅ Valid username creation
  - ✅ Empty/null username validation
  - ✅ Length validation (min 3, max 50)
  - ✅ Invalid characters validation
  - ✅ Equality comparison

#### Entities
- **UserTests** (4 tests)
  - ✅ User creation with valid data
  - ✅ User update
  - ✅ Delete adds domain event
  - ✅ Normalized fields (uppercase)

### Application.UnitTests ⚠️ (0 tests)
**Status**: Infrastructure setup complete, tests pending

**Planned Tests**:
- ValidationBehavior tests
- Command handler tests (Create, Update, Delete)
- Query handler tests
- AutoMapper profile tests

---

## Running Tests

### All Tests
```bash
dotnet test BlogApp.sln
```

### Domain Tests Only
```bash
dotnet test tests/Domain.UnitTests/Domain.UnitTests.csproj
```

### Application Tests Only
```bash
dotnet test tests/Application.UnitTests/Application.UnitTests.csproj
```

### With Coverage
```bash
dotnet test BlogApp.sln --collect:"XPlat Code Coverage"
```

---

## Test Structure

```
tests/
├── Domain.UnitTests/
│   ├── ValueObjects/
│   │   ├── EmailTests.cs
│   │   └── UserNameTests.cs
│   └── Entities/
│       └── UserTests.cs
└── Application.UnitTests/
    └── (pending)
```

---

## Test Framework

- **Test Runner**: NUnit 4.4.0
- **Mocking**: Moq 4.20.72
- **Coverage**: coverlet.collector 6.0.4

---

## Next Steps

### Priority 1: Application Layer Tests
- [ ] ValidationBehavior tests
- [ ] CreateUserCommandHandler tests
- [ ] LoginCommandHandler tests
- [ ] DeleteUserCommandHandler tests

### Priority 2: Domain Layer Tests
- [ ] Post entity tests
- [ ] Category entity tests
- [ ] Role entity tests
- [ ] UserDomainService tests

### Priority 3: Integration Tests
- [ ] API endpoint tests
- [ ] Repository tests
- [ ] Database integration tests

---

## Test Conventions

### Naming
- Test class: `{ClassName}Tests`
- Test method: `{MethodName}_{Scenario}_{ExpectedResult}`

### Structure (AAA Pattern)
```csharp
[Test]
public void MethodName_Scenario_ExpectedResult()
{
    // Arrange
    var input = "test";
    
    // Act
    var result = Method(input);
    
    // Assert
    Assert.That(result, Is.EqualTo(expected));
}
```

### Assertions
- Use `Assert.That()` (NUnit 3+ style)
- Avoid `Assert.AreEqual()` (legacy style)

---

## Current Status

**Total Tests**: 22  
**Passing**: 22 ✅  
**Failing**: 0  
**Coverage**: ~5% (Domain layer only)

**Target**: 60% coverage (120+ tests)

---

**Last Updated**: 2025-01-25
