namespace ERP.SharedKernel.Exceptions;

public class AppException : Exception
{
    public int StatusCode { get; set; }
    public List<string>? Errors { get; set; }

    public AppException(string message, int statusCode = 500, List<string>? errors = null)
        : base(message)
    {
        StatusCode = statusCode;
        Errors = errors;
    }
}

public class ValidationException : AppException
{
    public ValidationException(string message, List<string>? errors = null)
        : base(message, 400, errors)
    {
    }
}

public class NotFoundException : AppException
{
    public NotFoundException(string message)
        : base(message, 404)
    {
    }
}

public class UnauthorizedException : AppException
{
    public UnauthorizedException(string message)
        : base(message, 401)
    {
    }
}

public class ConflictException : AppException
{
    public ConflictException(string message)
        : base(message, 409)
    {
    }
}
