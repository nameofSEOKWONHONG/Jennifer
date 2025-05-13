namespace Jennifer.Tenant.Domains;

public class ApiResponse<T>
{
    public T Data { get; set; }
    public string Message { get; set; }
    public bool IsSuccess { get; set; }
    
    public dynamic Errors { get; set; }

    public static ApiResponse<T> Success(T data, string message = "")
    {
        return new ApiResponse<T>
        {
            Data = data,
            Message = message,
            IsSuccess = true
        };   
    }

    public static ApiResponse<T> Fail(string message, dynamic errors = null)
    {
        return new ApiResponse<T>
        {
            Message = message,
            IsSuccess = false,
            Errors = errors
        };   
    }
}