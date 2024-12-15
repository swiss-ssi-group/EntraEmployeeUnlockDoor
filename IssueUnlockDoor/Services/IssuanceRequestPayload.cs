using System.Text.Json.Serialization;

namespace IssueUnlockDoor.Services;

/// <summary>
/// https://learn.microsoft.com/en-us/azure/active-directory/verifiable-credentials/credential-design
/// https://learn.microsoft.com/en-us/azure/active-directory/verifiable-credentials/presentation-request-api
/// </summary>
public class IssuanceRequestPayload
{
    [JsonPropertyName("authority")]
    public string Authority { get; set; } = string.Empty;
    [JsonPropertyName("includeReceipt")]
    public bool IncludeReceipt { get; set; }
    [JsonPropertyName("registration")]
    public Registration Registration { get; set; } = new Registration();
    [JsonPropertyName("callback")]
    public Callback Callback { get; set; } = new Callback();


    [JsonPropertyName("includeQRCode")]
    public bool IncludeQRCode { get; set; }

 
    [JsonPropertyName("requestedCredentials")]
    public List<RequestedCredentials> RequestedCredentials { get; set; } = new List<RequestedCredentials>();
}

public class Callback
{
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;
    [JsonPropertyName("state")]
    public string State { get; set; } = string.Empty;
    [JsonPropertyName("headers")]
    public Headers Headers { get; set; } = new Headers();
}

public class Headers
{
    [JsonPropertyName("api-key")]
    public string ApiKey { get; set; } = string.Empty;
}

public class Registration
{
    [JsonPropertyName("clientName")]
    public string ClientName { get; set; } = string.Empty;
}

public class Pin
{
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;
    [JsonPropertyName("length")]
    public int Length { get; set; } = 4;
}

public class RequestedCredentials
{
    [JsonPropertyName("type")]
    public string CrendentialsType { get; set; } = string.Empty;
    [JsonPropertyName("purpose")]
    public string Purpose { get; set; } = string.Empty;
    [JsonPropertyName("acceptedIssuers")]
    public List<string> AcceptedIssuers { get; set; } = new List<string>();
    [JsonPropertyName("configuration")]
    public RequestedConfiguration Configuration { get; set; } = new RequestedConfiguration();
}

public class RequestedConfiguration
{
    [JsonPropertyName("validation")]
    public RequestedConfigurationValidation Validation { get; set; } = new RequestedConfigurationValidation();
}

public class RequestedConfigurationValidation
{
    [JsonPropertyName("allowRevoked")]
    public bool AllowRevoked { get; set; }
    [JsonPropertyName("validateLinkedDomain")]
    public bool ValidateLinkedDomain { get; set; }
}

