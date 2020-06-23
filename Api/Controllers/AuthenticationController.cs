using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Plantagoo.DTOs.Authentication;
using Plantagoo.Interfaces;
using System;
using System.Threading.Tasks;

namespace Plantagoo.Api.Controllers
{
    [AllowAnonymous]
    public class AuthenticationController : BaseController
    {
        private readonly ILogger _logger;
        private readonly IAuthService _authService;

        public AuthenticationController(ILogger<UsersController> logger, IAuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        [HttpPost]
        public async Task<IActionResult> GetToken(CredentialsDTO credentials)
        {
            (string jwt, DateTime expiration) = await _authService.CreateJWT(credentials);
            if (string.IsNullOrWhiteSpace(jwt))
                return BadRequest(new { message = "Username or Password is incorrect" });

            return Ok(new { jwt, expiration });
        }
    }
}
