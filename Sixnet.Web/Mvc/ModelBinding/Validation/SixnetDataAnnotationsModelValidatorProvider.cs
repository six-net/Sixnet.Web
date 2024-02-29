using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Sixnet.Validation;
using Sixnet.DependencyInjection;

namespace Sixnet.Web.Mvc.ModelBinding.Validation
{
    public class SixnetDataAnnotationsModelValidatorProvider : IModelValidatorProvider
    {
        private readonly IOptions<MvcDataAnnotationsLocalizationOptions> _options;
        private readonly IStringLocalizerFactory _stringLocalizerFactory;
        private readonly IValidationAttributeAdapterProvider _validationAttributeAdapterProvider;

        public SixnetDataAnnotationsModelValidatorProvider()
        {
            var stringLocalizerFactory = SixnetContainer.GetService<IStringLocalizerFactory>();
            var validationAttributeAdapterProvider = SixnetContainer.GetService<IValidationAttributeAdapterProvider>();
            var options = SixnetContainer.GetService<IOptions<MvcDataAnnotationsLocalizationOptions>>();

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
            var isPropertyValidation = metadata.ContainerType != null && !string.IsNullOrEmpty(metadata.PropertyName);
            var rules = SixnetValidations.GetValidationRules(metadata.ContainerType, metadata.PropertyName);
            if (rules.IsNullOrEmpty())
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

                var validator = new SixnetDataAnnotationsModelValidator(
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
                else
                {
                    context.Results.Add(validatorItem);
                }
            }

            // Produce a validator if the type supports IValidatableObject
            if (typeof(IValidatableObject).IsAssignableFrom(context.ModelMetadata.ModelType))
            {
                context.Results.Add(new ValidatorItem
                {
                    Validator = new SixnetValidatableObjectAdapter(),
                    IsReusable = true
                });
            }
        }
    }
}
