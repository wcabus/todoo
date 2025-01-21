using Todoo.App.Models;

namespace Todoo.App.Services;

internal class TodoApiService(IHttpClientFactory httpClientFactory) : ITodoApiService
{
    private readonly HttpClient _client = httpClientFactory.CreateClient("client");

    public async Task<IReadOnlyCollection<TodoItem>> GetTodoItemsAsync()
    {
        return await _client.GetFromJsonAsync<IReadOnlyCollection<TodoItem>>("todos") ?? [];
    }

    public async Task CreateTodoItemAsync(CreateTodoItem todoItem)
    {
        await _client.PostAsJsonAsync("todos", todoItem);
    }
}