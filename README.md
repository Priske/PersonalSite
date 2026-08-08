# Personal Portfolio Website

A full-stack personal portfolio and content management application built with **ASP.NET Core**, **React**, **TypeScript**, and **PostgreSQL**.

The project serves as both my public portfolio and a long-term software development project where I can apply and experiment with backend development, frontend development, authentication, authorization, automated testing, CI/CD, containerization, and cloud deployment.

Rather than being a static portfolio, the website contains its own content management system. Administrators manage the official public website, while authenticated users can work with their own demo content without affecting the official portfolio.

The application is deployed to **Azure Container Apps** using an automated **GitHub Actions** deployment pipeline.

> 🚧 This project is actively developed and continues to evolve as I learn and add new functionality.

---

# Tech Stack

## Backend

- C#
- .NET 8
- ASP.NET Core Minimal APIs
- Entity Framework Core
- PostgreSQL
- JWT Authentication
- REST APIs

## Frontend

- React
- TypeScript
- TanStack Query
- React Router
- Vite

## Testing

- xUnit
- ASP.NET Core Integration Testing
- Testcontainers
- PostgreSQL integration database
- Domain tests
- Authorization and authentication tests

## Infrastructure & Deployment

- Docker
- Docker Compose
- Azure Container Apps
- Azure Container Registry
- GitHub Actions
- GitHub OpenID Connect (OIDC)

## Development

- Git
- GitHub
- Visual Studio Code
- Ubuntu Linux

---

# Features

## Public Portfolio

The public website displays the official portfolio content, including:

- Homepage
- Developer introduction
- Skills
- Featured projects
- Project technologies/tags
- Contact information
- Responsive layout

Official content is separated from demo content so user experimentation never modifies the public portfolio.

---

## Authentication

The application contains JWT-based authentication and role-based authorization.

Authenticated requests use the identity and role stored in the JWT to determine which operations the current user may perform.

The application currently supports:

- Administrator users
- Regular users
- Fake users used for demonstration purposes

Backend authorization is enforced independently of frontend route protection.

---

# Content Management

The application includes an account area for managing website content.

## Administrator

Administrators manage the official website and have elevated management permissions.

Administrators can manage:

- Official homepage content
- Official projects
- Project ordering
- Featured projects
- Tags
- Skills
- Users
- Fake users

## Regular User

Regular users can experiment with the application's content management functionality without modifying the real portfolio.

Users can manage:

- Their own account
- Their own demo homepage
- Their own demo projects
- Their own project ordering
- Tags they created
- Fake users available for demonstration purposes

Each user's demo website content is isolated from other users.

---

# Official and Demo Content

Website content supports two different contexts:

### Official

Official content represents the real public portfolio.

It is managed by administrators and displayed on the public website.

### Demo

Demo content allows authenticated users to experiment with the website's content management functionality.

Demo content is associated with the user who created it, allowing multiple users to manage their own content independently without affecting the official website or another user's demo content.

This separation is currently used for:

- Homepage content
- Projects

---

# Project Management

Projects can be managed directly through the website.

Supported functionality includes:

- Create projects
- Edit projects
- Delete projects
- Reorder projects
- Mark projects as featured
- Repository links
- Live project links
- Assign tags
- Official and Demo project separation

Administrators manage the official project portfolio.

Regular users manage their own Demo projects.

The Demo homepage displays the projects belonging to the authenticated user's demo environment.

---

# Tag Management

Tags represent technologies and other project classifications.

Tags are globally available so they can be reused across projects instead of creating duplicate tags for every user or project.

Supported functionality includes:

- Create tags
- Rename tags
- Delete unused tags
- Assign existing tags to projects
- View projects using a tag
- Globally unique tag names

Regular users may use tags created by other users or administrators but can only manage tags they created themselves.

Administrators can manage all tags.

---

# User Management

The application contains user management functionality with different capabilities depending on the authenticated role.

Administrators can manage the complete user environment.

Regular users can manage:

- Their own account
- Fake users used by the demonstration environment

Fake users allow user-management functionality to be demonstrated without giving regular users permission to modify real accounts.

The application can replenish the FakeUser pool when additional demonstration users are required.

---

# Homepage Management

Homepage content can be managed through the account dashboard.

Editable content includes the hero and contact sections of the website.

Administrators edit the official homepage.

Regular users receive their own Demo homepage configuration, which can be modified and previewed without changing the public website.

---

# Skills Management

The website contains a Skills section for displaying technologies and development skills.

Skills are currently managed by administrators and displayed as part of the public portfolio.

The Skills implementation continues to be expanded alongside the other content-management functionality.

---

# Testing

The project contains an extensive automated test suite covering both domain behavior and complete API workflows.

Testing includes:

