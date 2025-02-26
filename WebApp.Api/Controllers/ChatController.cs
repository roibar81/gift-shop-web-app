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
    public async Task<ActionResult<ChatResponse>> PostConversation([FromBody] ConversationRequest request)
    {
        try
        {
            var response = await _chatService.GetResponseAsync(request.Messages);
            return Ok(new ChatResponse { Response = response });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing chat conversation");
            return StatusCode(500, "Error processing chat conversation");
        }
    }

    [HttpGet("complementary-products/{productId}")]
    public async Task<ActionResult<ChatResponse>> GetComplementaryProducts(int productId)
    {
        try
        {
            var response = await _chatService.GetComplementaryProductsAsync(productId);
            return Ok(new ChatResponse { Response = response });
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting complementary products suggestions");
            return StatusCode(500, "Error getting complementary products suggestions");
        }
    }
}

public class ChatResponse
{
    public string Response { get; set; } = string.Empty;
} 