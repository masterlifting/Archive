#### Chemikalien-Gesellschaft Hans Lungmuß mbH & Co. KG

Feuerfest seit 1958.
Ihr Spezialist für innovative feuerfeste Produkte.

# Project / Repository Overview

Project ID: `FF-178` (Always use this ID, in addition to the GitHub issue ID, in the commit messages)

# Dashboard

## fully qualified domain name

### Warehouse

https://feuerfest.app/warehouse

---

The opening page for the inbound process.
On this page the user can select to start an inbound process.

https://feuerfest.app/warehouse/inbound

start → https://feuerfest.app/warehouse/inbound/start

On this page the user will see a list of started inbound processes.

The started processes are stored inside the state store with a key
starting with `inbound-start-`.

He can then select to start a new inbound process or select an already started process.

---

When the user starts a process, we always generate a unique id.
Use this id store the state in the state store.

Prefix the key with `inbound-`.

Example: `inbound-EjH9kVy6XEWtHOhUk2ex4g`

Use the method [GenerateBase64UrlEncodeHumanEyesFriendly](https://github.com/lungmuss/lungmuss.refractory.library/blob/8c4f996906d49ec8b7697117c5ac089e09eab68a/Lungmuss.Refractory.Library/Generators.cs#L14C57-L14C57) to generate a url friendly UUID.

https://feuerfest.app/warehouse/inbound/start/EjH9kVy6XEWtHOhUk2ex4g

On this page there are four fields:

- Order Number
- Container type
- Number of containers
- Approx. weight of the containers
- Article / Supplier

## Login
 

### Environment variables

ASPNETCORE_URLS

The environment variable ASPNETCORE_URLS is used to set the URL that the application listens on. The default URL should be set to: http://localhost:80

ASPNETCORE_ENVIRONMENT

The environment variable ASPNETCORE_ENVIRONMENT is used to set the environment for the application. The environment can be Development, Staging, or Production. The environment is used to set the logging level. For example, if the environment is set to Development, then the logging level is set to Debug. If the environment is set to Production, then the logging level is set to Information.

LOKI_URL

Valid LOKI Url.

OIDC_VALID_ISSUER

Set to the OIDC issuer URL. Example: "https://auth.feuerfest.app/auth/realms/master"

OIDC_VALID_AUDIENCE

Set to the OIDC audience URL. Example: "master-realm"

OIDC_AUTHORITY

set to the OIDC authority URL. Example: "https://auth.feuerfest.app/auth/realms/master"

# ASP.NET Core as a proxy

To proxy the internal stream through your externally accessible Blazor server application in a streaming fashion, you can use a simple ASP.NET Core middleware or controller action. Here's a step-by-step guide:

1. **Adding Necessary NuGet Packages**:
   Add the necessary package for HTTP client factory:

   ```
   dotnet add package Microsoft.Extensions.Http
   ```

2. **Setting up an HTTP Client**:
   Configure an `HttpClient` in `Startup.cs` or `Program.cs` (based on your .NET version):

   ```csharp
   public void ConfigureServices(IServiceCollection services)
   {
       services.AddHttpClient("InternalService", c =>
       {
           c.BaseAddress = new Uri("http://internal-service-address/");
       });

       // ... other services
   }
   ```

3. **Proxying the Stream**:
   You can set up a Controller action to proxy the stream. Here's a simple example:

   ```csharp
   [Route("api/proxy")]
   [ApiController]
   public class ProxyController : ControllerBase
   {
       private readonly IHttpClientFactory _clientFactory;

       public ProxyController(IHttpClientFactory clientFactory)
       {
           _clientFactory = clientFactory;
       }

       [HttpGet("pdf")]
       public async Task<IActionResult> GetPdf()
       {
           var client = _clientFactory.CreateClient("InternalService");
           var response = await client.GetAsync("path-to-pdf-endpoint");

           if (response.IsSuccessStatusCode)
           {
               var stream = await response.Content.ReadAsStreamAsync();
               return File(stream, "application/pdf");
           }

           return BadRequest(); // or another appropriate error response
       }
   }
   ```

4. **Access from Blazor**:
   You can then access this endpoint from your Blazor server application, e.g., by setting the `src` attribute of an `<iframe>` or `<embed>` to display the PDF content:

   ```razor
   <embed src="/api/proxy/pdf" type="application/pdf" width="100%" height="600px" />
   ```

With this setup, when you request the specific route on your Blazor server, it fetches the content from the internal Kubernetes service and serves it to you. Ensure that the Blazor server app's pod has the necessary network permissions to communicate with the internal service inside the Kubernetes cluster.

# UI Error handling

### The user's data is not lost when an error occurs.
### The user can see detailed information about the error if the environment is set to Development.
"ASPNETCORE_ENVIRONMENT": "Development"

## NotImplementedException

We catch the NotImplementedException and display a message that the feature has not been implemented yet.

## NotSupportedExeption

We catch the NotSupportedExeption and display a message that the action is not supported.

## Rest exceptions

We catch the rest of the exceptions and display a general message.

### We can handle each exception type differently and use different services for it.

#