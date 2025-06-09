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

    private static Result Success() => new() { IsSuccess = true };
    private static Result Failure(string message) => new() { IsSuccess = false, Message = message };
    private static Result Failure(Error error) => new() { IsSuccess = false, Error = error};
    public static Task<Result> SuccessAsync() => Task.FromResult(Success());
    public static Task<Result> FailureAsync(string message) => Task.FromResult(Failure(message));
    public static Task<Result> FailureAsync(Error error) => Task.FromResult(Failure(error));   
}

public class Result<T>: Result, IResult
{
    public T Data { get; set; }
    
    private static Result<T> Success(T data) => new() { IsSuccess = true, Data = data };
    private new static Result<T> Failure(string message) => new() { IsSuccess = false, Message = message };
    private new static Result<T> Failure(Error error) => new() { IsSuccess = false, Error = error};
    public static Task<Result<T>> SuccessAsync(T data) => Task.FromResult(Success(data));
    public new static Task<Result<T>> FailureAsync(string message) => Task.FromResult(Failure(message));
    public new static Task<Result<T>> FailureAsync(Error error) => Task.FromResult(Failure(error));  
}

public class PaginatedResult<T> : Result<T>, IResult
{
    public new IEnumerable<T> Data { get; set; }
    
    public int PageNo { get; set; }

    public int TotalPages { get; set; }

    public int TotalCount { get; set; }
    public int PageSize { get; set; }

    public bool HasPreviousPage => PageNo > 1;

    public bool HasNextPage => PageNo < TotalPages;
    
    public static Task<PaginatedResult<T>> SuccessAsync(int totalCount, T[] data, int pageNo, int pageSize) =>
        Task.FromResult(new PaginatedResult<T>
        {
            IsSuccess = true,
            Data = data,
            PageNo = pageNo,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        });
}