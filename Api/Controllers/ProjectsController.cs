using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Plantagoo.DTOs.Projects;
using Plantagoo.Interfaces;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using static Plantagoo.Response.EServiceResponseTypes;

namespace Plantagoo.Api.Controllers
{
    public class ProjectsController : BaseController
    {
        private readonly ILogger _logger;
        private readonly IProjectService _projectService;

        public ProjectsController(ILogger<ProjectsController> logger, IProjectService projectService)
        {
            _logger = logger;
            _projectService = projectService;
        }

        [HttpPost]
        public async Task<ActionResult> CreateUserProject([FromBody] ProjectDTO project)
        {
            var userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var serviceResponse = await _projectService.CreateAsync(userId, project);
            return serviceResponse.ResponseType switch
            {
                EResponseType.Success => CreatedAtAction(nameof(FindUserProject), new { version = "1", id = serviceResponse.Data.Id }, serviceResponse.Data),
                EResponseType.CannotCreate => BadRequest(serviceResponse.Message),
                _ => throw new NotImplementedException()
            };
        }

        [HttpGet]
        public async Task<ActionResult> FindUserProjects()
        {
            var userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new NotImplementedException());
            var serviceResponse = await _projectService.FindAllAsync(userId);
            switch (serviceResponse.ResponseType)
            {
                case EResponseType.Success:
                    return Ok(serviceResponse.Data);
                case EResponseType.NotFound:
                    return NotFound();
                default:
                    throw new NotImplementedException();
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> FindUserProject(Guid id)
        {
            var userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var serviceResponse = await _projectService.FindByIdAsync(userId, id);
            return serviceResponse.ResponseType switch
            {
                EResponseType.Success => Ok(serviceResponse.Data),
                EResponseType.NotFound => NotFound(),
                _ => throw new NotImplementedException()
            };
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUserProject(Guid id, ProjectDTO project)
        {
            var userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var serviceResponse = await _projectService.UpdateAsync(userId, id, project);
            return serviceResponse.ResponseType switch
            {
                EResponseType.Success => Ok(serviceResponse.Data),
                EResponseType.NotFound => NotFound(serviceResponse.Message),
                EResponseType.CannotUpdate => BadRequest(serviceResponse.Message),
                _ => throw new NotImplementedException()
            };
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUserProject(Guid id)
        {
            var userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var serviceResponse = await _projectService.DeleteAsync(userId, id);
            return serviceResponse.ResponseType switch
            {
                EResponseType.Success => NoContent(),
                EResponseType.NotFound => NotFound(),
                _ => throw new NotImplementedException()
            };
        }
    }
}
