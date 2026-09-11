using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RoomRental.Application.Abstraction.Exceptions;
using RoomRental.Domain.Exceptions;

namespace RoomRental.ExceptionHandling;

/// <summary>
/// Переводит исключения домена и слоя приложения в коды ответа.
/// Наружу уходит только текст сообщения - трассировка остаётся в логах.
/// </summary>
internal class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    /// <inheritdoc />
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title) = Resolve(exception);

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Необработанная ошибка при {Method} {Path}",
                httpContext.Request.Method, httpContext.Request.Path);
        }

        httpContext.Response.StatusCode = statusCode;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = statusCode == StatusCodes.Status500InternalServerError
                    ? "Внутренняя ошибка сервера."
                    : exception.Message,
            },
        });
    }

    private static (int StatusCode, string Title) Resolve(Exception exception) => exception switch
    {
        InvalidCredentialsException => (StatusCodes.Status401Unauthorized, "Неверные учётные данные"),
        AccessDeniedException => (StatusCodes.Status403Forbidden, "Доступ запрещён"),
        EntityNotFoundException => (StatusCodes.Status404NotFound, "Не найдено"),
        SlotAlreadyBookedException => (StatusCodes.Status409Conflict, "Время занято"),
        EmailAlreadyTakenException => (StatusCodes.Status409Conflict, "Email занят"),
        BookingAlreadyStartedException => (StatusCodes.Status409Conflict, "Бронь уже началась"),
        ArgumentException => (StatusCodes.Status400BadRequest, "Некорректный запрос"),
        _ => (StatusCodes.Status500InternalServerError, "Внутренняя ошибка сервера"),
    };
}
