using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AgendaManager.Infrastructure;

public class EncryptedParameter(IDataProtectionProvider dataProtectionProvider) :
    IModelBinder,
    IOutboundParameterTransformer
{
    private readonly IDataProtector protector
        = dataProtectionProvider.CreateProtector("EncryptedParameter");

    public string? TransformOutbound(object? value)
    {
        if (value is not string s) return null;
        return protector.Protect(s);
    }

    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var key = bindingContext.FieldName;
        var valueProviderResult = bindingContext.ValueProvider.GetValue(key);

        if (valueProviderResult.FirstValue is { } value)
        {
            var result = protector.Unprotect(value);
            bindingContext.Result = ModelBindingResult.Success(result);
        }

        return Task.CompletedTask;
    }
}