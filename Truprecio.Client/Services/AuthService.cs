using System.Net.Http.Json;
using Truprecio.Shared;

namespace Truprecio.Client.Services
{
    public class AuthService
    {
        private readonly HttpClient _http;
        private readonly LocalStorageService _localStorage;

        public AuthService(HttpClient http, LocalStorageService localStorage)
        {
            _http = http;
            _localStorage = localStorage;
        }

        public async Task<bool> Login(LoginRequest request)
        {
            var response = await _http.PostAsJsonAsync("api/auth/login", request);

            if (!response.IsSuccessStatusCode)
                return false;

            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();

            if (loginResponse != null)
            {
                await _localStorage.SetItem("authToken", loginResponse.Token);
                return true;
            }

            return false;
        }
    }
}
