namespace CoffeeBeanExplorer.Domain.Exceptions;

public abstract class ApiException(string message, int statusCode, string errorCode) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
    public string ErrorCode { get; } = errorCode;
}