using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using EZNEW.DataValidation;
using EZNEW.DependencyInjection;

namespace EZNEW.Web.Mvc.Validation
{
    public class CustomDataAnnotationsModelValidatorProvider : IModelValidatorProvider
    {
        private readonly IOptions<MvcDataAnnotationsLocalizationOptions> _options;
        private readonly IStringLocalizerFactory _stringLocalizerFactory;
        private readonly IValidationAttributeAdapterProvider _validationAttributeAdapterProvider;

        public CustomDataAnnotationsModelValidatorProvider()
        {
            IServiceProvider serviceProvider = ContainerManager.ServiceProvider;
            IValidationAttributeAdapterProvider validationAttributeAdapterProvider = serviceProvider.GetService(typeof(IValidationAttributeAdapterProvider)) as IValidationAttributeAdapterProvider;
            IOptions<MvcDataAnnotationsLocalizationOptions> options = serviceProvider.GetService(typeof(IOptions<MvcDataAnnotationsLocalizationOptions>)) as IOptions<MvcDataAnnotationsLocalizationOptions>;
            IStringLocalizerFactory stringLocalizerFactory = serviceProvider.GetService(typeof(IStringLocalizerFactory)) as IStringLocalizerFactory;
            if (validationAttributeAdapterProvider == null)
            {
                throw new ArgumentNullException(nameof(validationAttributeAdapterProvider));
            }
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _validationAttributeAdapterProvider = validationAttributeAdapterProvider;
            _options = options;
            _stringLocalizerFactory = stringLocalizerFactory;
        }

        public void CreateValidators(ModelValidatorProviderContext context)
        {
            IStringLocalizer stringLocalizer = null;
            if (_stringLocalizerFactory != null && _options.Value.DataAnnotationLocalizerProvider != null)
            {
                stringLocalizer = _options.Value.DataAnnotationLocalizerProvider(
                    context.ModelMetadata.ContainerType ?? context.ModelMetadata.ModelType,
                    _stringLocalizerFactory);
            }
            var metadata = context.ModelMetadata;
            var isPropertyValidation = metadata.ContainerType != null && !String.IsNullOrEmpty(metadata.PropertyName);
            var rules = ValidationManager.GetValidationRules(metadata.ContainerType, metadata.PropertyName);
            if (rules == null)
            {
                return;
            }
            foreach (var rule in rules)
            {
                var validationAttribute = rule.CreateValidationAttribute();
                if (validationAttribute == null)
                {
                    continue;
                }
                var validatorItem = new ValidatorItem(validationAttribute);
                if (validatorItem.Validator != null)
                {
                    continue;
                }

                var attribute = validatorItem.ValidatorMetadata as ValidationAttribute;
                if (attribute == null)
                {
                    continue;
                }

                var validator = new CustomDataAnnotationsModelValidator(
                    _validationAttributeAdapterProvider,
                    attribute,
                    stringLocalizer);

                validatorItem.Validator = validator;
                validatorItem.IsReusable = true;
                // Inserts validators based on whether or not they are 'required'. We want to run
                // 'required' validators first so that we get the best possible error message.
                if (attribute is RequiredAttribute)
                {
                    context.Results.Remove(validatorItem);
                    context.Results.Insert(0, validatorItem);
                }
                context.Results.Add(validatorItem);
            }

            // Produce a validator if the type supports IValidatableObject
            if (typeof(IValidatableObject).IsAssignableFrom(context.ModelMetadata.ModelType))
            {
                context.Results.Add(new ValidatorItem
                {
                    Validator = new CustomValidatableObjectAdapter(),
                    IsReusable = true
                });
            }
        }
    }
}
