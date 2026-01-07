namespace ERP.SharedKernel.DTOs;

public class ApiResponseDto<T>
{
    public int Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }

    public static ApiResponseDto<T> Success(T data, string message = "Success", int status = 200)
    {
        return new ApiResponseDto<T>
        {
            Status = status,
            Message = message,
            Data = data
        };
    }

    public static ApiResponseDto<T> Fail(List<string> errors, string message = "Failed", int status = 400)
    {
        return new ApiResponseDto<T>
        {
            Status = status,
            Message = message,
            Errors = errors
        };
    }

    public static ApiResponseDto<T> Error(string message, int status = 500)
    {
        return new ApiResponseDto<T>
        {
            Status = status,
            Message = message,
            Errors = null
        };
    }
}
