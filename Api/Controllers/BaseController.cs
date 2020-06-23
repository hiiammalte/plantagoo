using Microsoft.AspNetCore.Mvc;

namespace Plantagoo.Api.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    { }
}
