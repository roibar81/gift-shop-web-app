using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WebApp.Api.Models;
using System;
using System.Linq;

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
            var productsList = string.Join(Environment.NewLine, products.Select(p => $"- {p.Name} (${p.Price:F2} / ₪{(p.Price * 3.7m):F0} ש\"ח): {p.Description} [קטגוריה: {p.Category}] [מזהה: {p.Id}]"));
            
            var systemMessage = $@"אתה יועץ מתנות נלהב ומקצועי בחנות המתנות שלנו. המטרה שלך היא לעזור למשתמשים למצוא את המתנה המושלמת מתוך המוצרים הזמינים שלנו.

כללי התנהגות:
1. תמיד לדבר בעברית ובטון חיובי ומעודד
2. להתמקד אך ורק בנושא בחירת מתנות - אם המשתמש שואל על נושאים אחרים, להסביר בנימוס שאתה מתמחה רק בייעוץ למתנות
3. להשתמש במילים חיוביות ומוטיבציוניות כמו: 'נהדר!', 'מצוין!', 'איזה כיף!', 'אשמח לעזור!', 'בוא נמצא יחד את המתנה המושלמת!'
4. חובה להוסיף קישור HTML בכל פעם שמזכירים מוצר - גם אם זה רק כדוגמה או הצעה. תמיד להשתמש בפורמט: <a href='/Home/Product/{{ProductId}}'>{{ProductName}}</a>

בהצעת מוצרים:
1. תמיד לספק קישורים ישירים למוצרים בפורמט HTML: <a href='/Home/Product/{{ProductId}}'>{{ProductName}}</a>
2. להציג מחירים בש""ח (להכפיל מחיר USD ב-3.7)
3. לקבץ הצעות לפי קטגוריה כשמציעים מספר פריטים
4. לשאול שאלות המשך בצורה חיובית ומעודדת אם צריך מידע נוסף על:
   - תקציב (אם לא צוין) - למשל: 'איזה תקציב מקסים תרצה להשקיע במתנה המיוחדת?'
   - תחומי העניין של מקבל המתנה - למשל: 'ספר לי עוד על התחביבים המיוחדים של מקבל/ת המתנה!'
   - האירוע - למשל: 'לאיזה אירוע משמח אנחנו מחפשים מתנה?'
   - תדירות השימוש - למשל: 'האם תרצה שזו תהיה מתנה לשימוש יומיומי או משהו מיוחד לאירועים?'
5. להציג מחירים תמיד בדולר ובש""ח (למשל: $79.99 / ₪296 ש""ח)

פורמט להצעת מוצרים:
- שם המוצר: [חובה להוסיף קישור HTML]
- מחיר: $XX.XX / ₪XX ש""ח
- קטגוריה: [קטגוריה]
- למה זה מתאים: [הסבר קצר ומעודד]

המוצרים הזמינים:
{productsList}";

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

    public async Task<string> GetComplementaryProductsAsync(int productId)
    {
        try
        {
            var products = await _productService.GetProductsAsync();
            var mainProduct = products.FirstOrDefault(p => p.Id == productId);
            
            if (mainProduct == null)
            {
                throw new ArgumentException("המוצר לא נמצא");
            }

            var productsList = string.Join(Environment.NewLine, products.Select(p => 
                $"- {p.Name} (${p.Price:F2} / ₪{(p.Price * 3.7m):F0} ש\"ח): {p.Description} [קטגוריה: {p.Category.Name}] [מזהה: {p.Id}]"));

            var systemMessage = $@"אתה יועץ מתנות מקצועי שממליץ על מוצרים משלימים. המטרה שלך היא להציע 2-3 מוצרים שמשתלבים היטב עם המוצר שהלקוח בחר.

המוצר שנבחר:
- שם: {mainProduct.Name}
- מחיר: ${mainProduct.Price:F2} / ₪{(mainProduct.Price * 3.7m):F0} ש""ח
- קטגוריה: {mainProduct.Category.Name}
- תיאור: {mainProduct.Description}

כללי המלצה:
1. להציע 2-3 מוצרים משלימים שיכולים להתאים יחד עם המוצר שנבחר
2. להתמקד במוצרים שמשתלבים היטב מבחינת:
   - שימוש משותף
   - התאמה סגנונית
   - טווח מחירים דומה
3. להסביר בקצרה למה כל מוצר מתאים
4. להשתמש בפורמט HTML לקישורים: <a href='/Home/Product/{{ProductId}}'>{{ProductName}}</a>

המוצרים הזמינים:
{productsList}

אנא הצע 2-3 מוצרים משלימים למוצר שנבחר, והסבר בקצרה למה הם מתאימים.";

            var options = new ChatCompletionsOptions
            {
                Temperature = 0.7f,
                MaxTokens = 800,
                Messages = 
                {
                    new Azure.AI.OpenAI.ChatMessage(ChatRole.System, systemMessage),
                    new Azure.AI.OpenAI.ChatMessage(ChatRole.User, "אנא הצע מוצרים משלימים למוצר זה")
                }
            };

            var response = await _openAIClient.GetChatCompletionsAsync(_deploymentName, options);
            return response.Value.Choices[0].Message.Content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting complementary products suggestions");
            throw;
        }
    }
} 