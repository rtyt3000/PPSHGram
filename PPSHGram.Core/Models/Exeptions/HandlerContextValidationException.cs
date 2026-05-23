namespace PPSHGram.Core.Models.Exeptions;

public sealed class HandlerContextValidationException : Exception
{
    public HandlerContextValidationException(string message) : base(message)
    {
    }
}
