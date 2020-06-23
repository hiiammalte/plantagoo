namespace Plantagoo.Authentication
{
    public sealed class TokenSettings
    {
        public string Secret { get; set; }
        public int AccessExpirationInMinutes { get; set; }
    }
}
