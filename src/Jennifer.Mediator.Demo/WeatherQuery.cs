using FluentValidation;
using Mediator;

namespace Jennifer.Mediator.Demo;

public sealed record GetWeatherForecastQuery() : IRequest<WeatherForecastDto[]>;

// WeatherForecastDto.cs
public sealed record WeatherForecastDto(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

public sealed class GetWeatherForecastQueryHandler : IRequestHandler<GetWeatherForecastQuery, WeatherForecastDto[]>
{
    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild",
        "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    public ValueTask<WeatherForecastDto[]> Handle(GetWeatherForecastQuery request, CancellationToken cancellationToken)
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
            new WeatherForecastDto(
                DateTime.Now.AddDays(index),
                Random.Shared.Next(-20, 55),
                Summaries[Random.Shared.Next(Summaries.Length)]
            )).ToArray();

        return ValueTask.FromResult(forecast);
    }
}

public sealed record CreateWeatherForecastCommand(WeatherForecastDto[] Forecasts) : ICommand<long>;
public sealed class CreateWeatherForecastCommandHandler : ICommandHandler<CreateWeatherForecastCommand, long>
{
    public async ValueTask<long> Handle(CreateWeatherForecastCommand command, CancellationToken cancellationToken)
    {
        return 100;
    }
}

public sealed record RemoveWeatherForecastCommand(long Id) : ICommand<Result>;
public sealed class RemoveWeatherForecastCommandHandler(ISender sender) : ICommandHandler<RemoveWeatherForecastCommand, Result>
{
    public async ValueTask<Result> Handle(RemoveWeatherForecastCommand command, CancellationToken cancellationToken)
    {
        await Task.Delay(1000, cancellationToken);
        var result = await sender.Send(new GetWeatherForecastQuery(), cancellationToken);
        if (result is not null)
        {
            
        }
        return Result.Success();
    }
}
public sealed class RemoveWeatherForecastCommandValidator : AbstractValidator<RemoveWeatherForecastCommand>
{
    public RemoveWeatherForecastCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id는 0보다 커야 합니다.");
    }
}

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

public class Error
{
    public string Code { get; set; }
    public string Message { get; set; }
    public Error[] Errors { get; set; }

    public Error(string code, string message)
    {
        this.Code = code;
        this.Message = message;   
    }
}

public class ValidationError : Error
{
    public ValidationError(Error[] erros) : base("Validation.General",
        "One or more validation errors occurred")
    {
        this.Errors = erros;
    }
}