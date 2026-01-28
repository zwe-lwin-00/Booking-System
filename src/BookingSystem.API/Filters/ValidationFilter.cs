using FluentValidation;

namespace BookingSystem.API.Filters;

public class ValidationFilter<T> : IEndpointFilter
{
    private readonly IValidator<T> _validator;

    public ValidationFilter(IValidator<T> validator)
    {
        _validator = validator;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var model = context.Arguments.OfType<T>().FirstOrDefault();
        if (model == null)
        {
            return Results.BadRequest("Request body is required.");
        }

        var validationResult = await _validator.ValidateAsync(model);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.Errors);
        }

        return await next(context);
    }
}
