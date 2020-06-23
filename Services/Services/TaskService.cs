using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Plantagoo.Data;
using Plantagoo.DTOs.Tasks;
using Plantagoo.Entities;
using Plantagoo.Interfaces;
using Plantagoo.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Plantagoo.Response.EServiceResponseTypes;

namespace Plantagoo.Services
{
    public class TaskService : ITaskService
    {
        private readonly ILogger _logger;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public TaskService(ILogger<ProjectService> logger, AppDbContext context, IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        private IQueryable<TaskModel> GetProjectTasks(Guid userId, Guid projectId)
        {
            return _context.Tasks
                .Include(c => c.Project)
                .ThenInclude(c => c.Owner)
                .Where(c => c.Project.Id == projectId && c.Project.Owner.Id == userId);
        }

        public async Task<ServiceResponse<TaskDetailsDTO>> CreateAsync(Guid userId, Guid projectId, TaskDTO taskDto)
        {
            var serviceResponse = new ServiceResponse<TaskDetailsDTO>();
            try
            {
                var project = await _context.Projects
                    .Include(c => c.Owner)
                    .Include(c => c.Tasks)
                    .FirstAsync(c => c.Id == projectId && c.Owner.Id == userId);

                var task = _mapper.Map<TaskModel>(taskDto);
                if (project.Tasks == null)
                    project.Tasks = new List<TaskModel>();

                project.Tasks.Add(task);
                await _context.SaveChangesAsync();

                serviceResponse.Data = _mapper.Map<TaskDetailsDTO>(task);
            }
            catch (InvalidOperationException)
            {
                serviceResponse.ResponseType = EResponseType.NotFound;
                serviceResponse.Message = "Project of task not found.";
            }
            catch { throw; }
            return serviceResponse;
        }

        public async Task<ServiceResponse<TaskDetailsDTO>> DeleteAsync(Guid userId, Guid projectId, Guid id)
        {
            var serviceResponse = new ServiceResponse<TaskDetailsDTO>();
            try
            {
                _context.Tasks.Remove(new TaskModel { Id = id, Project = new ProjectModel { Id = projectId, Owner = new UserModel { Id = userId } } });
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                serviceResponse.ResponseType = EResponseType.NotFound;
            }
            catch { throw; }
            return serviceResponse;
        }

        public async Task<ServiceResponse<IEnumerable<TaskDetailsDTO>>> FindAllAsync(Guid userId, Guid projectId)
        {
            var serviceResponse = new ServiceResponse<IEnumerable<TaskDetailsDTO>>();
            try
            {
                var tags = await _mapper.ProjectTo<TaskDetailsDTO>(GetProjectTasks(userId, projectId))
                    .AsNoTracking()
                    .ToListAsync();

                if (tags.Any() == true)
                {
                    serviceResponse.Data = tags;
                }
                else
                {
                    serviceResponse.ResponseType = EResponseType.NotFound;
                    serviceResponse.Message = "Tasks not found.";
                }
            }
            catch (InvalidOperationException)
            {
                serviceResponse.ResponseType = EResponseType.NotFound;
                serviceResponse.Message = "Project and/or Owner (User) not found.";
            }
            catch { throw; }
            return serviceResponse;
        }

        public async Task<ServiceResponse<TaskDetailsDTO>> FindByIdAsync(Guid userId, Guid projectId, Guid id)
        {
            var serviceResponse = new ServiceResponse<TaskDetailsDTO>();
            try
            {
                serviceResponse.Data = await _mapper.ProjectTo<TaskDetailsDTO>(GetProjectTasks(userId, projectId))
                    .AsNoTracking()
                    .FirstAsync(c => c.Id == id);
            }
            catch (InvalidOperationException)
            {
                serviceResponse.ResponseType = EResponseType.NotFound;
                serviceResponse.Message = "Task, Project and/or Owner (User) not found.";
            }
            catch { throw; }
            return serviceResponse;
        }

        public async Task<ServiceResponse<TaskDetailsDTO>> UpdateAsync(Guid userId, Guid projectId, Guid id, TaskDTO taskDto)
        {
            var serviceResponse = new ServiceResponse<TaskDetailsDTO>();
            try
            {
                var task = await GetProjectTasks(userId, projectId)
                    .FirstAsync(c => c.Id == id);

                task = _mapper.Map(taskDto, task);

                await _context.SaveChangesAsync();
                serviceResponse.Data = _mapper.Map<TaskDetailsDTO>(task);
            }
            catch (InvalidOperationException)
            {
                serviceResponse.ResponseType = EResponseType.NotFound;
                serviceResponse.Message = "Task, Project and/or Owner (User) not found.";
            }
            catch { throw; }
            return serviceResponse;
        }
    }
}
