using System.Text.Json.Serialization;

namespace IssueUnlockDoor.Services;

/// <summary>
/// Verified Employee scheme
/// </summary>
public class CredentialsClaims
{
    [JsonPropertyName("doorCode")]
    public string? DoorCode { get; set; } = string.Empty;
}
