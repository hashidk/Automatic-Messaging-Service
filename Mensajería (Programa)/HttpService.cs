using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Mensajería
{
    public class HttpService
    {
        private readonly HttpClient _client;

        public HttpService(string uri)
        {
            this._client = new() { BaseAddress = new Uri(uri), };
            this._client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "EAALrIISbTX8BOy8xZB3ji70zZBppHvsOGi39NwTx4vsb8RNWjq2wf6ob2fWWRBdUuawpnZARVrnKayLpClZA2DqnBYWY3WkEcLwc6TgAp2FyGKFyTKMdDCR7D4qGL5kp4OIUpMKHAVmPzUfVXRuUH02DCsKqKfyN1JAB5wmjlAMRisyKoyTL8xSeugSUOvvsbVjNj0FJloO6WlUG4Podnp30cSopapzLZBBMZD");
            this._client.DefaultRequestHeaders.Add("Accept", "application/json");
        }


        public async Task<string> GetAsync()
        {
            using HttpResponseMessage response = await this._client.GetAsync("");
            return await response.Content.ReadAsStringAsync();
        }
        public async Task<string> PostAsync(string phone)
        {
            using StringContent jsonContent = new(
                JsonSerializer.Serialize(
                new
                {
                    messaging_product = "whatsapp",
                    to = phone,
                    type = "template",
                    template =
                    new
                    {
                        name = "hello_world",
                        language = new { code = "en_US" }
                    }
                }),
                    Encoding.UTF8,
                    "application/json");

            using HttpResponseMessage response = await this._client.PostAsync(
                "",
                jsonContent);

            return await response.Content.ReadAsStringAsync();
        }
    }
}
