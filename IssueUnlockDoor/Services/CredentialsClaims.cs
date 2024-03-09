using System.Text.Json.Serialization;

namespace IssueUnlockDoor.Services;

/// <summary>
/// self-issued-attestation
/// </summary>
public class CredentialsClaims
{
    [JsonPropertyName("doorCode")]
    public string? DoorCode { get; set; } = string.Empty;
}
