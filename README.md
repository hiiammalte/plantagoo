# Plantagoo - .NET Core restful API skeleton

This is a simple, pre-configured ASP.NET Core project, meant to be adopted and aiming to give you a head start in building restful APIs, based on the three-tier architecture.

To showcase some of its functionality, this project comes with a minimalistic and fictional implementation of a project management application called Plantagoo.

## Motivation

Personally I’ve been repeatedly creating ASP.NET Core API projects from scratch over the last couple of months. Although tech is known to be changing and improving rapidly, I felt the need to create and publish this project to be able to share, reference and build upon it in the future.

## Features

- Three-Tier Architecture, using Class Libraries per tier
- DI-based, asynchronous services, returning generic response object
- JWT-based authentication and authorization
- PBKDF2-based hashing
- Middleware-based global error handling
- Restful API endpoints, returning DTOs and HTTP status codes
- Implementation of filtering, paging & sorting for API endpoint, which returns lists of objects
- Implementation of API endpoint versioning
- Entity Framework Core-based data access via POCO entities
- AutoMapper-based object-to-object mapping and database query optimization
- Security-focused configurability of database connection string and JWT-parameters via environment-based „appsettings.json“-files, secrets and/or environment variables
- xUnit-based integration testing, including setup, seeding and teardown of dedicated database

## Getting started

To get this project up and running as is, feel free to follow these steps:

### Prerequisites

- .NET Core 3.1+ SDK
- IDE (preferably Visual Studio or Visual Studio Code)
- MySQL Server 8.0.20+

### Setup

1. Clone this repository
2. At the root directory, restore required packages by running:

```
dotnet restore
```

3. Open `appsettings.{environment}.json` files within Plantagoo.API project to customize the following connection string to your needs (see step 7 for credentials):

```
server=localhost;Port=3306;database=apiSkeletonDb
```

4. For development purposes, enable secret storage by running (If you're used to working with environment variables, fast forward to step 7):

```
dotnet user-secrets init
```

5. If you’re on Windows, goto and open `%APPDATA%\Microsoft\UserSecrets\<user_secrets_id>\secrets.json` file.
6. If you're on Linux/macOS, goto and open `~/.microsoft/usersecrets/<user_secrets_id>/secrets.json` file.
7. Within this secrets.json file, add the following lines to set your JWT-parameters and to extend your database connection string by adding username and password:

```
"TokenSettings:Secret": "…",
"TokenSettings:AccessExpirationInMinutes": "…",
"DB:Username": "…",
"DB:Password": "…"
```

8. For an Code-First Approach to creating the database, run the following command (make sure your current directory is `Plantagoo.Data`):

```
dotnet ef migrations add InitialCreate
```

9. Followed by:

```
dotnet ef database update
```

10. Next, build the solution by running:

```
dotnet build
```

11. Once done, launch the application by running:

```
dotnet run
```

12. Launch https://localhost:5001/swagger/index.html in your browser to view the Swagger documentation of your API

## Technologies

- .NET Core 3.1
- ASP.NET Core 3.1
- Entity Framework Core 3.1
- AutoMapper
- Swashbuckle
- xUnit
