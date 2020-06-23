using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Security.Cryptography;

namespace Plantagoo.Encryption
{
    public class PBKDF2Hasher : IPasswordHasher
    {
        private const int SaltSize = 16; // 128 bit 
        private const int KeySize = 32; // 256 bit

        private HashingOptions Options { get; }

        public PBKDF2Hasher(IOptions<HashingOptions> options)
        {
            Options = options.Value;
        }

        public string Hash(string password)
        {
            using var algorithm = new Rfc2898DeriveBytes(password, SaltSize, Options.Iterations, HashAlgorithmName.SHA256);
            var key = Convert.ToBase64String(algorithm.GetBytes(KeySize));
            var salt = Convert.ToBase64String(algorithm.Salt);

            return $"{Options.Iterations}.{salt}.{key}";
        }

        public (bool verified, bool needsUpgrade) Check(string hash, string password)
        {
            var parts = hash.Split('.', 3);

            if (parts.Length != 3)
            {
                throw new FormatException("Unexpected hash format. " +
                  "Should be formatted as `{iterations}.{salt}.{hash}`");
            }

            var iterations = Convert.ToInt32(parts[0]);
            var salt = Convert.FromBase64String(parts[1]);
            var key = Convert.FromBase64String(parts[2]);

            var needsUpgrade = iterations != Options.Iterations;

            using var algorithm = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            var keyToCheck = algorithm.GetBytes(KeySize);
            var verified = keyToCheck.SequenceEqual(key);

            return (verified, needsUpgrade);
        }
    }
}
