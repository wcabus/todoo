using Todoo.Api.Models;

namespace Todoo.Api.Services;

public interface ITodoService
{
    Task<IReadOnlyCollection<TodoItem>> GetTodosForUserAsync(string userId);
    Task<TodoItem?> GetTodoItemForUserAsync(Guid id, string userId);
    Task AddTodoItemAsync(TodoItem todoItem);
    Task UpdateTodoItemAsync(TodoItem todoItem);
    Task RemoveTodoItemAsync(TodoItem todoItem);
}