# Avolutions.Baf.Core

Core library for the Avolutions Business Application Framework (BAF).

## Installation

Install via [NuGet](https://www.nuget.org/packages/Avolutions.Baf.Core):

```bash
dotnet add package Avolutions.Baf.Core
```

## Quick Start

In your Program.cs, add BAF to the service collection and middleware pipeline.

```csharp
using Avolutions.BAF.Core.Modules.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Register EF Core DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register BAF Core with your DbContext
builder.Services.AddBafCore<ApplicationDbContext>();

var app = builder.Build();

// Initialize BAF Core
app.UseBafCore();

app.Run();
```