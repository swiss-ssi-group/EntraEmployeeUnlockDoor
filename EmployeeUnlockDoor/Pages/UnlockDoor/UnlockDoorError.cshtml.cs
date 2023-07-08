using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EmployeeUnlockDoor.Pages.UnlockDoor;

[AllowAnonymous]
public class UnlockDoorErrorModel : PageModel
{
    public string? ErrorMessage { get; set; }

    public IActionResult OnGet()
    {
        ErrorMessage = $"Something went wrong: your cannot enter!";
        return Page();
    }
}
