using TodoApp.Application.Commands.Todos;
using TodoApp.Application.Dtos.Todos;
using TodoApp.Application.Queries.Todos;
using TodoApp.Domain.Aggregates.Todos;
using DomainTodo = TodoApp.Domain.Aggregates.Todos.Todo;

namespace TodoApp.Web.Pages.Todos;

public class DetailsModel(ISender sender) : BasePageModel(sender)
{
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty(SupportsGet = true)]
    public TodoListFilter? ReturnFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? ReturnSearchTerm { get; set; }

    [BindProperty]
    public EditTodoInput Input { get; set; } = new();

    public TodoDto Todo { get; private set; } = default!;

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        return await LoadPageAsync(cancellationToken);
    }

    public async Task<IActionResult> OnPostSaveAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return await LoadPageAsync(cancellationToken);
        }

        try
        {
            await Sender.Send(
                new UpdateTodoCommand(Id, Input.Title, Input.Description, Input.Priority, Input.DueDate),
                cancellationToken);

            StatusMessage = "Todo updated.";
            return RedirectToPage(new { Id, ReturnFilter, ReturnSearchTerm });
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            return await LoadPageAsync(cancellationToken);
        }
    }

    public Task<IActionResult> OnPostCompleteAsync(CancellationToken cancellationToken)
        => ExecuteAndReloadAsync(new CompleteTodoCommand(Id), "Todo completed.", cancellationToken);

    public Task<IActionResult> OnPostReopenAsync(CancellationToken cancellationToken)
        => ExecuteAndReloadAsync(new ReopenTodoCommand(Id), "Todo reopened.", cancellationToken);

    public Task<IActionResult> OnPostTogglePinAsync(CancellationToken cancellationToken)
        => ExecuteAndReloadAsync(new TogglePinTodoCommand(Id), "Pin status updated.", cancellationToken);

    public async Task<IActionResult> OnPostDeleteAsync(CancellationToken cancellationToken)
        => await ExecuteAndHandleAsync(
            new DeleteTodoCommand(Id),
            "Todo removed.",
            () => RedirectToPage("/Todos/Index", new { Filter = ReturnFilter, SearchTerm = ReturnSearchTerm }),
            LoadPageAsync,
            cancellationToken);

    private async Task<IActionResult> ExecuteAndReloadAsync<TCommand>(TCommand command, string successMessage, CancellationToken cancellationToken)
        => await ExecuteAndHandleAsync(
            command,
            successMessage,
            () => RedirectToPage(new { Id, ReturnFilter, ReturnSearchTerm }),
            LoadPageAsync,
            cancellationToken);

    private async Task<IActionResult> LoadPageAsync(CancellationToken cancellationToken)
    {
        try
        {
            Todo = await Sender.Send(new GetTodoByIdQuery(Id), cancellationToken);
            Input = new EditTodoInput
            {
                Title = Todo.Title,
                Description = Todo.Description,
                Priority = Todo.Priority,
                DueDate = Todo.DueDate
            };

            return Page();
        }
        catch (Exception ex)
        {
            ErrorMessage ??= ex.Message;
            return RedirectToPage("/Todos/Index", new { Filter = ReturnFilter, SearchTerm = ReturnSearchTerm });
        }
    }

    public sealed class EditTodoInput
    {
        [Required]
        [MaxLength(DomainTodo.TitleMaxLength)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(DomainTodo.DescriptionMaxLength)]
        public string? Description { get; set; }

        public TodoPriority Priority { get; set; } = TodoPriority.Medium;

        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }
    }
}
