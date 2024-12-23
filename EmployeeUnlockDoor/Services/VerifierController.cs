using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using EmployeeUnlockDoor.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using VerifierInsuranceCompany.Services;

namespace VerifierInsuranceCompany;

[Route("api/[controller]/[action]")]
public class VerifierController : Controller
{
    protected readonly CredentialSettings _credentialSettings;
    protected readonly IDistributedCache _distributedCache;
    protected readonly ILogger<VerifierController> _log;
    private readonly VerifierService _verifierService;
    private readonly HttpClient _httpClient;

    public VerifierController(IOptions<CredentialSettings> appSettings,
        IDistributedCache distributedCache,
        ILogger<VerifierController> log,
        VerifierService verifierService,
        IHttpClientFactory httpClientFactory)
    {
        _credentialSettings = appSettings.Value;
        _distributedCache = distributedCache;
        _log = log;
        _verifierService = verifierService;
        _httpClient = httpClientFactory.CreateClient();
    }

    [HttpGet("/api/verifier/presentation-request")]
    public async Task<ActionResult> PresentationRequest()
    {
        try
        {
            var payload = _verifierService.GetVerifierRequestPayload(Request);
            var (Token, Error, ErrorDescription) = await _verifierService.GetAccessToken();

            if (string.IsNullOrEmpty(Token))
            {
                _log.LogError("failed to acquire accesstoken: {Error} : {ErrorDescription}", Error, ErrorDescription);
                return BadRequest(new { error = Error, error_description = ErrorDescription });
            }

            var defaultRequestHeaders = _httpClient.DefaultRequestHeaders;
            defaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

            var res = await _httpClient.PostAsJsonAsync(
                _credentialSettings.Endpoint, payload);

            if (res.IsSuccessStatusCode)
            {
                var response = await res.Content.ReadFromJsonAsync<VerifierResponse>();
                response!.Id = payload.Callback.State;
                _log.LogTrace("succesfully called Request API");

                if (res.StatusCode == HttpStatusCode.Created)
                {
                    var cacheData = new CacheData
                    {
                        Status = VerifierConst.NotScanned,
                        Message = "Request ready, please scan with Authenticator",
                        Expiry = response.Expiry.ToString(CultureInfo.InvariantCulture),
                    };
                    CacheData.AddToCache(payload.Callback.State, _distributedCache, cacheData);

                    return Ok(response);
                }
            }
            else
            {
                var message = await res.Content.ReadAsStringAsync();

                _log.LogError("Unsuccesfully called Request API {message}", message);
                return BadRequest(new { error = "400", error_description = message });
            }

            var errorResponse = await res.Content.ReadAsStringAsync();
            _log.LogError("Unsuccesfully called Request API");
            return BadRequest(new { error = "400", error_description = "Something went wrong calling the API: " + errorResponse });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "400", error_description = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult> PresentationCallback()
    {
        var content = await new StreamReader(Request.Body).ReadToEndAsync();
        var verifierCallbackResponse = JsonSerializer.Deserialize<VerifierCallbackResponse>(content);

        try
        {
            if (verifierCallbackResponse != null && verifierCallbackResponse.RequestStatus == VerifierConst.RequestRetrieved)
            {
                var cacheData = new CacheData
                {
                    Status = VerifierConst.RequestRetrieved,
                    Message = "QR Code is scanned. Waiting for validation...",
                };
                CacheData.AddToCache(verifierCallbackResponse.State, _distributedCache, cacheData);
            }

            if (verifierCallbackResponse != null && verifierCallbackResponse.RequestStatus == VerifierConst.PresentationVerified)
            {
                var cacheData = new CacheData
                {
                    Status = VerifierConst.PresentationVerified,
                    Message = "Presentation verified",
                    Payload = JsonSerializer.Serialize(verifierCallbackResponse.VerifiedCredentialsData),
                    Subject = verifierCallbackResponse.Subject
                };

                ParseClaimsData(verifierCallbackResponse!.VerifiedCredentialsData[0], cacheData);
                ParseClaimsData(verifierCallbackResponse!.VerifiedCredentialsData[1], cacheData);

                CacheData.AddToCache(verifierCallbackResponse.State, _distributedCache, cacheData);
            }

            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "400", error_description = ex.Message });
        }
    }

    private static void ParseClaimsData(VerifiedCredentialsData verifiedCredentialsData, CacheData cacheData)
    {
        if (verifiedCredentialsData.CredentialTypes.Contains("VerifiedEmployee"))
        {
            var data = verifiedCredentialsData!.Claims.Deserialize<EmployeeClaims>();

            cacheData.Employee.Photo = data!.Photo;
            cacheData.Employee.RevocationId = data!.RevocationId;
            cacheData.Employee.PreferredLanguage = data!.PreferredLanguage;
            cacheData.Employee.Surname = data!.Surname;
            cacheData.Employee.GivenName = data!.GivenName;
            cacheData.Employee.DisplayName = data!.DisplayName;
            cacheData.Employee.Mail = data!.Mail;
            cacheData.Employee.JobTitle = data!.JobTitle;
        }
        else // "DoorCode"
        {
            var data = verifiedCredentialsData!.Claims.Deserialize<DoorCodeClaims>();
            if (data!.DoorCode != null)
                cacheData.DoorCode = data.DoorCode;
        }
    }

    [HttpGet("/api/verifier/presentation-response")]
    public ActionResult PresentationResponse()
    {
        try
        {
            string? state = Request.Query["id"];
            if (state == null)
            {
                return BadRequest(new { error = "400", error_description = "Missing argument 'id'" });
            }

            var data = CacheData.GetFromCache(state, _distributedCache);
            if (data != null)
            {
                Debug.WriteLine("check if there was a response yet: " + data);
                return new ContentResult
                {
                    ContentType = "application/json",
                    Content = JsonSerializer.Serialize(data)
                };
            }

            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "400", error_description = ex.Message });
        }
    }
}
