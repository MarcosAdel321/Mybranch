# TaskTracker API portfolio demo

A small C# REST API that demonstrates ASP.NET Core, Entity Framework Core, SQLite/SQL, request validation, async CRUD operations, and a simple domain model.

## Run it

Install the .NET 9 SDK, then from `TaskTracker.Api` run:

```powershell
dotnet restore
dotnet run
```

The app creates `tasktracker.db` in its working directory. Open the URL printed by `dotnet run` and use `/api/tasks`.

## Endpoints

| Method | Route | Purpose |
|---|---|---|
| GET | `/api/tasks` | List tasks |
| GET | `/api/tasks/{id}` | Get one task |
| POST | `/api/tasks` | Create a task |
| PUT | `/api/tasks/{id}` | Update a task |
| DELETE | `/api/tasks/{id}` | Delete a task |

Example create request:

```json
{
  "title": "Review API proposal",
  "description": "Check requirements and estimate the work"
}
```

## Design notes

- `WorkTask` is the domain model and `TaskDbContext` maps it to a relational database.
- Request records keep incoming data separate from the persisted entity.
- Async EF Core calls avoid blocking request threads during database access.
- Input validation returns a structured 400 response for invalid titles.
- `AsNoTracking` is used for read-only queries.

This is a learning/portfolio sample, not a production template. A production API may need authentication, authorization, migrations, structured logging, pagination, and automated tests depending on its requirements.
