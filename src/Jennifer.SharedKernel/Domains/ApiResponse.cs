namespace Jennifer.SharedKernel.Domains;

public class ApiResponse<T>
{
    public T Data { get; set; }
    public List<string> Messages { get; set; }
    public bool Succeeded { get; set; }
    
    public dynamic Errors { get; set; }
    
    public new static ApiResponse<T> Fail()
    {
        return new ApiResponse<T> { Succeeded = false };
    }

    public new static ApiResponse<T> Fail(string message)
    {
        return new ApiResponse<T> { Succeeded = false, Messages = new List<string> { message } };
    }

    public new static ApiResponse<T> Fail(List<string> messages, dynamic errors)
    {
        return new ApiResponse<T> { Succeeded = false, Messages = messages, Errors = errors};
    }

    public new static Task<ApiResponse<T>> FailAsync()
    {
        return Task.FromResult(Fail());
    }

    public new static Task<ApiResponse<T>> FailAsync(string message)
    {
        return Task.FromResult(Fail(message));
    }

    public new static Task<ApiResponse<T>> FailAsync(string message, dynamic errors)
    {
        return Task.FromResult(Fail(new List<string>{ message }, errors));
    }

    public new static Task<ApiResponse<T>> FailAsync(List<string> messages, dynamic errors)
    {
        return Task.FromResult(Fail(messages, errors));
    }

    public new static ApiResponse<T> Success(string message)
    {
        return new ApiResponse<T> { Succeeded = true, Messages = new List<string>() {message} };
    }

    public static ApiResponse<T> Success(T data)
    {
        return new ApiResponse<T> { Succeeded = true, Data = data, Messages = new List<string>() { "Success." } };
    }

    public static ApiResponse<T> Success(T data, string message)
    {
        return new ApiResponse<T> { Succeeded = true, Data = data, Messages = new List<string> { "Success.", message } };
    }

    public static ApiResponse<T> Success(T data, List<string> messages)
    {
        return new ApiResponse<T> { Succeeded = true, Data = data, Messages = messages };
    }

    public new static Task<ApiResponse<T>> SuccessAsync(string message)
    {
        return Task.FromResult(Success(message));
    }

    public static Task<ApiResponse<T>> SuccessAsync(T data)
    {
        return Task.FromResult(Success(data));
    }

    public static Task<ApiResponse<T>> SuccessAsync(T data, string message)
    {
        return Task.FromResult(Success(data, message));
    }
}