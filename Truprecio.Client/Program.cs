using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazored.LocalStorage;
using System.Net.Http.Headers;

namespace Truprecio.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            // Local Storage para guardar/leer el token
            builder.Services.AddBlazoredLocalStorage();

            // HttpClient que añade el token automáticamente
            builder.Services.AddScoped(sp =>
            {
                var localStorage = sp.GetRequiredService<ILocalStorageService>();
                var client = new HttpClient(new AuthHeaderHandler(localStorage))
                {
                    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
                };
                return client;
            });

            await builder.Build().RunAsync();
        }
    }

    // DelegatingHandler que agrega el token
    public class AuthHeaderHandler : DelegatingHandler
    {
        private readonly ILocalStorageService _localStorage;

        public AuthHeaderHandler(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");

            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
