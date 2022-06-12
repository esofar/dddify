using Dddify.Localization;
using Microsoft.EntityFrameworkCore;
using MyCompany.MyProject.Domain.ValueObjects;
using MyCompany.MyProject.Infrastructure;

namespace MyCompany.MyProject.Application.Todos.Commands;

public class UpdateTodoCommandValidator : AbstractValidator<UpdateTodoCommand>
{
    private readonly ApplicationDbContext _context;
    private readonly ISharedStringLocalizer _localizer;

    public UpdateTodoCommandValidator(ApplicationDbContext context, ISharedStringLocalizer localizer)
    {
        _context = context;
        _localizer = localizer;

        RuleFor(v => v.Title)
           .NotEmpty().WithMessage(_localizer["The title is required."])
           .MaximumLength(200).WithMessage(_localizer["The title must not exceed 200 characters."])
           .MustAsync(BeUniqueTitle).WithMessage(_localizer["The specified title already exists."]);

        RuleFor(v => v.Colour)
           .NotEmpty().WithMessage(_localizer["The colour is required."])
           .MustAsync(BeSupportedColour).WithMessage(_localizer["The specified colour code is unsupported."]);
    }

    public async Task<bool> BeUniqueTitle(UpdateTodoCommand command, string title, CancellationToken cancellationToken)
    {
        return await _context.Todos
            .Where(c => c.Id != command.Id)
            .AllAsync(c => c.Title != title, cancellationToken);
    }

    public async Task<bool> BeSupportedColour(Colour colour, CancellationToken cancellationToken)
    {
        return await Task.FromResult(Colour.SupportedColours.Any(c => c.Code == colour.Code));
    }
}
