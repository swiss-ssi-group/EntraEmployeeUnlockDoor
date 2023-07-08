using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EmployeeUnlockDoor.Pages.UnlockDoor;

[AllowAnonymous]
public class UnlockDoorErrorModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string? UnlockErrorMessage { get; set; }

    public string? ErrorMessage { get; set; }

    public IActionResult OnGet()
    {
        ErrorMessage = $"Something went wrong: your cannot enter: {UnlockErrorMessage}";
        return Page();
    }
}
