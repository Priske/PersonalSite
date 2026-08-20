# Personal Site

A full-stack personal portfolio and content-management application built with ASP.NET Core, React, TypeScript, PostgreSQL, and Azure Blob Storage.

The public site presents my homepage, skills, projects, featured content, CV, and contact information. An authenticated account area provides the management tools behind that content. Administrators manage the official portfolio, while regular users can experiment with isolated demo homepage and project content without changing the public site.

Production is available at [beneeckman.be](https://beneeckman.be) and is deployed to Azure Container Apps through GitHub Actions.

> This is an actively developed personal project used to expand and demonstrate full-stack, testing, cloud, and software-design skills.

## Technology

### Backend

- C# and .NET 8
- ASP.NET Core Minimal APIs
- Entity Framework Core
- PostgreSQL with Npgsql
- JWT authentication and role-based authorization
- Azure Blob Storage using managed identity/default Azure credentials
- Swagger/OpenAPI

### Frontend

- React 19
- TypeScript
- TanStack Query
- React Router
- Vite
- Responsive custom CSS

### Testing and delivery

- xUnit
- ASP.NET Core integration testing
- Testcontainers with PostgreSQL
- Docker multi-stage builds
- GitHub Actions
- Azure Container Registry
- Azure Container Apps
- GitHub OpenID Connect authentication for Azure

## Current features

### Public portfolio

- Editable hero and contact sections
- Grouped and ordered skills
- Projects with tags, repository links, live links, and display ordering
- Featured content with titles, descriptions, tags, and multiple attached files
- Manual featured-file carousel without automatic rotation
- Inline images, videos, and PDFs
- Downloadable CV
- Responsive desktop and mobile layouts

### Featured content and files

Administrators can create and edit featured-content entries, assign existing or newly created tags, and attach multiple files.

Supported featured files:

- MP4 and WebM videos up to 100 MB
- JPG, JPEG, PNG, and WebP images up to 10 MB
- PDF documents up to 10 MB

File metadata and relationships are stored in PostgreSQL. File contents are stored in Azure Blob Storage. Public file responses support range requests for efficient video playback. Removing the last reference to an attached file also removes its blob and database record.

The CV uses the same blob-storage abstraction but has its own upload and download endpoints.

### Content management

The account area includes management pages for:

- Homepage configuration
- Projects and project ordering
- Featured content and attachments
- Skills and skill groups
- Tags
- Users
- Account information
- Analytics

Backend handlers enforce permissions independently of frontend route protection.

### Official and demo content

Official content is managed by administrators and displayed on the public website.

Authenticated regular users receive isolated demo homepage and project content. They can experiment with the management interface without modifying official content or another user's demo content.

### Tags

Tags are reusable across projects and featured content. Tag names are globally unique. Users may reuse existing tags, while ownership and administrator permissions determine who may rename or delete them.

### Analytics

The site records public engagement and server-side account activity, including:

- Page views and referrers
- Contact-link clicks
- Video starts, completed plays, and watched duration
- Login activity
- User creation and deletion activity

Public visitors can submit engagement events. Analytics reports require authentication and administrator authorization.

The analytics dashboard supports searching, date filtering, sorting, and aggregated totals.

## Architecture

The backend is organized by responsibility:

```text
PersonalSite.Api/
├── Application/   Request models and command/query handlers
├── Domain/        Entities, value objects, permissions, and validation
├── Endpoints/     ASP.NET Core Minimal API route definitions
├── Migrations/    Entity Framework Core migrations
├── Storage/       EF repositories, database configuration, and blob storage
├── Analytics/     Activity entities and metadata value types
├── Security/      Authentication-related implementation
├── Seeding/       Initial and development data
└── Wiring/        Dependency injection and application startup
```

The frontend is feature-oriented:

```text
Frontend/src/
├── account/
├── analytics/
├── auth/
├── featured/
├── home/
├── homePageConfig/
├── projects/
├── skills/
├── tags/
└── users/
```

TanStack Query manages server state, mutations, caching, and invalidation. In production, ASP.NET Core serves the compiled React application and API from a single container.

## Local development

### Requirements

- .NET 8 SDK
- Node.js 24 and npm
- Docker
- Azure CLI or another credential supported by `DefaultAzureCredential`
- Access to an Azure Storage account when testing uploads

### 1. Create the local environment file

Create an untracked `.env.local` file in the repository root:

```dotenv
POSTGRES_PASSWORD=choose-a-local-password
AZURE_STORAGE_ACCOUNT_NAME=your-storage-account
AZURE_STORAGE_CONTAINER_NAME=your-container
```

The PostgreSQL password must match the password in the local connection string configured below.

### 2. Configure API user secrets

The API project already has a user-secrets identifier. Configure the local database, JWT signing key, and initial administrator:

```bash
dotnet user-secrets set \
  "ConnectionStrings:PersonalSite" \
  "Host=localhost;Port=5433;Database=personal_site;Username=personal_site;Password=choose-a-local-password" \
  --project PersonalSite.Api

dotnet user-secrets set \
  "Jwt:SigningKey" \
  "replace-this-with-a-long-random-development-key" \
  --project PersonalSite.Api

dotnet user-secrets set \
  "InitialAdmin:Name" \
  "Your Name" \
  --project PersonalSite.Api

dotnet user-secrets set \
  "InitialAdmin:Email" \
  "your@email.example" \
  --project PersonalSite.Api

dotnet user-secrets set \
  "InitialAdmin:Password" \
  "choose-a-development-password" \
  --project PersonalSite.Api
```

The issuer and audience already have development defaults in `appsettings.json`.

### 3. Authenticate to Azure

The blob-storage implementation uses `DefaultAzureCredential`. For local development, the simplest option is:

```bash
az login
```

The signed-in identity needs permission to read and write blobs in the configured container.

### 4. Start the development environment

```bash
./dev.sh
```

The script:

1. Starts PostgreSQL through Docker Compose.
2. Installs locked frontend dependencies when needed.
3. Starts the API with `dotnet watch`.
4. Starts the Vite development server.

Local addresses:

- Frontend: `http://localhost:5173`
- API: `http://localhost:5285`
- Swagger: `http://localhost:5285/swagger`
- PostgreSQL: `localhost:5433`

Press `Ctrl+C` to stop the API and frontend. PostgreSQL remains running for faster subsequent starts. Use `./dev-stop.sh` to stop the remaining development services.

### Manual startup

The services can also be started separately:

```bash
docker compose --env-file .env.local -f compose.local.yaml up -d postgres
dotnet watch --project PersonalSite.Api/PersonalSite.Api.csproj run
npm --prefix Frontend ci
npm --prefix Frontend run dev
```

## Configuration

Production configuration is supplied through environment variables and Azure Container App secrets.

| Configuration | Purpose |
| --- | --- |
| `ConnectionStrings__PersonalSite` | PostgreSQL connection string |
| `Jwt__SigningKey` | JWT signing secret |
| `Jwt__Issuer` | Token issuer |
| `Jwt__Audience` | Token audience |
| `InitialAdmin__Name` | Initial administrator name |
| `InitialAdmin__Email` | Initial administrator email |
| `InitialAdmin__Password` | Initial administrator password |
| `AzureStorage__AccountName` | Azure Storage account containing portfolio files |
| `AzureStorage__ContainerName` | Blob container used for portfolio files |
| `SeedDatabase` | Enables additional development seed data |

Blob authentication does not use a storage connection string. Production uses the Azure identity available to the application.

## Database migrations

The API applies pending Entity Framework Core migrations during startup.

Create a migration after changing the persisted model:

```bash
dotnet ef migrations add MigrationName \
  --project PersonalSite.Api \
  --startup-project PersonalSite.Api
```

Check that the model and latest migration agree:

```bash
dotnet ef migrations has-pending-model-changes \
  --project PersonalSite.Api \
  --startup-project PersonalSite.Api
```

Always inspect a generated migration before committing or deploying it.

## Verification

Docker must be running because the integration tests use Testcontainers.

Run the backend test suite:

```bash
dotnet test PersonalSite.sln
```

Lint and build the frontend:

```bash
npm --prefix Frontend run lint
npm --prefix Frontend run build
```

Build the same container used in production:

```bash
docker build \
  --file PersonalSite.Api/Dockerfile \
  --tag personal-site:local \
  .
```

## Continuous integration and deployment

The general CI workflow runs on pushes and pull requests. It restores the .NET solution, installs locked frontend dependencies under Node.js 24, lints the frontend, and runs the backend test suite.

Pushes to `main` also trigger the production workflow:

1. Restore and build the backend.
2. Run the backend tests in Release mode.
3. Install and build the frontend under Node.js 24.
4. Build the production Docker image.
5. Push commit-specific and `latest` tags to Azure Container Registry.
6. Deploy the commit-specific image to Azure Container Apps.
7. Verify the deployed image and public website.

The production container is built in three stages:

- Node.js 24 builds the React application.
- The .NET 8 SDK publishes the API.
- The .NET 8 ASP.NET runtime serves both the API and compiled frontend.

Azure access from GitHub Actions uses OpenID Connect rather than a long-lived Azure password.

## Project status

The application is actively developed. Current work focuses on expanding portfolio content, analytics, test coverage, and production hardening while keeping official and demo data isolated.
