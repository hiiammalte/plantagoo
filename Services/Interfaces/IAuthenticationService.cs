using Plantagoo.DTOs.Authentication;
using System;
using System.Threading.Tasks;

namespace Plantagoo.Interfaces
{
    public interface IAuthService
    {
        Task<(string token, DateTime expiration)> CreateJWT(CredentialsDTO credentials);
    }
}
