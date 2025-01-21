using FluentValidation;

namespace Todoo.Api.Models;

public record CreateTodoItemRequest(string Title, string? Description)
{
    public class Validator : AbstractValidator<CreateTodoItemRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Description).MaximumLength(4000);
        }
    }
}