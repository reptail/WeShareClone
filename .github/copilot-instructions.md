# WeShareClone – Copilot Instructions

## Project Overview
WeShareClone is a .NET/Visual Studio application that replicates the deprecated WeShare app — a tool for splitting bills and travel costs among groups.

## Tech Stack
- **Language:** C#
- **Backend:** ASP.NET Core REST API with Dapper for database access
- **Frontend:** Blazor PWA using MudBlazor component library; consumes the backend REST API for all data access
- **Database:** Microsoft SQL Server (SQL scripts stored in `_SqlScripts/`)
- **Branching:** `master` is the stable branch; active development happens on `dev`

## Repository Structure
- `_SqlScripts/` — All database schema and migration scripts, named `NNN_Description.sql`. Always add new numbered scripts; never modify existing ones.
- `WeShareClone.Api/` — ASP.NET Core REST API (controllers-based, Swashbuckle/Swagger)

## Build
```
dotnet build
```

## Database Conventions
- Schema is managed via sequential scripts in `_SqlScripts/`, named `NNN_Description.sql` (e.g. `001_InitialSchema.sql`). Always add a new numbered script — never modify an existing one.
- `DATETIME2(3)` is the standard for all timestamp fields (millisecond precision).
- Currency fields use `NCHAR(3)` (ISO 4217 codes: DKK, EUR, USD, etc.).
- Monetary value fields use `DECIMAL(18,2)`.
- All constraints (defaults, foreign keys) are explicitly named following the pattern `DF_Table_Column` / `FK_Table_Column`.
- SQL Server `.mdf`/`.ldf`/`.ndf` files are gitignored; only scripts are committed.

## Environment
- `.env` files are gitignored. Secrets and connection strings must not be committed.
- VS Code settings (`.vscode/settings.json`, `tasks.json`, `launch.json`, `extensions.json`) are tracked; other `.vscode/` files are ignored.

## File Conventions
- All files use **CRLF** line endings.
- All files are saved as **UTF-8 without BOM**.
- `.gitkeep` files are used to track empty folders. Remove `.gitkeep` as soon as any other file is added to the same folder.

## C# Formatting
- Use C# 14 `extension` block syntax for extension members (not traditional `this` parameter methods).
- Never use `var` — always declare explicit types.
- For expression-bodied members, place the `=>` arrow on a new line when the expression spans multiple lines.
- Always use named arguments when calling constructors with multiple parameters.
- Place the closing `);` on a new line at the same indentation level as the `=>` arrow:
  ```csharp
  // single-line: arrow inline
  public string Name() => "value";

  // multi-line: arrow on new line, named args, closing ); aligned with =>
  public User ToDomain()
      => new(
          Id: db.Id,
          Name: db.Name
      );
  ```
