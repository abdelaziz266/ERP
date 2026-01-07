namespace ERP.SharedKernel.DTOs;

public class PaginatedResponseDto<T>
{
    public int Status { get; set; }
    public string Message { get; set; }
    public List<T> Data { get; set; }
    public PaginationMetaDto Pagination { get; set; }
    public List<string> Errors { get; set; }

    public static PaginatedResponseDto<T> Success(
        List<T> data,
        int pageNumber,
        int pageSize,
        int totalCount,
        string message = "Success",
        int status = 200)
    {
        return new PaginatedResponseDto<T>
        {
            Status = status,
            Message = message,
            Data = data,
            Pagination = new PaginationMetaDto
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            },
            Errors = null
        };
    }

    public static PaginatedResponseDto<T> Error(
        string message,
        int status = 500,
        List<string> errors = null)
    {
        return new PaginatedResponseDto<T>
        {
            Status = status,
            Message = message,
            Data = null,
            Pagination = null,
            Errors = errors
        };
    }
}

public class PaginationMetaDto
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}
