namespace CoffeeBeanExplorer.Domain.Exceptions;

public class BadRequestException(string message, string errorCode = "BadRequest")
    : ApiException(message, 400, errorCode);

public class UnauthorizedException(string message, string errorCode = "Unauthorized")
    : ApiException(message, 401, errorCode);

public class NotFoundException(string message, string errorCode = "NotFound")
    : ApiException(message, 404, errorCode);

public class InternalServerErrorException(string message, string errorCode = "InternalServerError")
    : ApiException(message, 500, errorCode);
