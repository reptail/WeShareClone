using Microsoft.AspNetCore.Mvc;

namespace WeShareClone.Controllers;

[ApiController]
[Route("/")]
public class DefaultController : ControllerBase
{
    /// <summary>
    /// Health check endpoint.
    /// </summary>
    [HttpGet("health")]
    public string Get()
        => "Hello World";
}
