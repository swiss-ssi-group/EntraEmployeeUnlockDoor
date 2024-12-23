using IssueUnlockDoor.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;

namespace IssueUnlockDoor;

public class IssuerService
{
    protected readonly CredentialSettings _credentialSettings;
    protected IMemoryCache _cache;
    protected readonly ILogger<IssuerService> _log;

    public IssuerService(IOptions<CredentialSettings> credentialSettings,
        IMemoryCache memoryCache,
        ILogger<IssuerService> log)
    {
        _credentialSettings = credentialSettings.Value;
        _credentialSettings ??= new CredentialSettings();

        _cache = memoryCache;
        _log = log;
    }

    public IssuanceRequestPayload GetIssuanceRequestPayload(HttpRequest request)
    {
        var payload = new IssuanceRequestPayload();

        payload.CredentialsType = "DoorCode";

        payload.Manifest = $"{_credentialSettings.CredentialManifest}";

        var host = GetRequestHostName(request);
        payload.Callback.State = Guid.NewGuid().ToString();
        payload.Callback.Url = $"{host}/api/issuer/issuanceCallback";
        payload.Callback.Headers.ApiKey = _credentialSettings.VcApiCallbackApiKey;

        payload.Registration.ClientName = "Door Code";
        payload.Authority = _credentialSettings.IssuerAuthority;

        return payload;
    }

    public async Task<(string Token, string Error, string ErrorDescription)> GetAccessToken()
    {
        var isUsingClientSecret = _credentialSettings.AppUsesClientSecret(_credentialSettings);

        IConfidentialClientApplication app;
        if (isUsingClientSecret)
        {
            app = ConfidentialClientApplicationBuilder.Create(_credentialSettings.ClientId)
                .WithClientSecret(_credentialSettings.ClientSecret)
                .WithAuthority(new Uri(_credentialSettings.Authority))
                .Build();
        }
        else
        {
            var certificate = _credentialSettings.ReadCertificate(_credentialSettings.CertificateName);
            app = ConfidentialClientApplicationBuilder.Create(_credentialSettings.ClientId)
                .WithCertificate(certificate)
                .WithAuthority(new Uri(_credentialSettings.Authority))
                .Build();
        }

        // With client credentials flows the scopes is ALWAYS of the shape "resource/.default"
        var scopes = new string[] { _credentialSettings.VCServiceScope };

        AuthenticationResult? result;
        try
        {
            result = await app.AcquireTokenForClient(scopes)
                .ExecuteAsync();
        }
        catch (MsalServiceException ex) when (ex.Message.Contains("AADSTS70011"))
        {
            // Invalid scope. The scope has to be of the form "https://resourceurl/.default"
            // Mitigation: change the scope to be as expected
            return (string.Empty, "500", "Scope provided is not supported");
            //return BadRequest(new { error = "500", error_description = "Scope provided is not supported" });
        }
        catch (MsalServiceException ex)
        {
            // general error getting an access token
            return (string.Empty, "500", "Something went wrong getting an access token for the client API:" + ex.Message);
            //return BadRequest(new { error = "500", error_description = "Something went wrong getting an access token for the client API:" + ex.Message });
        }

        _log.LogTrace("{AccessToken}", result.AccessToken);
        return (result.AccessToken, string.Empty, string.Empty);
    }

    public string GetRequestHostName(HttpRequest request)
    {
        var scheme = "https";// : this.Request.Scheme;
        var originalHost = request.Headers["x-original-host"];
        if (!string.IsNullOrEmpty(originalHost))
        {
            return $"{scheme}://{originalHost}";
        }
        else
        {
            return $"{scheme}://{request.Host}";
        }
    }
}
