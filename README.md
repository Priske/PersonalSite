# Personal Site

A full-stack portfolio and content-management application built with ASP.NET Core, React, TypeScript, PostgreSQL, Azure Blob Storage, and OpenAI.

The public site presents my homepage, skills, projects, featured content, CV, and contact information. It also includes a portfolio assistant that answers questions about me and my work using administrator-managed Markdown knowledge files.

An authenticated account area provides the management tools behind the site. Administrators manage the official portfolio, assistant knowledge, and analytics. Regular users can experiment with isolated demo homepage and project content without changing the public site.

Production is available at [beneeckman.be](https://beneeckman.be) and is deployed to Azure Container Apps through GitHub Actions.

> This is an actively developed personal project used to expand and demonstrate full-stack development, testing, cloud deployment, AI integration, and software-design skills.

## Technology

### Backend

- C# and .NET 8
- ASP.NET Core Minimal APIs
- Entity Framework Core
- PostgreSQL with Npgsql
- JWT bearer authentication and role-based authorization
- Azure Blob Storage using `DefaultAzureCredential`
- Official OpenAI .NET SDK and Responses API
- ASP.NET Core rate limiting
- Swagger/OpenAPI

### Frontend

- React 19
- TypeScript
- TanStack Query
- React Router
- Vite
- Responsive custom CSS
- Per-tab assistant history using `sessionStorage`

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
- Public portfolio assistant

### Portfolio assistant

Visitors can open the assistant from any page and ask questions about me, my skills, my projects, or how the portfolio was built.

The assistant:

- Uses the OpenAI Responses API through the official .NET SDK
- Answers from administrator-uploaded portfolio knowledge rather than querying unrelated application tables
- Sends the current question and assembled knowledge to OpenAI for each request
- Treats uploaded knowledge as reference material, not executable instructions
- Is instructed not to invent undocumented details, expose secrets, or follow prompt-injection attempts
- Explains technical subjects in language suitable for general visitors
- Limits questions to 1,000 characters
- Limits each client address to 10 requests per 10 minutes
- Returns a controlled `503 Service Unavailable` response when OpenAI cannot provide an answer

The frontend keeps displayed messages in `sessionStorage`, so refreshing a tab does not remove the visible conversation. Closing the tab ends that local session. Each question is evaluated independently; earlier displayed messages are not sent back to OpenAI as conversation context.

The browser detects offline mode immediately and cancels assistant requests that exceed 30 seconds. Quota failures, provider errors, invalid responses, network failures, and timeouts are presented to visitors as a simple temporary-unavailability message while the rest of the portfolio remains usable.

### Assistant knowledge

Administrators manage the assistant's knowledge from the account area by uploading Markdown documents such as `about-me.md` and `projects.md`.

Knowledge uploads:

- Require administrator authorization
- Accept `.md` files up to 256 KiB
- Require non-empty, valid UTF-8 content
- Normalize filenames before storage
- Reject duplicate filenames instead of silently overwriting existing knowledge
- Store file metadata and relationships in PostgreSQL
- Store the Markdown contents in Azure Blob Storage
- Remove the uploaded blob when the database save fails

At request time, the application reads every linked knowledge file in filename order, labels the documents, and supplies the combined content to the assistant.

### Assistant chat analytics

Successful assistant exchanges are recorded as analytics activities. Each record contains:

- The visitor's question
- The assistant's answer
- The authenticated user ID when available
- The creation timestamp

Administrators can review chat logs from the Assistant management page. The interface supports question-and-answer search, exact user-ID filtering, date filtering, date or user sorting, ascending or descending order, and pagination. Anonymous and authenticated conversations are summarized separately.

Failed or rejected assistant requests are not stored as successful exchanges.

### Featured content and files

Administrators can create and edit featured-content entries, assign existing or newly created tags, and attach multiple files.

Supported featured files:

- MP4 and WebM videos up to 100 MB
- JPG, JPEG, PNG, and WebP images up to 10 MB
- PDF documents up to 10 MB

File metadata and relationships are stored in PostgreSQL. File contents are stored in Azure Blob Storage. Public file responses support HTTP range requests for efficient video playback. Removing the last reference to an attached file also removes its blob and database record.

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
- Assistant knowledge and chat logs
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
- Successful assistant questions and answers

Public visitors can submit supported engagement events. Analytics reports require authentication and administrator authorization.

The analytics interfaces support searching, date filtering, sorting, pagination, and aggregated totals.

## Architecture

The backend is organized by responsibility:

```text
PersonalSite.Api/
├── Application/   Request models and command/query handlers
├── Domain/        Entities, value objects, permissions, validation, and exceptions
├── Endpoints/     ASP.NET Core Minimal API route definitions
├── Infrastructure/ External integrations such as OpenAI, SMTP, and security services
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
├── assistant/
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
- An OpenAI API key when testing the portfolio assistant

### 1. Create the local environment file

Create an untracked `.env.local` file in the repository root:

```dotenv
POSTGRES_PASSWORD=choose-a-local-password
AZURE_STORAGE_ACCOUNT_NAME=your-storage-account
AZURE_STORAGE_CONTAINER_NAME=your-container
SMTP_PASSWORD=your-development-smtp-password
```

The PostgreSQL password must match the password in the local connection string configured below. The development script reads the Azure Storage and SMTP values from this file.

If the complete API container is started through `compose.local.yaml`, also add:

```dotenv
JWT_SIGNING_KEY=replace-this-with-a-long-random-development-key
INITIAL_ADMIN_NAME=Your Name
INITIAL_ADMIN_EMAIL=your@email.example
INITIAL_ADMIN_PASSWORD=choose-a-development-password
OPENAI_API_KEY=your-development-openai-api-key
```

### 2. Configure API user secrets

The API project already has a user-secrets identifier. Configure the local database, JWT signing key, initial administrator, and OpenAI API key:

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

dotnet user-secrets set \
  "OpenAI:ApiKey" \
  "your-development-openai-api-key" \
  --project PersonalSite.Api
```

The JWT issuer and audience and the OpenAI model have defaults in `appsettings.json`.

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

ASP.NET Core configuration is supplied through `appsettings.json`, user secrets, environment variables, GitHub Actions secrets, and Azure Container App secrets.

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
| `Smtp__Password` | SMTP account password used by the contact form |
| `OpenAI__ApiKey` | OpenAI API key used by the portfolio assistant |
| `OpenAI__Model` | OpenAI model name; defaults to the value in `appsettings.json` |
| `SeedDatabase` | Enables additional development seed data |

Blob authentication does not use a storage connection string. Production uses the Azure identity available to the application.

The production deployment stores the SMTP password and OpenAI API key as GitHub Actions secrets, writes them to Azure Container App secrets, and maps them into the container through secret references. Secret values are not committed to the repository or embedded in the container image.

## Database migrations

The API applies pending Entity Framework Core migrations during startup. The assistant knowledge model uses an additive migration that creates the knowledge and attachment tables. Assistant chat logs reuse the existing analytics activity and metadata tables.

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

Run the backend test suite in the same configuration used by deployment:

```bash
dotnet test PersonalSite.sln --configuration Release
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

1. Restore and build the backend in Release mode.
2. Run the backend tests.
3. Install and build the frontend under Node.js 24.
4. Build the production Docker image.
5. Push commit-specific and `latest` tags to Azure Container Registry.
6. Validate and configure the SMTP and OpenAI secrets in Azure Container Apps.
7. Deploy the commit-specific image as a new Container App revision.
8. Wait for the revision to report healthy and running.
9. Verify that the revision uses the expected commit-specific image.
10. Route production traffic to the healthy revision when multiple-revision mode is enabled.
11. Verify the public website.

The production container is built in three stages:

- Node.js 24 builds the React application.
- The .NET 8 SDK publishes the API.
- The .NET 8 ASP.NET runtime serves both the API and compiled frontend.

Azure access from GitHub Actions uses OpenID Connect rather than a long-lived Azure password.

## Production rollout

After the first deployment containing the assistant knowledge migration:

1. Sign in to the production administrator account.
2. Open the Assistant management page.
3. Upload the production Markdown knowledge files.
4. Ask a grounded question through the public chatbox.
5. Confirm that the successful exchange appears in the administrator chat logs.

Local database rows and local blob relationships are not copied to production automatically.

## Project status

The application is actively developed and deployed. Current work focuses on expanding portfolio content, assistant knowledge, analytics, test coverage, and production hardening while keeping official and demo data isolated.
