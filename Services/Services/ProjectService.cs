using AutoMapper;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Plantagoo.Data;
using Plantagoo.DTOs.Projects;
using Plantagoo.Entities;
using Plantagoo.Filtering;
using Plantagoo.Interfaces;
using Plantagoo.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Plantagoo.Response.EServiceResponseTypes;

namespace Plantagoo.Services
{
    public class ProjectService : IProjectService
    {
        private readonly ILogger _logger;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IFilterHelper<ProjectDetailsDTO> _filterHelper;

        public ProjectService(ILogger<ProjectService> logger, AppDbContext context, IMapper mapper, IFilterHelper<ProjectDetailsDTO> filterHelper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _filterHelper = filterHelper ?? throw new ArgumentNullException(nameof(filterHelper));
        }

        private IQueryable<ProjectModel> GetUserProjects(Guid userId)
        {
            return _context.Projects
                .Include(c => c.Tasks)
                .Where(c => c.Owner.Id == userId);
        }

        public async Task<ServiceResponse<ProjectDetailsDTO>> CreateAsync(Guid userId, ProjectDTO projectDto)
        {
            var serviceResponse = new ServiceResponse<ProjectDetailsDTO>();
            try
            {
                var user = await _context.Users
                    .Include(c => c.Projects)
                    .FirstAsync(c => c.Id == userId);

                var project = _mapper.Map<ProjectModel>(projectDto);
                if (user.Projects == null)
                    user.Projects = new List<ProjectModel>();

                user.Projects.Add(project);
                await _context.SaveChangesAsync();

                serviceResponse.Data = _mapper.Map<ProjectDetailsDTO>(project);
            }
            catch (InvalidOperationException)
            {
                serviceResponse.ResponseType = EResponseType.NotFound;
                serviceResponse.Message = "Owner (User) of product not found.";
            }
            catch { throw; }
            return serviceResponse;
        }

        public async Task<ServiceResponse<ProjectDetailsDTO>> FindByIdAsync(Guid userId, Guid id)
        {
            var serviceResponse = new ServiceResponse<ProjectDetailsDTO>();
            try
            {
                serviceResponse.Data = await _mapper.ProjectTo<ProjectDetailsDTO>(GetUserProjects(userId))
                    .AsNoTracking()
                    .FirstAsync(c => c.Id == id);
            }
            catch (InvalidOperationException)
            {
                serviceResponse.ResponseType = EResponseType.NotFound;
                serviceResponse.Message = "Project and/or Owner (User) not found.";
            }
            catch { throw; }
            return serviceResponse;
        }



        public async Task<ServiceResponse<ProjectDetailsDTO>> UpdateAsync(Guid userId, Guid id, ProjectDTO projectDto)
        {
            var serviceResponse = new ServiceResponse<ProjectDetailsDTO>();
            try
            {
                var project = await GetUserProjects(userId)
                    .FirstAsync(c => c.Id == id);

                project = _mapper.Map(projectDto, project);

                await _context.SaveChangesAsync();
                serviceResponse.Data = _mapper.Map<ProjectDetailsDTO>(project);
            }
            catch (InvalidOperationException)
            {
                serviceResponse.ResponseType = EResponseType.NotFound;
                serviceResponse.Message = "Project and/or Owner (User) not found.";
            }
            catch { throw; }
            return serviceResponse;
        }

        public async Task<ServiceResponse<ProjectDetailsDTO>> DeleteAsync(Guid userId, Guid id)
        {
            var serviceResponse = new ServiceResponse<ProjectDetailsDTO>();
            try
            {
                _context.Projects.Remove(new ProjectModel { Id = id, Owner = new UserModel { Id = userId } });
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                serviceResponse.ResponseType = EResponseType.NotFound;
            }
            catch { throw; }
            return serviceResponse;
        }

        public async Task<ServiceResponse<IEnumerable<ProjectDetailsDTO>>> FindAllAsync(Guid userId)
        {
            var serviceResponse = new ServiceResponse<IEnumerable<ProjectDetailsDTO>>();
            try
            {
                var projects = await _mapper.ProjectTo<ProjectDetailsDTO>(GetUserProjects(userId))
                    .AsNoTracking()
                    .ToListAsync();

                if (projects.Any() == true)
                {
                    serviceResponse.Data = projects;
                }
                else
                {
                    serviceResponse.ResponseType = EResponseType.NotFound;
                    serviceResponse.Message = "Projects not found.";
                }
            }
            catch (InvalidOperationException)
            {
                serviceResponse.ResponseType = EResponseType.NotFound;
                serviceResponse.Message = "Owner (User) not found.";
            }
            catch { throw; }
            return serviceResponse;
        }

        public async Task<ServiceResponse<PagingReturnModel<ProjectDetailsDTO>>> FilterAllAsync(Guid userId, FilterOptions filterParameters)
        {
            var predicate = PredicateBuilder.New<ProjectDetailsDTO>();
            predicate = predicate.Or(p => p.Name.Contains(filterParameters.SearchTerm));
            predicate = predicate.Or(p => p.Description.Contains(filterParameters.SearchTerm));

            var serviceResponse = new ServiceResponse<PagingReturnModel<ProjectDetailsDTO>>();
            try
            {
                var projects = _mapper.ProjectTo<ProjectDetailsDTO>(GetUserProjects(userId))
                        .Where(predicate)
                        .AsNoTracking();

                var sortedProjects = _filterHelper.ApplySorting(projects, filterParameters.OrderBy);
                var pagedProjects = await _filterHelper.ApplyPaging(sortedProjects, filterParameters.Page, filterParameters.Limit);

                if (pagedProjects?.Items?.Any() == true)
                {
                    serviceResponse.Data = pagedProjects;
                }
                else
                {
                    serviceResponse.ResponseType = EResponseType.NotFound;
                    serviceResponse.Message = "Project and/or Owner (User) not found.";
                }
            }
            catch { throw; }
            return serviceResponse;
        }

    }
}
