using Microsoft.AspNetCore.Mvc;
using WebApp.Api.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

namespace WebApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly ChatService _chatService;
    private readonly ILogger<ChatController> _logger;

    public ChatController(ChatService chatService, ILogger<ChatController> logger)
    {
        _chatService = chatService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> GetChatResponse([FromBody] string message)
    {
        try
        {
            if (string.IsNullOrEmpty(message))
            {
                _logger.LogWarning("Received empty message");
                Debug.WriteLine("WARNING: Received empty message");
                Console.WriteLine("\n[WARNING] Received empty message\n");
                return BadRequest("Message cannot be empty");
            }

            var separator = new string('=', 50);
            Console.WriteLine($"\n{separator}");
            Console.WriteLine($"Incoming Chat Message at {DateTime.Now:HH:mm:ss}");
            Console.WriteLine(separator);
            Console.WriteLine($"Message: {message}");
            Console.WriteLine(separator);
            
            Debug.WriteLine($"\n[CHAT] New Message Received at {DateTime.Now:HH:mm:ss}");
            Debug.WriteLine($"[CHAT] Message Content: {message}\n");
            
            _logger.LogInformation("Received chat message: {Message}", message);
            
            var response = await _chatService.GetChatResponseAsync(message);
            
            Console.WriteLine($"\nResponse:");
            Console.WriteLine(separator);
            Console.WriteLine(response);
            Console.WriteLine($"{separator}\n");
            
            _logger.LogInformation("Chat response: {Response}", response);
            Debug.WriteLine($"[CHAT] Response Sent: {response}\n");
            
            return Ok(new { message = response });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing chat message: {Message}", message);
            Debug.WriteLine($"[ERROR] Chat processing failed: {ex.Message}");
            Console.WriteLine($"\n[ERROR] Failed to process message: {ex.Message}\n");
            return StatusCode(500, new { message = "An error occurred while processing your request." });
        }
    }
} 