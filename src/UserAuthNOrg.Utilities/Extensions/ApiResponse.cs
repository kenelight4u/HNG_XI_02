using UserAuthNOrg.Utilities.Enums;

namespace UserAuthNOrg.Utilities.Extensions
{
    public record ApiResponse
    {
        public string Status { get; set; }

        public string Message { get; set; }

        public StatusCode StatusCode { get; set; }

        public List<Error> Errors { get; set; }
    }

    public record ApiResponse<T> : ApiResponse
    {
        public ApiResponse(T data = default, string message = "", StatusCode statusCode = StatusCode.OK)
        {
            StatusCode = statusCode;
            Status = statusCode.GetDescription();
            Message = message;
            Data = data; 
        }

        public ApiResponse(string message = "", StatusCode statusCode = StatusCode.OK)
        {
            StatusCode = statusCode;
            Status = statusCode.GetDescription();
            Message = message;
        }

        public ApiResponse(List<Error> errors)
        {
            Errors = errors.ToList();
        }

        public T Data { get; set; }
    }

    public record Error
    {
        public string Field { get; set; }

        public string Message { get; set; }
    }
}
