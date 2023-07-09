using System.Text.Json.Serialization;

namespace EmployeeUnlockDoor.Services;

public class DoorCodeClaims
{
    [JsonPropertyName("doorCode")]
    public string? DoorCode { get; set; }
}
