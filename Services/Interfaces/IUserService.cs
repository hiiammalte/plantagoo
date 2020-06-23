using Plantagoo.DTOs.Users;
using Plantagoo.Response;
using System;
using System.Threading.Tasks;

namespace Plantagoo.Interfaces
{
    public interface IUserService
    {
        Task<ServiceResponse<UserDetailsDTO>> RegisterAsync(UserRegisterDTO user);
        Task<ServiceResponse<UserDetailsDTO>> FindAsync(Guid id);
        Task<ServiceResponse<UserDetailsDTO>> UpdateAsync(Guid id, UserDTO user);
        Task<ServiceResponse<UserDetailsDTO>> DeleteAsync(Guid id);
    }
}
