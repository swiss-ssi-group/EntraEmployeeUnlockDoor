using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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

    [BindProperty]
    public string? NameSurnameMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var upn = HttpContext.User.FindFirst("RevocationId");
        var doorCode = HttpContext.User.FindFirst("DoorCode");

        var givenName = HttpContext.User.FindFirst("GivenName");
        var surname = HttpContext.User.FindFirst("Surname");

        NameSurnameMessage = $"Have a nice day {givenName!.Value} {surname!.Value}";

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
