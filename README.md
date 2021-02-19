# AzureADXNETCoreWebApp
.NET Core Web App to manage Azure ADX data, with Azure B2C authentication

## Overview
This project is a .NET Core Web App that manages Azure ADX data, with Azure B2C Authentication. It was created as a POC for several concepts and is an example of how you can leverage these components for a solution.

This project was originall ported from the **Azure Samples Active Directory ASP .NET Core Web App OpenID Connect** project. This project provides all the required for functionality to authenticate users with Azure B2C. The **1-5-B2C** section was used for this POC.

https://github.com/Azure-Samples/active-directory-aspnetcore-webapp-openidconnect-v2/tree/master/1-WebApp-OIDC/1-5-B2C

## Key Aspects 
The following are key aspects of the project:

 - .NET Core 3.1
 - Azure B2C authentication
 - Passes Azure B2C User OID value to an API for ADX data filtering
 - Azure Data Explorer (ADX) integration
 - [Sample ADX data ingestion](https://docs.microsoft.com/en-us/azure/data-explorer/ingest-sample-data)
 - [Kusto .NET SDK](https://docs.microsoft.com/en-us/azure/data-explorer/kusto/api/netfx/about-the-sdk) integration 
 - [MemoryCache](https://docs.microsoft.com/en-us/aspnet/core/performance/caching/memory?view=aspnetcore-3.1) integration
 - Dependency Injection

## Setup
In order to run the demo, you will need to compete the following steps to create your environment.

### Step 1: Get your own Azure AD B2C tenant

If you don't have an Azure AD B2C tenant yet, you'll need to create an Azure AD B2C tenant by following the [Tutorial: Create an Azure Active Directory B2C tenant](https://azure.microsoft.com/documentation/articles/active-directory-b2c-get-started).

### Step 2: Create your own user flow (policy)

This sample uses a unified sign-up/sign-in user flow (policy). Create this policy by following [these instructions on creating an AAD B2C tenant](https://azure.microsoft.com/documentation/articles/active-directory-b2c-reference-policies). You may choose to include as many or as few identity providers as you wish, but make sure **DisplayName** is checked in `User attributes` and `Application claims`.

If you already have an existing unified sign-up/sign-in user flow (policy) in your Azure AD B2C tenant, feel free to re-use it. The is no need to create a new one just for this sample.

Copy this policy name, so you can use it in step 5.

### Step 3: Create your own Web app

Now you need to [register your web app in your B2C tenant](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-app-registration#register-a-web-application), so that it has its own Application ID.

Your web application registration should include the following information:

- Enable the **Web App/Web API** setting for your application.
- Set the **Reply URL** to `https://localhost:44316/signin-oidc`.
- Copy the Application ID generated for your application, so you can use it in the next step.

### Step 4: Configure the sample with your app coordinates

1. Open the `appsettings.json` file.
2. Find the assignment for `Instance` and replace the value with your tenant name. For example, `https://fabrikam.b2clogin.com`
3. Find the assignment for `Domain` and replace the value with your Azure AD B2C domain name. For example, `fabrikam.onmicrosoft.com`
4. Find the assignment for `ClientID` and replace the value with the Application ID from Step 4.
5. Find the assignment for `SignUpSignInPolicyId` and replace with the name of the `Sign up and sign in` policy you created in Step 3.

```JSon
  "AzureAdB2C": {
    "Instance": "https://<your-tenant-name>.b2clogin.com",
    "ClientId": "<web-app-application-id>",
    "Domain": "<your-b2c-domain>",
    "SignedOutCallbackPath": "/signout/<your-sign-up-in-policy>",
    "SignUpSignInPolicyId": "<your-sign-up-in-policy>"
  }
```
### Step 5: Create a sample API to return the list of records for the user's assigned states.

The project uses an API to return a list of states the ADX will be filtered on. The project will pass the user's OID value (from the Azure B2C authentication) as a query pararemeter to the specifed API URL. The API should return a list of states to be used within the ADX querries.

**NOTE**

The project is confiogured to return the top 100 records for all states, if an API is not present.

Here is a sample API (using an Azure Function) to return values.

    public static async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req, ILogger log)
    {
      log.LogInformation("C# HTTP trigger function processed a request.");
      string oid = req.Query["oid"];

      List<string> lstids = new List<string>();
    
      // TODO: Go get the user's states from a data soruce
      // Simulate getting the user states
      switch (oid.ToLower())
      {
        case "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX":
          lstids.Add("CALIFORNIA");
          lstids.Add("ALASKA");
          break;
        default:
          lstids.Add("");
          break;
      }
      return new OkObjectResult(lstids);
    }

1. Create an API to return a list of user states.
2. Deploy your API to an accessible URL.


### Step 6: Configure / Populate ADX with sample data

1. Create a new ADX Cluster.
2. Create a new ADX Database.
3. Use this [guide](https://docs.microsoft.com/en-us/azure/data-explorer/ingest-sample-data) to ingest the sample **StormEvents** data.

### Stewp 7: Update your project with the ADX configuration

1. Open the `appsettings.json` file.
2. Find the assignment for `ADXCluster` and replace the value with your ADX Cluster Name.
3. Find the assignment for `ADXDatabase` and replace the value with your ADX Database Name.
4. Find the assignment for `ADXTable` and replace the value with your ADX Table Name.
5. Find the assignment for `APIURL` and replace with the URL of your API from **Step 5**.

```JSon
  "ProjectOptions": {
    "ADXCluster": "[Azure ADX Cluster Name]",
    "ADXDatabase": "[Azure ADX Database Name]",
    "ADXTable": "[Azure ADX Table Name]",
    "APIURL": "[API URL]"
  }
```

