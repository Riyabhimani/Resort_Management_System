using Newtonsoft.Json;
using System.Text;

namespace Resort_Management_System_MVC.Service
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string?> AuthenticateUserAsync(string username, string password, string? role = null)
        {
            var requestData = new
            {
                UserName = username,
                Password = password,
                Role = role 
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("http://localhost:5159/api/Login/login", content);

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsStringAsync();

            var jsonData = await response.Content.ReadAsStringAsync();
            return jsonData;
        }
    }
}
