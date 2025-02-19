namespace WebApp.Api.Models;

public class ConversationRequest
{
    public List<ChatMessage> Messages { get; set; } = new();
} 