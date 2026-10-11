using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;
using TalaanNiAna.Application.Common.Exceptions;

namespace TalaanNiAna.API.Filters;

/// <summary>
/// Validates every action argument that has a registered validator, so endpoints
/// never repeat it. Failures become the same ProblemDetails shape as other errors.
/// </summary>
public class ValidationFilter : IAsyncActionFilter
{
    private readonly IServiceProvider _services;

    public ValidationFilter(IServiceProvider services)
    {
        _services = services;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var failures = new List<FluentValidation.Results.ValidationFailure>();

        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null) continue;

            var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());

            if (_services.GetService(validatorType) is not IValidator validator) continue;

            var validationContext = new ValidationContext<object>(argument);
            var result = await validator.ValidateAsync(validationContext, context.HttpContext.RequestAborted);

            if (!result.IsValid)
                failures.AddRange(result.Errors);
        }

        if (failures.Count > 0)
        {
            var errors = failures
                .GroupBy(f => f.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(f => f.ErrorMessage).Distinct().ToArray());

            throw new RequestValidationException(errors);
        }

        await next();
    }
}
