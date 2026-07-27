# Personal Portfolio Website

A modern full-stack portfolio website built with **ASP.NET Core** and **React** to showcase my projects, technical skills, and continuous growth as a software developer.

This project serves as both my personal website and a playground where I explore new technologies, improve existing features, and apply software engineering best practices.

> 🚧 This project is actively under development.

---

## Tech Stack

### Backend
- C#
- ASP.NET Core Minimal APIs
- Entity Framework Core
- PostgreSQL
- JWT Authentication
- REST APIs

### Frontend
- React
- TypeScript
- TanStack Query
- React Router
- Vite

### Development
- Git & GitHub
- Visual Studio Code
- Linux (Ubuntu)

---

## Features

### Current
- Responsive portfolio website
- Project showcase
- Authentication & authorization
- Admin area for managing content
- REST API backend
- Modern React frontend
- Clean architecture
- Entity Framework Core data access

### Planned
- Tags & filtering
- Blog section
- Contact form
- Image management
- Project search
- Rich markdown support
- CI/CD pipeline
- Docker deployment

---

## Goals

This project is intended to demonstrate:

- Clean and maintainable code
- Modern .NET backend development
- React frontend development
- API design
- Authentication & authorization
- Database design
- Full-stack application architecture

As I continue learning, this project will evolve alongside my skills.

---

## Running the project
## Configuration

The API uses **.NET User Secrets** for local development.

Initialize User Secrets:

```bash
dotnet user-secrets init --project PersonalSite.Api
```

Set the required secrets:

```bash
dotnet user-secrets set "Jwt:SigningKey" "your-long-random-signing-key" --project PersonalSite.Api

dotnet user-secrets set "Jwt:Issuer" "PersonalSite.Api" --project PersonalSite.Api

dotnet user-secrets set "Jwt:Audience" "PersonalSite.Client" --project PersonalSite.Api

dotnet user-secrets set "DevelopmentAdmin:Password" "choose-a-secure-password" --project PersonalSite.Api
```

Required secrets:

| Key | Description |
|------|-------------|
| `Jwt:SigningKey` | Secret key used to sign JWT tokens. |
| `Jwt:Issuer` | JWT issuer. |
| `Jwt:Audience` | JWT audience. |
| `DevelopmentAdmin:Password` | Password for the seeded development administrator account. |

> These settings are only intended for local development and should never be committed to source control.
### Backend

```bash
cd PersonalSite.Api
dotnet restore
dotnet run
```

### Frontend

```bash
cd PersonalSite.Web
npm install
npm run dev
```

---

## About

I'm Ben Eeckman, a Junior .NET Developer passionate about backend development, software architecture, and building reliable applications.

This repository documents my progress as I continue learning and expanding my knowledge of modern software development.
