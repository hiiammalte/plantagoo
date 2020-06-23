using System;

namespace Plantagoo.Authentication
{
    public interface ITokenHelper
    {
        (string token, DateTime expiration) GenerateJWT(Guid userId, string userEmail);
    }
}
