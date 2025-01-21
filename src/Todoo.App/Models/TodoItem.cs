namespace Todoo.App.Models;

public record TodoItem(
    Guid Id,
    string Title,
    string? Description,
    bool IsCompleted,
    DateTimeOffset CreatedAt,
    DateTimeOffset? CompletedAt);
