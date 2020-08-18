using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using BlazorTest.Client.Services;

namespace BlazorTest.Client
{
    public class Program
    {
        internal static string Scope = "8614ce97-ec3e-421d-b639-3a6563b2ff50/API.Access";
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");
            //builder.HostEnvironment.BaseAddress
            builder.Services.AddHttpClient("BlazorTest.ServerAPI", client => client.BaseAddress = new Uri("https://blazortestapi.azurewebsites.net/"))
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            // Supply HttpClient instances that include access tokens when making requests to the server project
            builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("BlazorTest.ServerAPI"));

            builder.Services.AddMsalAuthentication(options =>
            {
                builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
                options.ProviderOptions.DefaultAccessTokenScopes.Add("8614ce97-ec3e-421d-b639-3a6563b2ff50/API.Access");
            });

            builder.Services.AddGrpcBearerTokenProvider();

            builder.Services.AddScoped(services =>
            {
                // Create a channel with a GrpcWebHandler that is addressed to the backend server. GrpcWebText is used because server streaming requires it.
                var httpHandler = new GrpcWebHandler(GrpcWebMode.GrpcWebText, new HttpClientHandler());
                return GrpcChannel.ForAddress(builder.HostEnvironment.BaseAddress, new GrpcChannelOptions { HttpHandler = httpHandler });
            });

            await builder.Build().RunAsync();
        }
    }
}
