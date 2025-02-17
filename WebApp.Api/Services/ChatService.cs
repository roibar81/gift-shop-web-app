using System.Text;
using System.Text.Json;

namespace WebApp.Api.Services;

public class ChatService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public ChatService(IConfiguration configuration)
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri("https://api.openai.com/v1/");
        _apiKey = configuration["OpenAI:ApiKey"] ?? throw new ArgumentNullException("OpenAI:ApiKey");
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
    }

    public async Task<string> GetChatResponseAsync(string userMessage)
    {
        var requestBody = new
        {
            model = "gpt-3.5-turbo",
            messages = new[]
            {
                new { role = "system", content = "You are a helpful gift advisor assistant." },
                new { role = "user", content = userMessage }
            },
            max_tokens = 150
        };

        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("chat/completions", content);
        
        if (!response.IsSuccessStatusCode)
            return "I apologize, but I'm having trouble processing your request at the moment.";

        var responseBody = await response.Content.ReadAsStringAsync();
        var responseObject = JsonSerializer.Deserialize<JsonElement>(responseBody);
        return responseObject.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? "";
    }
} 