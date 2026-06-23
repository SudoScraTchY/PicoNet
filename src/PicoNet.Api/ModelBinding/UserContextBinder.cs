using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PicoNet.Contracts.DTOs.Requests;
using PicoNet.Contracts.DTOs.Requests.Auth;

namespace PicoNet.Api.ModelBinding;

public class UserContextBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var httpContext = bindingContext.HttpContext;
        var claim = httpContext.User.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        if (claim is null || !Guid.TryParse(claim.Value, out var userId))
        {
            bindingContext.Result = ModelBindingResult.Failed();
            return Task.CompletedTask;
        }

        bindingContext.Result = ModelBindingResult.Success(new UserContext(userId));
        return Task.CompletedTask;
    }
}