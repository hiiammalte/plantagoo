using Plantagoo.DTOs.Projects;
using Plantagoo.Filtering;
using Plantagoo.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plantagoo.Interfaces
{
    public interface IProjectService
    {
        Task<ServiceResponse<ProjectDetailsDTO>> CreateAsync(Guid userId, ProjectDTO projectDto);
        Task<ServiceResponse<ProjectDetailsDTO>> FindByIdAsync(Guid userId, Guid id);
        Task<ServiceResponse<ProjectDetailsDTO>> UpdateAsync(Guid userId, Guid id, ProjectDTO projectDto);
        Task<ServiceResponse<ProjectDetailsDTO>> DeleteAsync(Guid userId, Guid id);
        Task<ServiceResponse<IEnumerable<ProjectDetailsDTO>>> FindAllAsync(Guid userId);
        Task<ServiceResponse<PagingReturnModel<ProjectDetailsDTO>>> FilterAllAsync(Guid userId, FilterOptions filter);
    }
}
