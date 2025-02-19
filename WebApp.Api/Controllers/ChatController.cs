using Microsoft.AspNetCore.Mvc;
using WebApp.Api.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using WebApp.Api.Models;

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

    [HttpPost("conversation")]
    public async Task<IActionResult> HandleConversation([FromBody] ConversationRequest request)
    {
        try
        {
            _logger.LogInformation("Handling conversation request");
            var response = await _chatService.GetResponseAsync(request.Messages);
            return Ok(new { response });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling conversation");
            return StatusCode(500, "An error occurred while processing your request");
        }
    }
} 