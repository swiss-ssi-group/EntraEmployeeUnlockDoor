# Entra Verified ID Employee Unlock Door flow

[![.NET](https://github.com/swiss-ssi-group/EntraEmployeeUnlockDoor/actions/workflows/dotnet.yml/badge.svg)](https://github.com/swiss-ssi-group/EntraEmployeeUnlockDoor/actions/workflows/dotnet.yml) [![Deploy EmployeeUnlockDoor](https://github.com/swiss-ssi-group/EntraEmployeeUnlockDoor/actions/workflows/azure-deploy-verifier.yml/badge.svg)](https://github.com/swiss-ssi-group/EntraEmployeeUnlockDoor/actions/workflows/azure-deploy-verifier.yml) [![Deploy Issue Door unlocked](https://github.com/swiss-ssi-group/EntraEmployeeUnlockDoor/actions/workflows/azure-deploy-issuer.yml/badge.svg)](https://github.com/swiss-ssi-group/EntraEmployeeUnlockDoor/actions/workflows/azure-deploy-issuer.yml)

[Use multiple Microsoft Entra Verified ID credentials in a verification presentation](https://damienbod.com/2023/08/31/use-multiple-microsoft-entra-verified-id-credentials-in-a-verification-presentation/)

## Deployment testing

### Get onboarded to the Azure AD tenant

Contact the HR, or your IT admin and ask nicely for an account.

### Get your verified employee credential

https://issueverifiableemployee.azurewebsites.net

### Get your door unlocker employee credential

Code for demo: 2023

https://issueunlockdoor.azurewebsites.net

### View your paycheck using the verified employee credential and the door unlocker credential

https://employeeunlockdoor.azurewebsites.net

## History

- 2024-12-15 .NET 9, Updated packages, updated deployments
- 2024-03-09 Updated packages
- 2023-07-27 Updated packages
- 2023-07-08 Initial version

## User secrets and verify configuration

Select the correct endpoint depending to the business of the application.

```
{
  "CredentialSettings": {
    "Endpoint": "https://verifiedid.did.msidentity.com/v1.0/verifiableCredentials/createPresentationRequest",
    //  "Endpoint": "https://verifiedid.did.msidentity.com/v1.0/verifiableCredentials/createIssuanceRequest",
    "VCServiceScope": "bbb94529-53a3-4be5-a069-7eaf2712b826/.default",
    "Instance": "https://login.microsoftonline.com/{0}",
    "TenantId": "YOURTENANTID",
    "ClientId": "APPLICATION CLIENT ID",
    "VcApiCallbackApiKey": "SECRET",
    "Authority": "YOUR authority",
    "ClientSecret": "[client secret or instead use the prefered certificate in the next entry]",
    // "CertificateName": "[Or instead of client secret: Enter here the name of a certificate (from the user cert store) as registered with your application]",
    "IssuerAuthority": "YOUR VC SERVICE DID",
    "VerifierAuthority": "YOUR VC SERVICE DID",
    "CredentialManifest":  "THE CREDENTIAL URL FROM THE VC PORTAL"
  }
}

```

## Issue Entra Verified ID Employee credentials

https://github.com/swiss-ssi-group/EntraVerifiedEmployee

## Local debugging, required for callback

```
ngrok http https://localhost:5002
```

## Links

https://learn.microsoft.com/en-us/azure/active-directory/verifiable-credentials/how-to-use-quickstart-multiple

https://learn.microsoft.com/en-us/azure/active-directory/verifiable-credentials/admin-api#attestations-type

https://github.com/swiss-ssi-group/AzureADVerifiableCredentialsAspNetCore

https://learn.microsoft.com/en-us/azure/active-directory/verifiable-credentials/decentralized-identifier-overview

https://ssi-start.adnovum.com/data

https://github.com/e-id-admin/public-sandbox-trustinfrastructure/discussions/14

https://openid.net/specs/openid-connect-self-issued-v2-1_0.html

https://identity.foundation/jwt-vc-presentation-profile/

https://learn.microsoft.com/en-us/azure/active-directory/verifiable-credentials/verifiable-credentials-standards

https://github.com/Azure-Samples/active-directory-verifiable-credentials-dotnet

https://aka.ms/mysecurityinfo

https://fontawesome.com/

https://developer.microsoft.com/en-us/graph/graph-explorer?tenant=damienbodsharepoint.onmicrosoft.com

https://learn.microsoft.com/en-us/graph/api/overview?view=graph-rest-1.0

https://github.com/Azure-Samples/VerifiedEmployeeIssuance

https://github.com/Azure-Samples/active-directory-verifiable-credentials-dotnet

https://github.com/AzureAD/microsoft-identity-web/blob/jmprieur/Graph5/src/Microsoft.Identity.Web.GraphServiceClient/Readme.md#replace-the-nuget-packages

https://docs.microsoft.com/azure/app-service/deploy-github-actions#configure-the-github-secret

https://issueverifiableemployee.azurewebsites.net/

https://datatracker.ietf.org/doc/draft-ietf-oauth-selective-disclosure-jwt/
