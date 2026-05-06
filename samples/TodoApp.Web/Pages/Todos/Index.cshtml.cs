using TodoApp.Application.Commands.Todos;
using TodoApp.Application.Dtos.Todos;
using TodoApp.Application.Queries.Todos;
using TodoApp.Domain.Aggregates.Todos;

namespace TodoApp.Web.Pages.Todos;

public class IndexModel(ISender sender) : BasePageModel(sender)
{
    [BindProperty]
    public CreateTodoInput Input { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public TodoListFilter Filter { get; set; } = TodoListFilter.All;

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    public TodoListDto TodoList { get; private set; } = new([], new(0, 0, 0, 0), TodoListFilter.All, null);

    public IReadOnlyList<FilterItem> Filters { get; } =
    [
        new(TodoListFilter.All, "All", "Review every todo in one place."),
        new(TodoListFilter.Active, "Active", "Focus on work that still needs attention."),
        new(TodoListFilter.Completed, "Completed", "Look back at what has already been shipped."),
        new(TodoListFilter.DueToday, "Due today", "Handle items that need to close today.")
    ];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        await LoadAsync(cancellationToken);
    }

    public async Task<IActionResult> OnPostCreateAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await LoadAsync(cancellationToken);
            return Page();
        }

        return await ExecuteAndRedirectAsync(
            new CreateTodoCommand(Input.Title, Input.Description, Input.Priority, Input.DueDate),
            "Todo created.",
            cancellationToken);
    }

    public async Task<IActionResult> OnPostCompleteAsync(Guid id, CancellationToken cancellationToken)
        => await ExecuteAndRedirectAsync(new CompleteTodoCommand(id), "Todo completed.", cancellationToken);

    public async Task<IActionResult> OnPostReopenAsync(Guid id, CancellationToken cancellationToken)
        => await ExecuteAndRedirectAsync(new ReopenTodoCommand(id), "Todo reopened.", cancellationToken);

    public async Task<IActionResult> OnPostTogglePinAsync(Guid id, CancellationToken cancellationToken)
        => await ExecuteAndRedirectAsync(new TogglePinTodoCommand(id), "Pin status updated.", cancellationToken);

    public async Task<IActionResult> OnPostDeleteAsync(Guid id, CancellationToken cancellationToken)
        => await ExecuteAndRedirectAsync(new DeleteTodoCommand(id), "Todo removed.", cancellationToken);

    private async Task<IActionResult> ExecuteAndRedirectAsync<TCommand>(
        TCommand command,
        string successMessage,
        CancellationToken cancellationToken)
        => await ExecuteAndHandleAsync(
            command,
            successMessage,
            () => RedirectToPage(new { Filter, SearchTerm }),
            LoadAsync,
            cancellationToken);

    private async Task LoadAsync(CancellationToken cancellationToken)
    {
        TodoList = await Sender.Send(new GetTodoListQuery(Filter, SearchTerm), cancellationToken);
    }

    public sealed class CreateTodoInput
    {
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public TodoPriority Priority { get; set; } = TodoPriority.Medium;

        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }
    }

    public sealed record FilterItem(TodoListFilter Value, string Label, string Description);
}
