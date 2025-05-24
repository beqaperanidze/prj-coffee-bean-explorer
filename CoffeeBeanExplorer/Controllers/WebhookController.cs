using Microsoft.AspNetCore.Mvc;

namespace CoffeeBeanExplorer.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/webhook")]
public class WebhookController : ControllerBase
{
    [HttpPost]
    public IActionResult ReceiveWebhook([FromBody] object payload)
    {
        return Ok(new { status = "received" });
    }
}
