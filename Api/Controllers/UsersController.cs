using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Plantagoo.DTOs.Users;
using Plantagoo.Interfaces;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using static Plantagoo.Response.EServiceResponseTypes;

namespace Plantagoo.Api.Controllers
{
    public class UsersController : BaseController
    {
        private readonly ILogger _logger;
        private readonly IUserService _userService;

        public UsersController(ILogger<UsersController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Register([FromBody] UserRegisterDTO userdata)
        {
            var serviceResponse = await _userService.RegisterAsync(userdata);
            return serviceResponse.ResponseType switch
            {
                EResponseType.Success => CreatedAtAction(nameof(Profile), new { version = "1" }, serviceResponse.Data),
                EResponseType.CannotCreate => BadRequest(serviceResponse.Message),
                _ => throw new NotImplementedException()
            };
        }

        [HttpGet]
        public async Task<ActionResult> Profile()
        {
            var userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var serviceResponse = await _userService.FindAsync(userId);
            return serviceResponse.ResponseType switch
            {
                EResponseType.Success => Ok(serviceResponse.Data),
                EResponseType.NotFound => NotFound(),
                _ => throw new NotImplementedException()
            };
        }

        [HttpPut]
        public async Task<ActionResult> UpdateProfile(UserDTO user)
        {
            var userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var serviceResponse = await _userService.UpdateAsync(userId, user);
            return serviceResponse.ResponseType switch
            {
                EResponseType.Success => Ok(serviceResponse.Data),
                EResponseType.NotFound => NotFound(),
                EResponseType.CannotUpdate => BadRequest(serviceResponse.Message),
                _ => throw new NotImplementedException()
            };
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteProfile()
        {
            var userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var serviceResponse = await _userService.DeleteAsync(userId);
            return serviceResponse.ResponseType switch
            {
                EResponseType.Success => NoContent(),
                EResponseType.NotFound => NotFound(),
                _ => throw new NotImplementedException()
            };
        }
    }
}
