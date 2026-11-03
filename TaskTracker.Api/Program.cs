using Microsoft.EntityFrameworkCore;
using TaskTracker.Api.Data;
using TaskTracker.Api.Models;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("TaskTracker")
    ?? "Data Source=tasktracker.db";

builder.Services.AddDbContext<TaskDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddProblemDetails();

var app = builder.Build();
app.UseExceptionHandler();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TaskDbContext>();
    db.Database.EnsureCreated();
}

app.MapGet("/", () => Results.Ok(new { name = "TaskTracker API", version = "1.0", docs = "/api/tasks" }));

var tasks = app.MapGroup("/api/tasks").WithTags("Tasks");

tasks.MapGet("/", async (TaskDbContext db, CancellationToken ct) =>
    Results.Ok(await db.Tasks.AsNoTracking().OrderByDescending(x => x.CreatedAtUtc).ToListAsync(ct)));

tasks.MapGet("/{id:int}", async (int id, TaskDbContext db, CancellationToken ct) =>
{
    var task = await db.Tasks.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
    return task is null ? Results.NotFound() : Results.Ok(task);
});

tasks.MapPost("/", async (CreateTaskRequest request, TaskDbContext db, CancellationToken ct) =>
{
    var title = request.Title?.Trim();
    if (string.IsNullOrWhiteSpace(title) || title.Length > 120)
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            [nameof(request.Title)] = ["Title is required and must be 120 characters or fewer."]
        });

    var task = new WorkTask { Title = title, Description = request.Description?.Trim() };
    db.Tasks.Add(task);
    await db.SaveChangesAsync(ct);
    return Results.Created($"/api/tasks/{task.Id}", task);
});

tasks.MapPut("/{id:int}", async (int id, UpdateTaskRequest request, TaskDbContext db, CancellationToken ct) =>
{
    var task = await db.Tasks.FirstOrDefaultAsync(x => x.Id == id, ct);
    if (task is null) return Results.NotFound();

    var title = request.Title?.Trim();
    if (string.IsNullOrWhiteSpace(title) || title.Length > 120)
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            [nameof(request.Title)] = ["Title is required and must be 120 characters or fewer."]
        });

    task.Title = title;
    task.Description = request.Description?.Trim();
    task.IsComplete = request.IsComplete;
    task.UpdatedAtUtc = DateTime.UtcNow;
    await db.SaveChangesAsync(ct);
    return Results.Ok(task);
});

tasks.MapDelete("/{id:int}", async (int id, TaskDbContext db, CancellationToken ct) =>
{
    var task = await db.Tasks.FirstOrDefaultAsync(x => x.Id == id, ct);
    if (task is null) return Results.NotFound();
    db.Tasks.Remove(task);
    await db.SaveChangesAsync(ct);
    return Results.NoContent();
});

app.Run();
