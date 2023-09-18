using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Configuration;

namespace AutomaticMessaging
{
    public class HttpService
    {
        private readonly HttpClient _client;

        public HttpService(string uri)
        {
            var token = ConfigurationManager.AppSettings["APIauthtoken"];
            this._client = new HttpClient() { BaseAddress = new Uri(uri), };
            this._client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            this._client.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public async Task<string> GetAsync()
        {
            HttpResponseMessage response = await this._client.GetAsync("");
            return await response.Content.ReadAsStringAsync();
        }
        public async Task<string> PostAsync(string phone, string message)
        {
            string nada;
            StringContent jsonContent = new StringContent(
                JsonSerializer.Serialize(
                new
                {
                    messaging_product = "whatsapp",
                    to = phone,
                    type = "template",
                    template = new { name = "hello_world", language = new { code = "en_US" } }
                }),
                    Encoding.UTF8,
                    "application/json");

            try
            {
                HttpResponseMessage response = await this._client.PostAsync("", jsonContent);
                response.EnsureSuccessStatusCode();
                return null;
            }
            catch (Exception ex)
            {
                new Logger().WriteToFile("[Error] " + ex.HResult.ToString("X") + " Message: " + ex.Message);
                return ex.Message;
            }
        }
    }
}
