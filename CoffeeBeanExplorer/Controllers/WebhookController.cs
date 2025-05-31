using Microsoft.AspNetCore.Mvc;

namespace CoffeeBeanExplorer.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/webhooks")]
public class WebhookController : ControllerBase
{
    /// <summary>
    ///     Receives webhook payloads from external services
    /// </summary>
    /// <param name="payload">The webhook payload data</param>
    /// <returns>Confirmation of receipt</returns>
    [HttpPost]
    public IActionResult ReceiveWebhook([FromBody] object payload)
    {
        return Ok(new { status = "received" });
    }
}
