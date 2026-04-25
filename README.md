# DnD Platform

A web application for creating and managing D&D character sheets, with versioning, templates, and D&D 5e data integration.

## Stack

- **Backend:** ASP.NET Core 9.0 (REST API)
- **Frontend:** Blazor WebAssembly 9.0
- **Database:** PostgreSQL 17 (via Entity Framework Core)
- **Auth:** JWT Bearer tokens

## Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

## Setup

**1. Start the database**

```bash
docker-compose up -d
```

This starts a PostgreSQL instance on port `5432` with:
- Database: `dndplatform`
- Username: `dnd`
- Password: `dnd_password`

**2. Start the backend**

```bash
cd src/DnDPlatform.Server
dotnet run
```

API runs on `http://localhost:5024`. Migrations are applied automatically on startup.

**3. Start the frontend** (optional)

```bash
cd src/DnDPlatform.Frontend
dotnet run
```

Frontend runs on `http://localhost:5252`.

## Project Structure

```
src/
├── DnDPlatform.Server        # ASP.NET Core API — controllers, auth, middleware
├── DnDPlatform.Frontend      # Blazor WASM frontend
├── DnDPlatform.Services      # Business logic
├── DnDPlatform.Repositories  # EF Core data access
└── DnDPlatform.Models        # Shared DTOs and domain models
```

## API Overview

All endpoints except `/api/auth/*` require a `Authorization: Bearer <token>` header.

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/register` | Register a new account |
| POST | `/api/auth/login` | Login and receive a JWT |
| GET | `/api/me` | Get current user info |
| GET | `/api/characters` | List your characters |
| POST | `/api/characters` | Create a character |
| GET | `/api/characters/{id}` | Get a character |
| PUT | `/api/characters/{id}/sheet` | Save sheet data |
| POST | `/api/characters/{id}/snapshot` | Create a named snapshot |
| GET | `/api/characters/{id}/versions` | List version history |
| POST | `/api/characters/{id}/revert/{version}` | Revert to a version |
| GET | `/api/characters/{id}/export` | Export as text file |
| DELETE | `/api/characters/{id}` | Delete a character |
| GET | `/api/templates` | List templates |
| GET | `/api/templates/{id}` | Get a template |
| POST | `/api/templates` | Create a template |
| GET | `/api/dnd5e/classes` | D&D 5e classes |
| GET | `/api/dnd5e/spells` | D&D 5e spells (supports `?filter=`) |
| GET | `/api/dnd5e/equipment` | D&D 5e equipment |
| GET | `/api/dnd5e/ability-scores` | D&D 5e ability scores |

### Sheet data format

The `sheetData` field in `PUT /api/characters/{id}/sheet` expects a **JSON string**, not a JSON object:

```json
{ "sheetData": "{\"strength\": 18, \"dexterity\": 14}" }
```

## Configuration

`src/DnDPlatform.Server/appsettings.json` controls the database connection and JWT settings. The defaults match the Docker Compose setup and are fine for local development.

For production, override `Jwt:Key` with a secure secret (min 32 chars) and update the connection string.
