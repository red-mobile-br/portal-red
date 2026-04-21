# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Portal RedMobile is a sales portal for commercial representatives. It consists of a Vue 3 SPA frontend and an ASP.NET Core 10 backend that acts as a **proxy/gateway** to a Protheus ERP REST API. The backend does not have its own database — all business data comes from Protheus.

## Commands

### Frontend (`frontend/`)
```bash
npm run dev       # Dev server with hot reload (--host)
npm run build     # Production build (vue-tsc + vite)
npm run hml       # Staging build (--mode hml)
npm run preview   # Preview production build locally
```

### Backend (`backend/`)
```bash
dotnet build                        # Compile solution
dotnet run --project API/           # Run locally (port 5010)
dotnet test Tests/                  # Run all tests
dotnet test Tests/ --filter "Name~ClassName"  # Run specific test class
```

### Docker (full stack)
```bash
docker compose up --build           # Build and start all services
docker compose up -d                # Start detached
```

## Architecture

### Data Flow
```
Frontend (Vue 3 / Axios)
  → JWT Bearer token
Backend (ASP.NET 10 / Controllers)
  → Basic Auth (red-api credentials)
Protheus ERP REST API (http://187.45.183.238:8090/rest)
```

**Important**: Protheus returns HTTP 200 even for business errors. The backend normalizes these into proper HTTP error responses. The frontend has a `normalizarErroApi()` function for client-side error handling.

### Backend Structure (`backend/API/`)
- `Controllers/` — 14 controllers; all inherit `BaseApiController` which handles JWT claim extraction and Protheus Basic Auth
- `Services/` — business logic: `EmailService`, `PdfService` (PuppeteerSharp/Chrome), reporting services
- `Models/` — DTOs organized as Requests/Responses
- `Exceptions/` — domain-specific exceptions (e.g., `UsuarioNaoEncontradoException`)
- `Templates/` — Handlebars templates for emails and PDFs
- Configuration is YAML-based (`appsettings.yaml`), not the default JSON

### Frontend Structure (`frontend/src/`)
- `features/` — 11 feature modules, each self-contained with `pages/`, `components/`, `router/`, and `services/`
- `services/` — shared API service layer; returns `ObjetoServico<R>` = `[Promise<R>, Canceler | null]` (supports request cancellation)
- `store/app-store.ts` — single Pinia store for user session
- `router/` — Vue Router with lazy-loaded features; `isPublic: true` meta for unprotected routes
- `core/` — shared interfaces, enums, DTOs

### Authentication
1. POST `/api/login` → forwards credentials to Protheus → returns JWT (24h expiry)
2. JWT stored in localStorage; Axios interceptor injects `Authorization: Bearer` header
3. `BaseApiController` extracts user ID from `ClaimTypes.NameIdentifier` claim

### PDF Generation
Backend uses PuppeteerSharp with headless Chrome. In Docker, Chrome is installed at `/usr/bin/google-chrome-stable` and set via `PUPPETEER_EXECUTABLE_PATH`.

## Environment Variables

| Variable | Used In | Description |
|---|---|---|
| `VITE_API_URL` | Frontend build | Backend API base URL |
| `VITE_MAPBOX` | Frontend build | Mapbox GL access token |
| `ASPNETCORE_ENVIRONMENT` | Backend | `Development` or `Production` |

## Naming Conventions

**All code must be written in PT-BR**: routes, variables, properties, method names, filenames, class names, etc.

The only English terms allowed are those with strict technical purpose as suffixes/prefixes:

| Allowed English term | Example |
|---|---|
| `Service` | `PedidoService` |
| `Repository` | `ClienteRepository` |
| `Controller` | `OrcamentoController` |
| `Exception` | `UsuarioNaoEncontradoException` |
| `Dto` | `PedidoDto` |
| `Model` | `ClienteModel` |
| `Middleware` | `ExcecaoMiddleware` |

Everything else — domain concepts, method names, variable names, route segments, file names — must be in Portuguese. For example: `buscarPedidos()`, `/api/orcamentos`, `valorTotal`, `PedidoNaoEncontradoException`.

## Frontend Conventions

- **Styling**: Tailwind CSS with `class` dark mode strategy; custom color tokens (primary, accent, neutral) in `tailwind.config.js`
- **Global styles**: `src/assets/styles/`
- **No CSS Modules** — this project uses Tailwind utility classes directly in Vue SFC `<template>`
- **Locale**: Portuguese (pt-BR); all user-facing strings are in Brazilian Portuguese
- **Print routes**: `/impressao/*` routes render report pages for printing/PDF export

## Testing

Backend uses xUnit + Moq + FluentAssertions + Bogus (faker). Tests live in `backend/Tests/`.

No frontend test suite exists currently.

## Arquivos Protheus (`.prw`)

Os arquivos em `protheus/` usam encoding **Windows-1252** (ISO-8859). **Nunca use o Edit tool nem o Write tool** nesses arquivos — ambos reescrevem em UTF-8, corrompendo os caracteres especiais e podendo zerar o arquivo.

Para editar arquivos `.prw`, use Python em modo binário:

```python
with open('protheus/WSXxx.prw', 'rb') as f:
    data = f.read()
data = data.replace(b'nomeAntigo', b'nomeNovo')
with open('protheus/WSXxx.prw', 'wb') as f:
    f.write(data)
```

Os originais limpos estão em `/home/hugo/Downloads/RepresentantesNew/` e servem como fonte de restauração caso um arquivo seja corrompido.
