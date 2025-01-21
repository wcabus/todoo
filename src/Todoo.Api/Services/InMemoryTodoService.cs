using Todoo.Api.Models;

namespace Todoo.Api.Services;

internal class InMemoryTodoService : ITodoService
{
    private readonly List<TodoItem> _todoItems = [];

    public Task<IReadOnlyCollection<TodoItem>> GetTodosForUserAsync(string userId)
    {
        var results = _todoItems.Where(x => x.UserId == userId).ToArray();
        return Task.FromResult<IReadOnlyCollection<TodoItem>>(results);
    }

    public Task<TodoItem?> GetTodoItemForUserAsync(Guid id, string userId)
    {
        return Task.FromResult(_todoItems.FirstOrDefault(x => x.Id == id && x.UserId == userId));
    }

    public Task AddTodoItemAsync(TodoItem todoItem)
    {
        _todoItems.Add(todoItem);
        return Task.CompletedTask;
    }

    public Task UpdateTodoItemAsync(TodoItem todoItem)
    {
        return Task.CompletedTask;
    }

    public Task RemoveTodoItemAsync(TodoItem todoItem)
    {
        _todoItems.Remove(todoItem);
        return Task.CompletedTask;
    }
}