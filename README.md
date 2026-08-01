# Personal Portfolio Website

A modern full-stack portfolio website built with **ASP.NET Core** and **React**.

This project serves as both my personal website and a long-term learning project where I explore new technologies, improve existing features, and apply modern software engineering practices.

Rather than being a simple portfolio, the goal is to build and maintain a production-style application using the same tools and workflows commonly found in professional software development.

> 🚧 This project is actively under development.

---

# Tech Stack

## Backend

- C#
- ASP.NET Core 8 Minimal APIs
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

## Infrastructure

- Docker
- Docker Compose
- Azure Container Apps
- Azure Container Registry
- GitHub Actions
- GitHub OIDC Authentication

## Development

- Git
- GitHub
- Visual Studio Code
- Ubuntu Linux
- Testcontainers

---

# Features

## Current

- Responsive portfolio website
- Project showcase
- Skills management
- Authentication & authorization
- Role-based access control
- Admin dashboard
- REST API
- React frontend
- Clean architecture
- Entity Framework Core
- PostgreSQL
- Automated database migrations
- Dockerized deployment
- Automated CI/CD with GitHub Actions
- Azure deployment

## Planned

## Planned

- Third-party authentication through external providers
- Demo dashboard for visitors
- Demo user role with restricted permissions
- Fake user management for demonstration purposes
- Tags and filtering
- Blog section
- Image management
- Project search
- Rich Markdown support

---

# Testing

The project contains a comprehensive automated test suite including:

- Unit tests
- Integration tests
- Domain validation tests
- Authentication tests
- Authorization tests
- Endpoint tests

Integration tests run against an isolated PostgreSQL database using Testcontainers.

---

# Continuous Integration & Deployment

Every push runs automated validation using GitHub Actions.

The pipeline:

- Restores dependencies
- Builds the solution
- Executes the automated test suite
- Builds the frontend
- Builds the Docker image
- Pushes the image to Azure Container Registry
- Deploys the application to Azure Container Apps

Azure authentication is performed using GitHub OpenID Connect (OIDC), meaning no Azure credentials are stored in the repository.

---

# Goals

This project demonstrates:

- Clean Architecture
- REST API design
- Authentication & Authorization
- Entity Framework Core
- PostgreSQL
- React
- TypeScript
- Docker
- Azure
- GitHub Actions
- CI/CD
- Automated Testing
- Secure configuration management

As I continue learning, the project evolves alongside my skills.

---

# Running Locally

## Configure User Secrets

```bash
dotnet user-secrets init --project PersonalSite.Api
```

```bash
dotnet user-secrets set "ConnectionStrings:PersonalSite" "Host=localhost;Port=5433;Database=personal_site;Username=personal_site;Password=local-development-password" --project PersonalSite.Api

dotnet user-secrets set "Jwt:SigningKey" "your-signing-key" --project PersonalSite.Api

dotnet user-secrets set "Jwt:Issuer" "PersonalSite.Api" --project PersonalSite.Api

dotnet user-secrets set "Jwt:Audience" "PersonalSite.Client" --project PersonalSite.Api

dotnet user-secrets set "InitialAdmin:Name" "Your Name" --project PersonalSite.Api

dotnet user-secrets set "InitialAdmin:Email" "your@email.com" --project PersonalSite.Api

dotnet user-secrets set "InitialAdmin:Password" "your-password" --project PersonalSite.Api
```

## Start PostgreSQL

```bash
docker compose \
    --env-file .env.local \
    -f compose.local.yaml \
    up -d postgres
```

## Start the backend

```bash
dotnet watch --project PersonalSite.Api
```

## Start the frontend

```bash
cd Frontend

npm install

npm run dev
```

---

# Future Roadmap

- Contact form
- Email verification
- Password reset
- Rich content editing
- Azure Blob Storage
- Improved accessibility
- Performance optimizations
- Additional admin features

---

# About

Hi, I'm **Ben Eeckman**.

I'm a Junior .NET Developer passionate about backend development, software architecture, automation and building reliable software.

This repository documents my progress as I continue learning and applying modern software engineering practices. 