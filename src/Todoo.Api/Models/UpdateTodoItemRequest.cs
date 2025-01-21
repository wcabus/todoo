using FluentValidation;

namespace Todoo.Api.Models;

public record UpdateTodoItemRequest(string Title, string? Description, bool IsCompleted)
{
    public class Validator : AbstractValidator<UpdateTodoItemRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Description).MaximumLength(4000);
        }
    }
}