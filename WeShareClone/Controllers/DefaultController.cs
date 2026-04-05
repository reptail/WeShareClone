using Microsoft.AspNetCore.Mvc;

namespace WeShareClone.Controllers;

[ApiController]
[Route("/")]
public class DefaultController : ControllerBase
{
    /// <summary>
    /// Health check endpoint.
    /// </summary>
    [HttpGet]
    public string Get()
        => "Hello World";
}
