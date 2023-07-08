using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Authentication;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EmployeeUnlockDoor.Pages.UnlockDoor;

[AllowAnonymous]
public class UnlockedModel : PageModel
{
    private readonly ValidateUserAndDoorCodeService _validateUserAndDoorCodeService;

    public UnlockedModel(ValidateUserAndDoorCodeService validateUserAndDoorCodeService)
    {
        _validateUserAndDoorCodeService = validateUserAndDoorCodeService;
    }

    [BindProperty]
    public string? StatePresented { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var upn = HttpContext.User.FindFirst("RevocationId");
        var doorCode = HttpContext.User.FindFirst("DoorCode");

        // Validate door code and VC claims
        (bool IsValid, string Error) validation = _validateUserAndDoorCodeService
                .PaycheckIdAndUserAreValid(upn!.Value, doorCode!.Value);

        if (!validation.IsValid)
        {
            await HttpContext.SignOutAsync();
            return Redirect($"~/UnlockDoor/UnlockDoorError/{validation.Error}");
        }

        // Data should be fetched from a DB or an ERP service etc.

        return Page();
    }
}
