using API.Errors;

namespace API.Dtos
{
    public class ResponseDto<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public ApiErrorResponse Error { get; set; }
    }
}