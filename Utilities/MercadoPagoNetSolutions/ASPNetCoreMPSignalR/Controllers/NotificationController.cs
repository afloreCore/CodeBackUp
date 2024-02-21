using ASPNetCoreMPSignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace IASPNetCoreMPSignalR.Controllers;

[ApiController]
[Route("api/notification")]
public class NotificationController : ControllerBase
{
    protected ChatHub _chatHub;
    protected string? topic = string.Empty;
    protected string? operationID = string.Empty;

    public NotificationController(IHubContext<ChatHub> hubContext)
    {
        _chatHub = new ChatHub(hubContext);
    }

    [HttpPost]
    public async Task<IActionResult> PostNotification()
    {
        try
        
        {
            topic = HttpContext.Request.Query["topic"];
            operationID = HttpContext.Request.Query["id"];
            string? client = HttpContext.Request.Query["clientId"];
            var result = await _chatHub.SendMessage(client, topic, operationID);

            return result ? StatusCode(200) : StatusCode(405);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

        

    }
    [HttpGet]
    public IActionResult Send()
    {
       // var result = await _chatHub.SendMessage(topic, operationID);
       
        return StatusCode(200, "Test ok");

    }
}
