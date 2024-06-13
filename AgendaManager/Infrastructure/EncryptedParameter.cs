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
        return value is not null
            ? protector.Protect(value.ToString()!) : null;
    }

    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var key = bindingContext.FieldName;
        var valueProviderResult = bindingContext.ValueProvider.GetValue(key);

        if (valueProviderResult.FirstValue is not { } value) return Task.CompletedTask;
        
        var result = protector.Unprotect(value);
        if (bindingContext.ModelType == typeof(Guid))
        {
            bindingContext.Result = ModelBindingResult.Success(Guid.Parse(result));    
        }
        else
        {
            // try to change string to something "normal", will probably fail on weirdo types
            var converted = Convert.ChangeType(result, bindingContext.ModelType);
            bindingContext.Result = ModelBindingResult.Success(converted);
        }

        return Task.CompletedTask;
    }
}