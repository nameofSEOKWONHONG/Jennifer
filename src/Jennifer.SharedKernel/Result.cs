namespace Jennifer.SharedKernel;

public interface IResult
{
    bool IsSuccess { get; set; }
    string Message { get; set; }
    Error Error { get; set; }
}

public class Result : IResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public Error Error { get; set; }
    
    public static Result Success() => new() { IsSuccess = true };
    public static Result Failure(string message) => new() { IsSuccess = false, Message = message };
    public static Result Failure(Error error) => new() { IsSuccess = false, Error = error};
}
public class Result<T>: Result, IResult
{
    public T Data { get; set; }
    
    public static Result<T> Success(T data) => new() { IsSuccess = true, Data = data };
    public static Result<T> Failure(string message) => new() { IsSuccess = false, Message = message };
    public static Result<T> Failure(Error error) => new() { IsSuccess = false, Error = error};   
}
