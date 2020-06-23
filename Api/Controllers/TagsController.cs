using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Plantagoo.DTOs.Tags;
using Plantagoo.Interfaces;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using static Plantagoo.Response.EServiceResponseTypes;

namespace Plantagoo.Api.Controllers
{
    public class TagsController : BaseController
    {
        private readonly ILogger _logger;
        private readonly ITagService _tagService;

        public TagsController(ILogger<TagsController> logger, ITagService tagService)
        {
            _logger = logger;
            _tagService = tagService;
        }

        [HttpPost]
        public async Task<ActionResult> CreateUserTag([FromBody] TagDTO tag)
        {
            var userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var serviceResponse = await _tagService.CreateAsync(userId, tag);
            return serviceResponse.ResponseType switch
            {
                EResponseType.Success => CreatedAtAction(nameof(FindUserTag), new { version = "1", id = serviceResponse.Data.Id }, serviceResponse.Data),
                EResponseType.CannotCreate => BadRequest(serviceResponse.Message),
                _ => throw new NotImplementedException()
            };
        }

        [HttpGet]
        public async Task<ActionResult> FindAllUserTags()
        {
            var userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var serviceResponse = await _tagService.FindAllAsync(userId);
            return serviceResponse.ResponseType switch
            {
                EResponseType.Success => Ok(serviceResponse.Data),
                EResponseType.NotFound => NotFound(),
                _ => throw new NotImplementedException()
            };
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> FindUserTag(Guid id)
        {
            var userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var serviceResponse = await _tagService.FindByIdAsync(userId, id);
            return serviceResponse.ResponseType switch
            {
                EResponseType.Success => Ok(serviceResponse.Data),
                EResponseType.NotFound => NotFound(),
                _ => throw new NotImplementedException()
            };
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUserTag(Guid id, TagDTO tag)
        {
            var userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var serviceResponse = await _tagService.UpdateAsync(userId, id, tag);
            return serviceResponse.ResponseType switch
            {
                EResponseType.Success => Ok(serviceResponse.Data),
                EResponseType.NotFound => NotFound(serviceResponse.Message),
                EResponseType.CannotUpdate => BadRequest(serviceResponse.Message),
                _ => throw new NotImplementedException()
            };
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUserTag(Guid id)
        {
            var userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var serviceResponse = await _tagService.DeleteAsync(userId, id);
            return serviceResponse.ResponseType switch
            {
                EResponseType.Success => NoContent(),
                EResponseType.NotFound => NotFound(),
                _ => throw new NotImplementedException()
            };
        }
    }
}