- Domain validation
- Value object validation
- Authentication
- Authorization
- User permissions
- Homepage management
- Official and Demo content separation
- Cross-user content isolation
- Project CRUD operations
- Project ordering
- Project metadata
- Project ownership
- Tag management
- Tag permissions
- Tag relationships
- User management
- API validation
- Database behavior

Integration tests run against an isolated **PostgreSQL** database using **Testcontainers**.

This allows the integration tests to exercise the real Entity Framework Core/PostgreSQL behavior instead of relying on an in-memory database implementation.

---

# Continuous Integration & Deployment

The application uses **GitHub Actions** for automated validation and deployment.

When changes are pushed to the production branch, the workflow:

1. Checks out the repository
2. Configures .NET
3. Restores dependencies
4. Builds the backend
5. Runs the automated test suite
6. Builds the React frontend
7. Builds the production Docker image
8. Pushes the image to Azure Container Registry
9. Deploys the new image to Azure Container Apps

Azure authentication uses **GitHub OpenID Connect (OIDC)** instead of storing long-lived Azure credentials in GitHub.

Deployment is therefore part of the same automated pipeline used to validate the application.

---

# Architecture

The backend separates responsibilities between the API/application layer, domain model, and persistence infrastructure.

The project makes use of concepts including:

- Domain entities
- Value objects
- Repository abstractions
- Command handlers
- Query handlers
- Domain exceptions
- Centralized exception handling
- Authentication
- Role-based authorization
- Ownership-based authorization
- Content metadata
- Dependency injection
- Entity Framework Core configurations

The frontend is organized around React features and communicates with the backend through the REST API.

**TanStack Query** manages server state, queries, mutations, and cache invalidation.

---

# Running Locally

## Requirements

To run the project locally you will need:

- .NET 8 SDK
- Node.js / npm
- Docker
- Git

---

## Configure User Secrets

Initialize user secrets for the API:

```bash
dotnet user-secrets init --project PersonalSite.Api
```

Configure the local PostgreSQL connection:

```bash
dotnet user-secrets set \
  "ConnectionStrings:PersonalSite" \
  "Host=localhost;Port=5433;Database=personal_site;Username=personal_site;Password=local-development-password" \
  --project PersonalSite.Api
```

Configure JWT authentication:

```bash
dotnet user-secrets set \
  "Jwt:SigningKey" \
  "your-long-development-signing-key" \
  --project PersonalSite.Api

dotnet user-secrets set \
  "Jwt:Issuer" \
  "PersonalSite.Api" \
  --project PersonalSite.Api

dotnet user-secrets set \
  "Jwt:Audience" \
  "PersonalSite.Client" \
  --project PersonalSite.Api
```

Configure the initial administrator:

```bash
dotnet user-secrets set \
  "InitialAdmin:Name" \
  "Your Name" \
  --project PersonalSite.Api

dotnet user-secrets set \
  "InitialAdmin:Email" \
  "your@email.com" \
  --project PersonalSite.Api

dotnet user-secrets set \
  "InitialAdmin:Password" \
  "your-development-password" \
  --project PersonalSite.Api
```

---

## Start PostgreSQL

```bash
docker compose \
  --env-file .env.local \
  -f compose.local.yaml \
  up -d postgres
```

---

## Start the Backend

```bash
dotnet watch --project PersonalSite.Api
```

---

## Start the Frontend

In another terminal:

```bash
cd Frontend

npm install

npm run dev
```

---

# Running the Tests

The backend test suite can be executed with:

```bash
dotnet test
```

Docker must be running because the integration tests use Testcontainers to create an isolated PostgreSQL database.

The frontend production build can be verified with:

```bash
cd Frontend

npm run build
```

---

# Future Roadmap

Possible future additions include:

- Third-party authentication
- Contact form
- Email verification
- Password reset
- Rich content editing
- Image management
- Azure Blob Storage
- Blog functionality
- Project search and filtering
- Improved accessibility
- Performance optimizations
- Expanded demo functionality
- Additional automated testing
- Further content-management functionality

The roadmap is intentionally flexible because the project also serves as an environment for learning and experimenting with new technologies and architectural approaches.

---

# Goals

This project gives me a practical environment for working with:

- C# and .NET
- ASP.NET Core
- REST API design
- Domain modelling
- Authentication and authorization
- Entity Framework Core
- PostgreSQL
- React
- TypeScript
- Server-state management
- Automated testing
- Docker
- CI/CD
- Azure
- GitHub Actions
- Cloud deployment
- Secure configuration management

Rather than treating these technologies as isolated exercises, the goal is to use them together in a continuously evolving production application.

---

# About

Hi, I'm **Ben Eeckman**.

I'm a Junior .NET Developer with a particular interest in backend development, software architecture, optimization, security, and building reliable software.

This repository documents my progress as I continue learning and applying modern software engineering practices through a real application.

## Live Website

**https://beneeckman.be**