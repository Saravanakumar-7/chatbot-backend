using System.Text.Json;
using Tips.Grin.Api.Contracts;

namespace Tips.Grin.Api.Extensions
{
    public class TokenValidationService : ITokenValidationService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        public TokenValidationService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<bool> IsTokenValid(string token)
        {
            var response = await _httpClient.GetAsync(string.Concat(_config["MasterService"], $"Auth/ValidateToken/validate?token={token}"));

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<TokenValidationResult>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return result?.IsValid == true;
            }

            return false;
        }

        private class TokenValidationResult
        {
            public bool IsValid { get; set; }
            public string Message { get; set; }
        }
    }
}
