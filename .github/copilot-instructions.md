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
- `_SqlScripts/` — All database schema and migration scripts live here. SQL changes go in this folder, not inline in application code.

## Database Conventions
- Database schema is managed via scripts in `_SqlScripts/`. When adding or altering tables, add a new script file rather than modifying existing ones.
- SQL Server `.mdf`/`.ldf`/`.ndf` files are gitignored; only scripts are committed.

## Environment
- `.env` files are gitignored. Secrets and connection strings must not be committed.
- VS Code settings (`.vscode/settings.json`, `tasks.json`, `launch.json`, `extensions.json`) are tracked; other `.vscode/` files are ignored.
