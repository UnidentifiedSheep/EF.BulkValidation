# EF.BulkValidation

A library for efficient batch data validation in Entity Framework Core. Combine validations (e.g., for database record existence) into a single SQL query, which will significantly reduce the number of database calls.

## Key features

- **Batch Validation**: Combine multiple existence checks into a single efficient SQL query.
- **Source Generation**: Automatically generate typed extension methods for `ValidationPlan` based on the attributes in your entities.
- **PostgreSQL Support**: Implementation for Npgsql and Entity Framework Core.
- **Flexibility**: Support for single keys, composite keys (tuples).

## Installation

Install the NuGet package:

```bash
dotnet add package ???
```

Register the DbValidator in your DI container (as generic type parameter provide your DbContext):
```csharp
using BulkValidation.Pgsql.Extensions;

// ...
services.AddPgsqlDbValidators<MyDbContext>();
```

## Usage

### 1. Mark entity properties with `[Validate]` or `[ValidateTuple]` attributes

Add `[Validate]` attributes to generate checks for individual fields or `[ValidateTuple]` for composite checks.

```csharp
using BulkValidation.Core.Attributes;

public partial class User
{
    [Validate]
    public Guid Id { get; set; }

    [Validate]
    [ValidateTuple("EmailAndPhone")]
    public string Email { get; set; }

    [ValidateTuple("EmailAndPhone")]
    public string Phone { get; set; }
}
```

### 2. Validation

Use the generated methods to create a validation plan.

```csharp
using BulkValidation.Core.Plan;
using BulkValidation.Core.Interfaces;
using BulkValidation.Pgsql.DbValidators;

public class UserService
{
    private readonly IDbValidator<MyDbContext, NpgsqlParameter> _dbValidator;

    public UserService(IDbValidator<MyDbContext, NpgsqlParameter> dbValidator)
    {
        _dbValidator = dbValidator;
    }

    public async Task ValidateUsers(IEnumerable<Guid> userIds)
    {
        var plan = new ValidationPlan();

        // Using the generated method for a single field
        foreach (var id in userIds)
        {
            plan.ValidateUserExistsId(id);
        }

        // Using the generated method for a composite key (Tuple)
        plan.ValidateUserExistsEmailAndPhone(("user@example.com", "+123456789"));

        // Performing validation in a single request
        var results = await _dbValidator.Validate(plan, throwOnError: false);
        
        if (results.Any())
        {
            // Error handling
        }
    }
}
```

## Performance

Using `EF.BulkValidation` avoids the N+1 problem when checking for data existence.
You can see the benchmark results [here](docs/benchmarks.md).

## Project structure

- `BulkValidation.Core`: Basic abstractions, rules, and validation plan model.
- `BulkValidation.Pgsql`: Implementation of the validator and SQL builder for PostgreSQL.
- `BulkValidation.SourceGeneration`: Code generator for creating `Validate{Entity}Exists{Property}` extension methods.

## License

The project is distributed under the MIT license.
