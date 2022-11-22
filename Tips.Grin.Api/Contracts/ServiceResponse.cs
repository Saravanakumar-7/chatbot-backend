using System.Net;

namespace Tips.Grin.Api.Contracts
{
    public class ServiceResponse<T>
    {
        public T? Data { get; set; }
        public string? Message { get; set; }
        public bool Success { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
