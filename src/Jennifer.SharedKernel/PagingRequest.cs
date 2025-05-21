namespace Jennifer.SharedKernel;

public class PagingRequest
{
    public int PageNo { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class PagingResult<T> : Result<T>
{
    public int Total { get; set; }
    public int PageNo { get; set; }
    public int PageSize { get; set; }

    public static PagingResult<T> Success(int total, T data, int pageNo, int pageSize)
    {
        return new PagingResult<T>
        {
            Total = total,
            Data = data,
            PageNo = pageNo,
            PageSize = pageSize
        };
    }
}