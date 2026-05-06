namespace TodoApp.Web.Pages;

public class IndexModel : PageModel
{
    public IActionResult OnGet()
        => RedirectToPage("/todos/index");
}
