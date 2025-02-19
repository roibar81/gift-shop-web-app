using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WebApp.Api.Models;

namespace WebApp.Api.Services;

public class ChatService
{
    private readonly OpenAIClient _openAIClient;
    private readonly string _deploymentName;
    private readonly ProductService _productService;
    private readonly ILogger<ChatService> _logger;

    public ChatService(IConfiguration configuration, ProductService productService, ILogger<ChatService> logger)
    {
        var apiKey = configuration["OpenAI:ApiKey"];
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new InvalidOperationException(
                "OpenAI API key is not configured. Please add your API key to appsettings.Development.json or use environment variables.");
        }
        
        _deploymentName = configuration["OpenAI:DeploymentName"] ?? "gpt-3.5-turbo";
        _openAIClient = new OpenAIClient(apiKey);
        _productService = productService;
        _logger = logger;
    }

    public async Task<string> GetResponseAsync(List<Models.ChatMessage> conversationHistory)
    {
        try
        {
            var products = await _productService.GetProductsAsync();
            
            var systemMessage = @"You are a helpful and friendly gift advisor for our online gift shop. Your goal is to help users find the perfect gift from our available products.

When suggesting products:
1. Always provide direct links to the products using HTML format: <a href='/Home/Product/{ProductId}'>{ProductName}</a>
2. Include the price in NIS (multiply USD price by 3.7)
3. Group suggestions by category when offering multiple items
4. Ask follow-up questions if you need more information about:
   - Budget (if not specified)
   - Recipient's interests
   - Occasion
   - Usage frequency
5. When mentioning prices, always show both USD and NIS (e.g., $79.99 / ₪296 NIS)

Format your product suggestions like this:
- Product Name: [link to product]
- Price: $XX.XX / ₪XX NIS
- Category: [category]
- Why it's a good fit: [brief explanation]

Available products:
" + string.Join("\n", products.Select(p => $"- {p.Name} (${p.Price:F2} / ₪{(p.Price * 3.7m):F0} NIS): {p.Description} [Category: {p.Category}] [ID: {p.Id}]"));

            var options = new ChatCompletionsOptions
            {
                Temperature = 0.7f,
                MaxTokens = 800
            };

            // Add the system message first
            options.Messages.Add(new Azure.AI.OpenAI.ChatMessage(ChatRole.System, systemMessage));

            // Add the conversation history
            foreach (var message in conversationHistory)
            {
                options.Messages.Add(new Azure.AI.OpenAI.ChatMessage(
                    message.Role == "user" ? ChatRole.User : ChatRole.Assistant,
                    message.Content));
            }

            var response = await _openAIClient.GetChatCompletionsAsync(_deploymentName, options);
            return response.Value.Choices[0].Message.Content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting response from ChatGPT");
            throw;
        }
    }
} 