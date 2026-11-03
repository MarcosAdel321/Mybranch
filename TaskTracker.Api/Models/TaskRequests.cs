namespace TaskTracker.Api.Models;

public sealed record CreateTaskRequest(string? Title, string? Description);
public sealed record UpdateTaskRequest(string? Title, string? Description, bool IsComplete);
