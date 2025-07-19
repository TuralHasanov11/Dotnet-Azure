---
applyTo: "**/*.cs,**/*.cshtml,**/*.razor"
---

# Project coding standards for C# and ASP.NET

## General Guidelines

- Use `Task.WhenEach` to perform common actions as each task completes.
- Retrieving the Process Id with `System.Environment.ProcessId`
- Extend `IParsable` interface to convert any type. Extension methods can be created as well
- Use extension method with `ConditionalWeakTable` to add additional props to class
- Use `ValidationContext` to inject services into `ValidationAttribute`
- Use `DataProtectorTokenProvider` and `DataProtectionTokenProviderOptions` base classes to create custom token providers to set token lifespan

## Infrastructure

- Use `TagWith` on `IQueryable` to tag unusual database queries

## Tests

- Use `BeforeAfterTestAttribute` for actions before and after a test
- Use `FakeTimeProvider` to mock date time.
