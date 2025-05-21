using System.Diagnostics.CodeAnalysis;

namespace Jennifer.SharedKernel;

public class Result
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public Error Error { get; set; }
    public static Result Success() => new() { IsSuccess = true };
    public static Result Failure(string message) => new() { IsSuccess = false, Message = message };
    public static Result Failure(Error error) => new() { IsSuccess = false, Error = error};
}
public class Result<T>: Result
{
    public T Data { get; set; }
    
    public static Result<T> Success(T data) => new() { IsSuccess = true, Data = data };
    public static Result<T> Failure(string message) => new() { IsSuccess = false, Message = message };
    public static Result<T> Failure(Error error) => new() { IsSuccess = false, Error = error};   
}
