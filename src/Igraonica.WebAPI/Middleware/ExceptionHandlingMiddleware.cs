using Igraonica.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Igraonica.WebAPI.Middleware;

/// <summary>
/// Sve greske se obradjuju na jednom mestu, umesto da se hvataju u svakoj
/// metodi kontrolera. Domenski izuzeci se preslikavaju u odgovarajuce
/// statusne kodove protokola HTTP, a odgovor se oblikuje po standardu
/// RFC 7807.
/// </summary>
public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (DomainException ex)
        {
            await WriteProblemAsync(context, MapStatusCode(ex), ex.Message, ex.ErrorCode);
        }
        catch (Exception ex)
        {
            // Neocekivane greske se beleze u punom obliku, ali se korisniku
            // vraca opsta poruka, bez tehnickih detalja koji bi mogli
            // otkriti unutrasnju strukturu sistema (NFZ-10).
            logger.LogError(ex, "Neocekivana greska: {Path}", context.Request.Path);

            await WriteProblemAsync(context, StatusCodes.Status500InternalServerError,
                "Doslo je do neocekivane greske. Molimo pokusajte ponovo.", "INTERNAL_ERROR");
        }
    }

    private static int MapStatusCode(DomainException ex) => ex switch
    {
        CapacityExceededException => StatusCodes.Status409Conflict,
        EmailNotVerifiedException => StatusCodes.Status403Forbidden,
        EntityNotFoundException => StatusCodes.Status404NotFound,
        CancellationDeadlinePassedException => StatusCodes.Status400BadRequest,
        CancellationReasonRequiredException => StatusCodes.Status400BadRequest,
        InvalidStateTransitionException => StatusCodes.Status409Conflict,
        InvalidReservationDateException => StatusCodes.Status400BadRequest,
        DuplicateEmailException => StatusCodes.Status409Conflict,
        InvalidCredentialsException => StatusCodes.Status401Unauthorized,
        InvalidTokenException => StatusCodes.Status401Unauthorized,
        ConfirmationDeadlinePassedException => StatusCodes.Status400BadRequest,
        ExpireTooEarlyException => StatusCodes.Status400BadRequest,
        CompleteTooEarlyException => StatusCodes.Status400BadRequest,
        RateLimitExceededException => StatusCodes.Status429TooManyRequests,
        ForbiddenException => StatusCodes.Status403Forbidden,
        _ => StatusCodes.Status400BadRequest,
    };

    private static async Task WriteProblemAsync(HttpContext context, int statusCode, string detail, string errorCode)
    {
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = statusCode;

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = ReasonPhrase(statusCode),
            Detail = detail,
            Type = $"https://igraonica.rs/errors/{errorCode.ToLowerInvariant()}",
            Instance = context.Request.Path,
        };
        problem.Extensions["errorCode"] = errorCode;

        await context.Response.WriteAsJsonAsync(problem);
    }

    private static string ReasonPhrase(int statusCode) => statusCode switch
    {
        400 => "Neispravan zahtev",
        401 => "Neautorizovan pristup",
        403 => "Zabranjen pristup",
        404 => "Resurs nije pronadjen",
        409 => "Konflikt",
        429 => "Previse zahteva",
        _ => "Greska servera",
    };
}
