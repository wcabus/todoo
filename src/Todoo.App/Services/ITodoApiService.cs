using Todoo.App.Models;

namespace Todoo.App.Services;

public interface ITodoApiService
{
    Task<IReadOnlyCollection<TodoItem>> GetTodoItemsAsync();
    Task CreateTodoItemAsync(CreateTodoItem todoItem);
}