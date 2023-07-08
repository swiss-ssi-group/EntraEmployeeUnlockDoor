using System.Text.Json.Serialization;

namespace IssueUnlockDoor.Services;

/// <summary>
/// Verified Employee scheme
/// </summary>
public class CredentialsClaims
{
    [JsonPropertyName("doorCode")]
    public string? DoorCode { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}
