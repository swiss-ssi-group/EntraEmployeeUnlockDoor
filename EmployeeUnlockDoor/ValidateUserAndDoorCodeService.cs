namespace EmployeeUnlockDoor;

public class ValidateUserAndDoorCodeService
{
    public bool PaycheckIdAndUserAreValid(string? userPrincipalName, string? doorCode)
    {
        if (userPrincipalName == null || doorCode == null)
            return false;

        // Get door code data using API and validate that upn matches user which have access
        // This can be specific to your IoT system

        // simi check, only accept requests for "2023" and any upn
        // This code be updated once a month
        if(doorCode == "2023")
            return true;

        return false;
    }
}
