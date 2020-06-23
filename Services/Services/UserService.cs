using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Plantagoo.Data;
using Plantagoo.DTOs.Users;
using Plantagoo.Entities;
using Plantagoo.Interfaces;
using Plantagoo.Response;
using System;
using System.Threading.Tasks;
using static Plantagoo.Response.EServiceResponseTypes;

namespace Plantagoo.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger _logger;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public UserService(ILogger<UserService> logger, AppDbContext context, IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ServiceResponse<UserDetailsDTO>> RegisterAsync(UserRegisterDTO userDto)
        {
            var serviceResponse = new ServiceResponse<UserDetailsDTO>();
            try
            {
                var toAdd = _mapper.Map<UserModel>(userDto);
                await _context.Users.AddAsync(toAdd);
                await _context.SaveChangesAsync();
                serviceResponse.Data = _mapper.Map<UserDetailsDTO>(toAdd);
            }
            catch (DbUpdateConcurrencyException)
            {
                serviceResponse.ResponseType = EResponseType.CannotCreate;
                serviceResponse.Message = "Email already in use by another User. Please login or choose different.";
            }
            catch { throw; }
            return serviceResponse;
        }

        public async Task<ServiceResponse<UserDetailsDTO>> FindAsync(Guid id)
        {
            var serviceResponse = new ServiceResponse<UserDetailsDTO>();
            try
            {
                var user = await _mapper.ProjectTo<UserDetailsDTO>(_context.Users)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (user == null)
                {
                    serviceResponse.ResponseType = EResponseType.NotFound;
                }
                else
                {
                    serviceResponse.Data = user;
                }
            }
            catch { throw; }
            return serviceResponse;
        }

        public async Task<ServiceResponse<UserDetailsDTO>> UpdateAsync(Guid id, UserDTO userDto)
        {
            var serviceResponse = new ServiceResponse<UserDetailsDTO>();
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (user == null)
                {
                    serviceResponse.ResponseType = EResponseType.NotFound;
                }
                else
                {
                    user = _mapper.Map(userDto, user);
                    await _context.SaveChangesAsync();
                    serviceResponse.Data = _mapper.Map<UserDetailsDTO>(user);
                }
            }
            catch (DbUpdateException)
            {
                serviceResponse.ResponseType = EResponseType.CannotUpdate;
                serviceResponse.Message = "Email already in use by another User. Please choose different.";
            }
            catch { throw; }
            return serviceResponse;
        }

        public async Task<ServiceResponse<UserDetailsDTO>> DeleteAsync(Guid id)
        {
            var serviceResponse = new ServiceResponse<UserDetailsDTO>();
            try
            {
                _context.Users.Remove(new UserModel { Id = id });
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                serviceResponse.ResponseType = EResponseType.NotFound;
            }
            catch { throw; }
            return serviceResponse;
        }
    }
}
