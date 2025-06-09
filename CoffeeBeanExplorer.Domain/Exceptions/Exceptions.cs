namespace CoffeeBeanExplorer.Domain.Exceptions;

public class BadRequestException(string message, string errorCode = "BadRequest", Exception? innerException = null)
    : ApiException(message, 400, errorCode, innerException);

public class UnauthorizedException(string message, string errorCode = "Unauthorized", Exception? innerException = null)
    : ApiException(message, 401, errorCode, innerException);

public class NotFoundException(string message, string errorCode = "NotFound", Exception? innerException = null)
    : ApiException(message, 404, errorCode, innerException);

public class InternalServerErrorException(
    string message,
    string errorCode = "InternalServerError",
    Exception? innerException = null)
    : ApiException(message, 500, errorCode, innerException);
