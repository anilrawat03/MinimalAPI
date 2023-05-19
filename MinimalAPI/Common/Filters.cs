using FluentValidation;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;
using System.Reflection;
using System.Xml;
using System.Linq;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace MinimalAPI.Common
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class ValidateAttribute : Attribute
    {
    }
    public static class ValidationFilter
    {
        public static EndpointFilterDelegate ValidationFilterFactory(EndpointFilterFactoryContext context, EndpointFilterDelegate next)
        {
            IEnumerable<ValidationDescriptor> validationDescriptors = GetValidators(context.MethodInfo, context.ApplicationServices);

            if (validationDescriptors.Any())
            {
                return invocationContext => Validate(validationDescriptors, invocationContext, next);
            }

            // pass-thru
            return invocationContext => next(invocationContext);
        }

        private static async ValueTask<object?> Validate(IEnumerable<ValidationDescriptor> validationDescriptors, EndpointFilterInvocationContext invocationContext, EndpointFilterDelegate next)
        {
            foreach (ValidationDescriptor descriptor in validationDescriptors)
            {
                var argument = invocationContext.Arguments[descriptor.ArgumentIndex];

                if (argument is not null)
                {
                    var validationResult = await descriptor.Validator.ValidateAsync(
                        new ValidationContext<object>(argument)
                    );

                    if (!validationResult.IsValid)
                    {
                        return Results.BadRequest(new APIResponse
                        {
                            Message = validationResult.Errors.FirstOrDefault()?.ErrorMessage
                        });
                    }
                }
            }

            return await next.Invoke(invocationContext);
        }

        static IEnumerable<ValidationDescriptor> GetValidators(MethodInfo methodInfo, IServiceProvider serviceProvider)
        {
            ParameterInfo[] parameters = methodInfo.GetParameters();

            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo parameter = parameters[i];

                if (parameter.GetCustomAttribute<ValidateAttribute>() is not null)
                {
                    Type validatorType = typeof(IValidator<>).MakeGenericType(parameter.ParameterType);

                    // Note that FluentValidation validators needs to be registered as singleton
                    IValidator? validator = serviceProvider.GetService(validatorType) as IValidator;

                    if (validator is not null)
                    {
                        yield return new ValidationDescriptor { ArgumentIndex = i, ArgumentType = parameter.ParameterType, Validator = validator };
                    }
                }
            }
        }

        private class ValidationDescriptor
        {
            public required int ArgumentIndex { get; init; }
            public required Type ArgumentType { get; init; }
            public required IValidator Validator { get; init; }
        }
    }
    public abstract class EndpointFilters : IEndpointFilter
    {
        protected EndpointFilters()
        {
        }
        public virtual async ValueTask<object> InvokeAsync(EndpointFilterInvocationContext context,
            EndpointFilterDelegate next)
        {
            // var user = context.HttpContext.LoggedUser();
            if (context.HttpContext.Request.ContentType == "application/x-www-form-urlencoded")
            {
                string json = JsonConvert.SerializeObject(context.HttpContext.Request.Form, Formatting.Indented);
            }
            context.HttpContext.Items.Add("userdata", "anil Rawat");
            return await next(context);
        }

        public string GetBodyStringFromDictionary(Dictionary<string, string> formField)
        {
            string body = "{";
            foreach (var pair in formField)
            {
                body += $"{pair.Key}={pair.Value}&";
            }
            body = "}";
            // delete last "&"
            body = body.Substring(0, body.Length - 1);

            return body;
        }
    }
    public class AEndpointFilter : EndpointFilters
    {
        public AEndpointFilter() { }
    }
    public static class BindingExtensions
    {
        public static async Task<T> BindFromForm<T>(this HttpContext httpContext)
        {
            var serviceProvider = httpContext.RequestServices;
            var factory = serviceProvider.GetRequiredService<IModelBinderFactory>();
            var metadataProvider = serviceProvider.GetRequiredService<IModelMetadataProvider>();
            var metadata = metadataProvider.GetMetadataForType(typeof(T));
            var modelBinder = factory.CreateBinder(new()
            {
                Metadata = metadata
            });

            var context = new DefaultModelBindingContext
            {
                ModelMetadata = metadata,
                ModelName = string.Empty,
                ValueProvider = new FormValueProvider(
                    BindingSource.Form,
                    httpContext.Request.Form,
                    CultureInfo.InvariantCulture
                ),
                ActionContext = new ActionContext(
                    httpContext,
                    new RouteData(),
                    new ActionDescriptor()),
                ModelState = new ModelStateDictionary()
            };
            await modelBinder.BindModelAsync(context);
            return (T)context.Result.Model;
        }
    }
}
