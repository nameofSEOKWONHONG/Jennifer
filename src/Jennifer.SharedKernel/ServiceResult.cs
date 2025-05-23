namespace Jennifer.SharedKernel;

public class ServiceResult<T>
{
    public T Data { get; set; }
    public string Message { get; set; }
    public bool IsSuccess { get; set; }
    
    public static ServiceResult<T> Success(T data) => new ServiceResult<T>
    {
        Data = data,
        IsSuccess = true
    };
    public static ServiceResult<T> Failure(string message) => new ServiceResult<T>
    {
        Message = message,
        IsSuccess = false
    };
}