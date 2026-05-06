namespace TodoApp.Web.Pages;

public abstract class BasePageModel(ISender sender) : PageModel
{
    protected ISender Sender { get; } = sender;

    [TempData]
    public string? StatusMessage { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    protected async Task<IActionResult> ExecuteAndHandleAsync<TCommand>(
        TCommand command,
        string successMessage,
        Func<IActionResult> onSuccess,
        Func<CancellationToken, Task> onFailureReload,
        CancellationToken cancellationToken)
    {
        try
        {
            await Sender.Send(command!, cancellationToken);
            StatusMessage = successMessage;
            return onSuccess();
        }
        catch (Exception ex)
        {
            ErrorMessage = MapErrorMessage(ex);
        }

        await onFailureReload(cancellationToken);
        return Page();
    }

    protected virtual string MapErrorMessage(Exception ex)
        => ex switch
        {
            BusinessException businessException => businessException.ErrorCode ?? businessException.Message,
            _ => ex.Message
        };
}
