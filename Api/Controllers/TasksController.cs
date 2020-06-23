using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Plantagoo.DTOs.Tasks;
using Plantagoo.Interfaces;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using static Plantagoo.Response.EServiceResponseTypes;

namespace Plantagoo.Api.Controllers
{
    [Route("api/v{version:apiVersion}/Projects/{projectId}[controller]")]
    public class TasksController : BaseController
    {
        private readonly ILogger _logger;
        private readonly ITaskService _taskService;

        public TasksController(ILogger<ProjectsController> logger, ITaskService taskService)
        {
            _logger = logger;
            _taskService = taskService;
        }

        [HttpPost]
        public async Task<ActionResult> CreateProjectTask([FromBody] TaskDTO task, Guid projectId)
        {
            var userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var serviceResponse = await _taskService.CreateAsync(userId, projectId, task);
            return serviceResponse.ResponseType switch
            {
                EResponseType.Success => CreatedAtAction(nameof(FindProjectTask), new { version = "1", id = serviceResponse.Data.Id }, serviceResponse.Data),
                EResponseType.CannotCreate => BadRequest(serviceResponse.Message),
                _ => throw new NotImplementedException()
            };
        }

        [HttpGet]
        public async Task<ActionResult> FindAllProjectTasks(Guid projectId)
        {
            var userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var serviceResponse = await _taskService.FindAllAsync(userId, projectId);
            return serviceResponse.ResponseType switch
            {
                EResponseType.Success => Ok(serviceResponse.Data),
                EResponseType.NotFound => NotFound(),
                _ => throw new NotImplementedException()
            };
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> FindProjectTask(Guid projectId, Guid id)
        {
            var userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var serviceResponse = await _taskService.FindByIdAsync(userId, projectId, id);
            return serviceResponse.ResponseType switch
            {
                EResponseType.Success => Ok(serviceResponse.Data),
                EResponseType.NotFound => NotFound(),
                _ => throw new NotImplementedException()
            };
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUserTag(Guid projectId, Guid id, TaskDTO task)
        {
            var userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var serviceResponse = await _taskService.UpdateAsync(userId, projectId, id, task);
            return serviceResponse.ResponseType switch
            {
                EResponseType.Success => Ok(serviceResponse.Data),
                EResponseType.NotFound => NotFound(serviceResponse.Message),
                EResponseType.CannotUpdate => BadRequest(serviceResponse.Message),
                _ => throw new NotImplementedException()
            };
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProjectTask(Guid projectId, Guid id)
        {
            var userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var serviceResponse = await _taskService.DeleteAsync(userId, projectId, id);
            return serviceResponse.ResponseType switch
            {
                EResponseType.Success => NoContent(),
                EResponseType.NotFound => NotFound(),
                _ => throw new NotImplementedException()
            };
        }
    }
}
