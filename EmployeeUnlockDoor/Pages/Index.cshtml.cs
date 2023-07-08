using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EmployeeUnlockDoor.Pages;

public class IndexModel : PageModel
{
    public IActionResult OnGet()
    {
        return Redirect("/UnlockDoor/UnlockDoor");
    }
}
