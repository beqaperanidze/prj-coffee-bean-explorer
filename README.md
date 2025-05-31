# Coffee Bean Explorer

Coffee Bean Explorer is a C\# ASP.NET Core Web API for managing and exploring a collection of coffee beans, user
reviews, tags, origins, and user-curated lists. It provides RESTful endpoints for CRUD operations and rich filtering,
making it easy to build coffee-related applications or integrate with other systems.

## Features

- Manage coffee beans, origins, tags, and user profiles
- Add, update, and delete reviews for beans
- Organize beans into user-defined lists
- Retrieve beans, users, reviews, tags, and lists with flexible filtering
- Built with ASP.NET Core, Dapper, and PostgreSQL

## API Endpoints

- `/api/v1/beans` — CRUD for coffee beans
- `/api/v1/origins` — CRUD for bean origins
- `/api/v1/tags` — CRUD for tags and tag-bean associations
- `/api/v1/reviews` — CRUD for reviews, with filtering by bean or user
- `/api/v1/users` — CRUD for user accounts
- `/api/v1/user-lists` — CRUD for user lists and list items
- `/api/v1/webhooks` — Endpoint for receiving external webhook payloads

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/)
- [JetBrains Rider](https://www.jetbrains.com/rider/) or Visual Studio

### Installation

1. **Clone the repository:**
   ```sh
   git clone https://github.com/beqaperanidze/prj-coffee-bean-explorer.git
   cd prj-coffee-bean-explorer
   ```

2. **Open the solution:**
    - Open the solution in JetBrains Rider or your preferred IDE.

3. **Restore dependencies:**
   ```sh
   dotnet restore
   ```

4. **Configure the database:**
    - Update the connection string in `appsettings.json` to match your PostgreSQL setup.
    - Run the SQL script at `CoffeeBeanExplorer.Infrastructure/Migrations/InitialSchema.sql` to set up the database
      schema.

5. **Run the application:**
   ```sh
   dotnet run --project CoffeeBeanExplorer
   ```