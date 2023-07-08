namespace EmployeeUnlockDoor;

public class ValidateUserAndDoorCodeService
{
    public (bool IsValid, string Error) PaycheckIdAndUserAreValid(string? userPrincipalName, string? doorCode)
    {
        if (userPrincipalName == null || doorCode == null)
            return (false, "Error, data is missing, no upn or door code available");

        // Get door code data using API and validate that upn matches user which have access
        // This can be specific to your IoT system

        // simi check, only accept requests for "2023" and any upn
        // This code be updated once a month
        if(doorCode == "2023")
            return (true, string.Empty);

        return (false, "Error, data code is incorrect, code is '2023'");
    }
}
