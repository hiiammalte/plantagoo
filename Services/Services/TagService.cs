using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Plantagoo.Data;
using Plantagoo.DTOs.Tags;
using Plantagoo.Entities;
using Plantagoo.Interfaces;
using Plantagoo.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Plantagoo.Response.EServiceResponseTypes;

namespace Plantagoo.Services
{
    public class TagService : ITagService
    {
        private readonly ILogger _logger;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public TagService(ILogger<ProjectService> logger, AppDbContext context, IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        private IQueryable<TagModel> GetUserTags(Guid userId)
        {
            return _context.Tags
                .Where(c => c.Owner.Id == userId);
        }

        public async Task<ServiceResponse<TagDetailsDTO>> CreateAsync(Guid userId, TagDTO tagDto)
        {
            var serviceResponse = new ServiceResponse<TagDetailsDTO>();
            try
            {
                var user = await _context.Users
                    .Include(c => c.Tags)
                    .FirstAsync(c => c.Id == userId);

                var tag = _mapper.Map<TagModel>(tagDto);
                if (user.Tags == null)
                    user.Tags = new List<TagModel>();

                user.Tags.Add(tag);
                await _context.SaveChangesAsync();

                serviceResponse.Data = _mapper.Map<TagDetailsDTO>(tag);
            }
            catch (InvalidOperationException)
            {
                serviceResponse.ResponseType = EResponseType.NotFound;
                serviceResponse.Message = "Owner (User) of tag not found.";
            }
            catch { throw; }
            return serviceResponse;
        }

        public async Task<ServiceResponse<TagDetailsDTO>> DeleteAsync(Guid userId, Guid id)
        {
            var serviceResponse = new ServiceResponse<TagDetailsDTO>();
            try
            {
                _context.Tags.Remove(new TagModel { Id = id, Owner = new UserModel { Id = userId } });
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                serviceResponse.ResponseType = EResponseType.NotFound;
            }
            catch { throw; }
            return serviceResponse;
        }

        public async Task<ServiceResponse<IEnumerable<TagDetailsDTO>>> FindAllAsync(Guid userId)
        {
            var serviceResponse = new ServiceResponse<IEnumerable<TagDetailsDTO>>();
            try
            {
                var tags = await _mapper.ProjectTo<TagDetailsDTO>(GetUserTags(userId))
                    .AsNoTracking()
                    .ToListAsync();

                if (tags.Any() == true)
                {
                    serviceResponse.Data = tags;
                }
                else
                {
                    serviceResponse.ResponseType = EResponseType.NotFound;
                    serviceResponse.Message = "Tags not found.";
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

        public async Task<ServiceResponse<TagDetailsDTO>> FindByIdAsync(Guid userId, Guid id)
        {
            var serviceResponse = new ServiceResponse<TagDetailsDTO>();
            try
            {
                serviceResponse.Data = await _mapper.ProjectTo<TagDetailsDTO>(GetUserTags(userId))
                    .AsNoTracking()
                    .FirstAsync(c => c.Id == id);
            }
            catch (InvalidOperationException)
            {
                serviceResponse.ResponseType = EResponseType.NotFound;
                serviceResponse.Message = "Tag and/or Owner (User) not found.";
            }
            catch { throw; }
            return serviceResponse;
        }

        public async Task<ServiceResponse<TagDetailsDTO>> UpdateAsync(Guid userId, Guid id, TagDTO tagDto)
        {
            var serviceResponse = new ServiceResponse<TagDetailsDTO>();
            try
            {
                var tag = await GetUserTags(userId)
                    .Include(c => c.ProjectTags)
                    .FirstAsync(c => c.Id == id);

                tag = _mapper.Map(tagDto, tag);

                await _context.SaveChangesAsync();
                serviceResponse.Data = _mapper.Map<TagDetailsDTO>(tag);
            }
            catch (InvalidOperationException)
            {
                serviceResponse.ResponseType = EResponseType.NotFound;
                serviceResponse.Message = "Tag and/or Owner (User) not found.";
            }
            catch { throw; }
            return serviceResponse;
        }
    }
}
