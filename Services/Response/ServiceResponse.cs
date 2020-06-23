using static Plantagoo.Response.EServiceResponseTypes;

namespace Plantagoo.Response
{
    public class ServiceResponse<T>
    {
        public T Data { get; set; }
        public EResponseType ResponseType { get; set; }
        public string Message { get; set; } = null;
    }
}
