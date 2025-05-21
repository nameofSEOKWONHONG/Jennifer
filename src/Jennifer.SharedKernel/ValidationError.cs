namespace Jennifer.SharedKernel;

public sealed class ValidationError : Error
{
    public ValidationError(Error[] erros) : base("Validation.General",
        "One or more validation errors occurred")
    {
        this.Errors = erros;
    }
}