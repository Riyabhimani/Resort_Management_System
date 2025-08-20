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
                    Role = role // ✅ Sending role if required
                };

                var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync("https://localhost:7093/api/User/login", content);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }

                return null;
            }

        }
    }
