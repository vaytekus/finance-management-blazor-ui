# finance-management-blazor-ui

Blazor WebAssembly Standalone UI for the [finance-management-api-cqrs](https://github.com/vaytekus/finance-management-api-cqrs) backend. Built on .NET 10 with MudBlazor.

## Features

- **Blazor WebAssembly Standalone** — pure client-side WASM, no Blazor Server hosting
- **JWT authentication (hybrid)** — access token in memory, refresh token in HttpOnly Secure cookie
- **Silent refresh on startup** — restores session before first render via `TryRestoreSessionAsync`
- **Silent refresh on 401** — `AuthDelegatingHandler` retries the request after refreshing the access token
- **Auth pipeline** — custom `AuthenticationStateProvider` bridging in-memory token state to Blazor's `<CascadingAuthenticationState>` + `<AuthorizeRouteView>`
- **Secure by default** — global `[Authorize]` via `_Imports.razor`, public pages opt in with `[AllowAnonymous]`
- **Named `HttpClient`s** — `"Api"` (auth-aware, points at backend) and `"Local"` (no auth, for WASM static assets)
- **Login page** — `EditForm` + `DataAnnotationsValidator`, snackbar feedback, open-redirect-safe `returnUrl` handling, show/hide password toggle
- **Register page** — `EditForm` with client-side password confirmation validation, show/hide toggles for both password fields
- **Empty layout for auth pages** — `EmptyLayout` bypasses `MudDrawer` on login/register screens
- **Branded splash screen** — static HTML/CSS loader shown while WASM boots and silent refresh runs
- **Wallets CRUD** — list (`MudTable`), create/edit form, delete confirmation dialog
- **Operation Types CRUD** — list with color-coded `MudChip` for Income/Expense, create/edit form with enum-bound `MudSelect`
- **Operations CRUD** — list with date/wallet/type filters, create/edit form, soft delete
- **Reports** — daily and period reports with income/expense totals and per-type breakdown
- **Profile page** — update username/email, change password (with current password verification), avatar initials in header update on save
- **Users CRUD (admin)** — paginated user list, create/edit/delete, role management; accessible only to Admin role
- **Reusable edit/create form pattern** — dual `@page` routes (`/edit` + `/edit/{id:guid}`), `OnParametersSetAsync` for load
- **Show/hide password toggle** — on all password fields across Login, Register, Profile, UserCreateDialog, UserEditDialog
- **`HttpResponseExtensions`** — `ReadRequiredAsync<T>` and `ReadOrNullAsync<T>` eliminate repeated `EnsureSuccessStatusCode + ReadFromJsonAsync` boilerplate
- **`JwtClaimsParser`** — static class extracted from `AppAuthenticationStateProvider` (SRP)
- **Enum-safe JSON** — shared `ApiJsonOptions` with `JsonStringEnumConverter`
- **Shared Contracts** — request/response DTOs live in `FinanceManagement.Contracts` referenced by both API and UI

## Stack

- .NET 10 / Blazor WebAssembly Standalone
- **MudBlazor 9** — Material Design components (form, snackbar, layout, progress, table, dialog, menu)
- `Microsoft.AspNetCore.Components.Authorization` — auth state + `AuthorizeRouteView`
- `Microsoft.Extensions.Http` — `IHttpClientFactory` (typed + named clients)
- `System.Net.Http.Json` — JSON helpers over `HttpClient`
- Shared `FinanceManagement.Contracts` project (records + enums, no dependencies)

## Architecture

Standalone WASM app talking to the CQRS API. No hosting server — served as static assets, all logic runs in the browser.

```
src/
├── backend/                              # Reference copy of finance-management-api-cqrs
│   ├── FinanceManagement.Domain/
│   ├── FinanceManagement.Application/
│   ├── FinanceManagement.Infrastructure/
│   ├── FinanceManagement.Api/
│   └── FinanceManagement.Functions/
├── frontend/
│   └── FinanceManagement.Web/            # Blazor WASM app
│       ├── Common/                       # HttpResponseExtensions
│       ├── Layout/                       # MainLayout, EmptyLayout, NavMenu, RedirectToLogin
│       ├── Pages/
│       │   ├── Login.razor
│       │   ├── Register.razor
│       │   ├── Profile.razor
│       │   ├── Wallets/{List,Edit}.razor
│       │   ├── OperationTypes/{List,Edit}.razor
│       │   ├── Operations/{List,Edit}.razor
│       │   └── Reports/Reports.razor
│       ├── Services/
│       │   ├── Auth/                     # AuthStateService, AppAuthenticationStateProvider,
│       │   │                             #   AuthService, JwtClaimsParser,
│       │   │                             #   IProfileService, ProfileService
│       │   ├── Http/                     # AuthDelegatingHandler
│       │   ├── Common/                   # ApiJsonOptions
│       │   ├── Wallets/                  # IWalletService + WalletService
│       │   ├── OperationTypes/           # IOperationTypeService + OperationTypeService
│       │   ├── Operations/               # IOperationService + OperationService + OperationQuery
│       │   ├── Reports/                  # IReportService + ReportService
│       │   └── Users/                    # IUserService + UserService
│       ├── Shared/                       # ConfirmDialog, UserCreateDialog, UserEditDialog
│       ├── wwwroot/                      # index.html (splash), css/, appsettings.json
│       ├── App.razor
│       ├── Program.cs
│       └── _Imports.razor
└── shared/
    └── FinanceManagement.Contracts/      # Auth/, Users/, Wallets/, Operations/, OperationTypes/, Reports/, Enums/, Common/
```

### Auth flow

1. On app boot `Program.cs` runs `AuthService.TryRestoreSessionAsync()` before `host.RunAsync()`.
2. `AuthService` POSTs to `/api/auth/refresh` with `credentials: 'include'`. If the HttpOnly cookie has a valid refresh token, it gets a new access token; otherwise it silently fails.
3. `AuthStateService` (singleton) stores the access token + expiry + user in memory and raises `OnChange`.
4. `AppAuthenticationStateProvider` observes `AuthStateService.OnChange` and calls `NotifyAuthenticationStateChanged` → Blazor re-renders auth-aware components.
5. `JwtClaimsParser` parses the JWT payload (base64 decode) into `ClaimsPrincipal` — extracted as a static class from `AppAuthenticationStateProvider`.
6. Protected pages carry `[Authorize]` (via global `_Imports.razor`); unauthenticated hits render `<RedirectToLogin/>` and navigate to `/login?returnUrl=...`.
7. Every outbound API request goes through `AuthDelegatingHandler`, which attaches `Authorization: Bearer <token>` and sets `BrowserRequestCredentials.Include`. On 401 it triggers a refresh and retries once (skipping `/api/auth/*` to avoid recursion).

### Profile feature

- Route `/profile` renders two `MudPaper` cards side-by-side: **Profile Info** and **Change Password**.
- Profile Info form pre-fills `UserName` and `Email` from `AuthStateService.User` on init; on save calls `ProfileService.UpdateProfileAsync` then `GetMeAsync` and updates `AuthStateService` so the header avatar initials refresh immediately.
- Change Password form requires `CurrentPassword`, `NewPassword`, and `ConfirmPassword`; passwords must match client-side before the request is sent.
- All password fields have show/hide eye icon toggles (`Adornment.End` with `VisibilityOff/Visibility` icons).
- `IProfileService` / `ProfileService` hit `api/users/me` (GET, PUT) and `api/users/me/password` (PUT).

### Users admin feature

- Route `/users` accessible only to Admin role (server-side `[Authorize(Roles = "Admin")]` + client-side route guard).
- `MudTable` with server-side pagination; row actions: Edit (opens `UserEditDialog`), Delete (opens `ConfirmDialog`).
- `UserCreateDialog` — create user with role selection and password field.
- `UserEditDialog` — edit username/email/role; optionally reset password (leave blank to keep current).

### Wallets feature

- Route `/wallets` → `MudTable` bound to `IReadOnlyList<WalletResponse>`.
- Row actions: **Edit** navigates to `/wallets/edit/{id}`, **Delete** opens `ConfirmDialog`.
- `Pages/Wallets/Edit.razor` handles both create and edit via dual `@page` directives.
- `MudSelect` bound to the `Currency` enum; `EditForm` + `DataAnnotationsValidator`.

### Operation Types feature

- Route `/operation-types` → `MudTable` with Name, Kind, Description columns.
- Kind rendered as color-coded `MudChip` (Income → `Color.Success`, Expense → `Color.Error`).
- `MudSelect` bound to `OperationKind`; optional description trimmed to `null` when whitespace-only.

### Operations feature

- Route `/operations` → `MudTable` with date, wallet, type, amount, note columns.
- Filter bar: date range, wallet selector, operation type selector.
- Soft delete — deleted operations are hidden from the list but retained in DB.

### Reports feature

- Route `/reports` → daily and period report modes.
- Displays `TotalIncome`, `TotalExpense`, `TotalBalance`, and a per-type breakdown table.
- Currency selector passed as a query param to the API.

## Run

The UI needs the CQRS backend running. Clone and start [finance-management-api-cqrs](https://github.com/vaytekus/finance-management-api-cqrs) first.

Then in this repo:

```bash
cd src/frontend/FinanceManagement.Web
dotnet run --launch-profile https
```

Open `https://localhost:5101` in a browser.

Default admin credentials (seeded by the backend): `admin` / `admin`.

### API base URL

Configured in `src/frontend/FinanceManagement.Web/wwwroot/appsettings.json`:

```json
{ "ApiBaseUrl": "https://localhost:5001" }
```

### CORS

The backend must allow the WASM origin with credentials. In the API's `Program.cs` the CORS policy allows `https://localhost:5101` and calls `AllowCredentials()` — a wildcard origin will not work because the browser blocks credentialed requests to `*`.

## Tests

```bash
# Unit tests
dotnet test tests/FinanceManagement.Tests

# Integration tests (requires Docker for PostgreSQL via Testcontainers)
dotnet test tests/FinanceManagement.IntegrationTests
```

45 tests total: 7 unit, 38 integration.
