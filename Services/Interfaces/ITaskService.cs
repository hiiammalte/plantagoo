using Plantagoo.DTOs.Tasks;
using Plantagoo.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plantagoo.Interfaces
{
    public interface ITaskService
    {
        Task<ServiceResponse<TaskDetailsDTO>> CreateAsync(Guid userId, Guid projectId, TaskDTO taskDto);
        Task<ServiceResponse<TaskDetailsDTO>> FindByIdAsync(Guid userId, Guid projectId, Guid id);
        Task<ServiceResponse<TaskDetailsDTO>> UpdateAsync(Guid userId, Guid projectId, Guid id, TaskDTO taskDto);
        Task<ServiceResponse<TaskDetailsDTO>> DeleteAsync(Guid userId, Guid projectId, Guid id);
        Task<ServiceResponse<IEnumerable<TaskDetailsDTO>>> FindAllAsync(Guid userId, Guid projectId);
    }
}
