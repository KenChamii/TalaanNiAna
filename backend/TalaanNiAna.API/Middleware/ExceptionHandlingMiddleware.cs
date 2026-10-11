using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TalaanNiAna.Application.Common.Exceptions;

namespace TalaanNiAna.API.Middleware;

/// <summary>
/// Turns exceptions into RFC 7807 ProblemDetails. Domain exceptions keep their
/// message because the user needs it; anything else returns a generic 500.
/// </summary>
public class ExceptionHandlingMiddleware
{
    /// <summary>Nginx's "client closed request". Not in StatusCodes.</summary>
    private const int ClientClosedRequest = 499;

    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web);

    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await WriteProblemAsync(context, ex);
        }
    }

    private async Task WriteProblemAsync(HttpContext context, Exception ex)
    {
        if (context.Response.HasStarted)
        {
            _logger.LogError(ex, "Exception after the response started for {Method} {Path}",
                context.Request.Method, context.Request.Path);
            return;
        }

        var problem = Map(ex, context);

        if (problem.Status >= StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(ex, "Unhandled exception processing {Method} {Path}",
                context.Request.Method, context.Request.Path);
        }
        else
        {
            _logger.LogInformation("{Status} on {Method} {Path}: {Message}",
                problem.Status, context.Request.Method, context.Request.Path, ex.Message);
        }

        problem.Instance = context.Request.Path;
        problem.Extensions["traceId"] = context.TraceIdentifier;

        context.Response.Clear();
        context.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsync(JsonSerializer.Serialize(problem, JsonOptions));
    }

    private static ProblemDetails Map(Exception ex, HttpContext context) => ex switch
    {
        RequestValidationException validation => Problem(
            StatusCodes.Status400BadRequest, "Hindi tama ang detalye", validation.Message,
            validation.Code, errors: validation.Errors),

        NotFoundException notFound => Problem(
            StatusCodes.Status404NotFound, "Hindi nahanap", notFound.Message, notFound.Code),

        ConflictException conflict => Problem(
            StatusCodes.Status409Conflict, "Hindi matuloy", conflict.Message, conflict.Code),

        DomainException domain => Problem(
            StatusCodes.Status400BadRequest, "Hindi matuloy", domain.Message, domain.Code),

        // Lost an optimistic concurrency race. The caller should reload and retry.
        DbUpdateConcurrencyException => Problem(
            StatusCodes.Status409Conflict, "Hindi matuloy",
            "Nabago ang datos habang nagse-save. Pakisubukan ulit.", "stock_changed"),

        OperationCanceledException when context.RequestAborted.IsCancellationRequested => Problem(
            ClientClosedRequest, "Kanselado", "Kanselado ang request.", "cancelled"),

        _ => Problem(
            StatusCodes.Status500InternalServerError, "May error sa server",
            "May naganap na error sa server. Pakisubukan ulit.", "server_error")
    };

    private static ProblemDetails Problem(
        int status,
        string title,
        string detail,
        string code,
        IDictionary<string, string[]>? errors = null)
    {
        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail
        };

        problem.Extensions["code"] = code;

        if (errors is not null)
            problem.Extensions["errors"] = errors;

        return problem;
    }
}
