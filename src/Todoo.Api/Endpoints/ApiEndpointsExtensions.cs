using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Todoo.Api.Authorization;
using Todoo.Api.Models;
using Todoo.Api.Services;
using Todoo.Api.Validation;

namespace Todoo.Api.Endpoints;

internal static class ApiEndpointsExtensions
{
    public static void MapApiEndpoints(this WebApplication app)
    {
        var todosGroup = app
            .MapGroup("/todos")
            .RequireAuthorization()
            .AddEndpointFilterFactory(ValidationFilter.ValidationFilterFactory);

        var todosByIdGroup = todosGroup
            .MapGroup("{id:guid}");

        todosGroup.MapGet("", GetTodosForCurrentUserAsync);
        todosGroup.MapPost("", AddTodoItemAsync); 
        
        todosByIdGroup.MapGet("", GetTodoItemAsync).WithName(Routes.GetTodoById);
        todosByIdGroup.MapPut("", UpdateTodoItemAsync);
        todosByIdGroup.MapDelete("", RemoveTodoItemAsync);
    }

    internal static async Task<Ok<IReadOnlyCollection<TodoItem>>> GetTodosForCurrentUserAsync(ITodoService service, ClaimsPrincipal user)
    {
        var todos = await service.GetTodosForUserAsync(user.GetUserId());
        return TypedResults.Ok(todos);
    }

    internal static async Task<Results<Ok<TodoItem>, NotFound>> GetTodoItemAsync(Guid id, ITodoService service, ClaimsPrincipal user)
    {
        var todoItem = await service.GetTodoItemForUserAsync(id, user.GetUserId());
        if (todoItem is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(todoItem);
    }

    internal static async Task<Results<CreatedAtRoute<TodoItem>, ValidationProblem>> AddTodoItemAsync([Validate] CreateTodoItemRequest request, ITodoService service, ClaimsPrincipal user)
    {
        var newTodoItem = new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            CreatedAt = DateTimeOffset.UtcNow,
            UserId = user.GetUserId()
        };

        await service.AddTodoItemAsync(newTodoItem);
        return TypedResults.CreatedAtRoute(newTodoItem, Routes.GetTodoById, new { newTodoItem.Id });
    }

    internal static async Task<Results<NoContent, NotFound, BadRequest<string>, ValidationProblem>> UpdateTodoItemAsync(Guid id, [Validate] UpdateTodoItemRequest request, ITodoService service, ClaimsPrincipal user)
    {
        var todoItem = await service.GetTodoItemForUserAsync(id, user.GetUserId());
        if (todoItem is null)
        {
            return TypedResults.NotFound();
        }

        if (todoItem.IsCompleted)
        {
            return TypedResults.BadRequest("Cannot update a completed todo item.");
        }

        todoItem.Title = request.Title;
        todoItem.Description = request.Description;
        todoItem.IsCompleted = request.IsCompleted;
        todoItem.CompletedAt = request.IsCompleted ? DateTimeOffset.UtcNow : null;

        await service.UpdateTodoItemAsync(todoItem);
        return TypedResults.NoContent();
    }

    internal static async Task<Results<NoContent, NotFound>> RemoveTodoItemAsync(Guid id, ITodoService service, ClaimsPrincipal user)
    {
        var todoItem = await service.GetTodoItemForUserAsync(id, user.GetUserId());
        if (todoItem is null)
        {
            return TypedResults.NotFound();
        }
        
        await service.RemoveTodoItemAsync(todoItem);
        return TypedResults.NoContent();
    }

    internal class Routes
    {
        public const string GetTodoById = nameof(GetTodoById);
    }
}