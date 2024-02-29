using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Sixnet.Validation;
using Sixnet.DependencyInjection;

namespace Sixnet.Web.Mvc.ModelBinding.Validation
{
    public class SixnetDataAnnotationsClientModelValidatorProvider : IClientModelValidatorProvider
    {
        private readonly IOptions<MvcDataAnnotationsLocalizationOptions> _options;
        private readonly IStringLocalizerFactory _stringLocalizerFactory;
        private readonly IValidationAttributeAdapterProvider _validationAttributeAdapterProvider;

        public SixnetDataAnnotationsClientModelValidatorProvider()
        {
            IServiceProvider serviceProvider = SixnetContainer.ServiceProvider;
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

        /// <inheritdoc />
        public void CreateValidators(ClientValidatorProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            IStringLocalizer stringLocalizer = null;
            if (_options.Value.DataAnnotationLocalizerProvider != null && _stringLocalizerFactory != null)
            {
                // This will pass first non-null type (either containerType or modelType) to delegate.
                // Pass the root model type(container type) if it is non null, else pass the model type.
                stringLocalizer = _options.Value.DataAnnotationLocalizerProvider(
                    context.ModelMetadata.ContainerType ?? context.ModelMetadata.ModelType,
                    _stringLocalizerFactory);
            }

            var hasRequiredAttribute = false;
            var metadata = context.ModelMetadata;
            var isPropertyValidation = metadata.ContainerType != null && !String.IsNullOrEmpty(metadata.PropertyName);
            var rules = SixnetValidations.GetValidationRules(metadata.ContainerType, metadata.PropertyName);
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
                var validatorItem = new ClientValidatorItem(validationAttribute);
                if (validatorItem.Validator != null)
                {
                    // Check if a required attribute is already cached.
                    hasRequiredAttribute |= validatorItem.Validator is RequiredAttributeAdapter;
                    continue;
                }
                hasRequiredAttribute |= validationAttribute is RequiredAttribute;
                var adapter = _validationAttributeAdapterProvider.GetAttributeAdapter(validationAttribute, stringLocalizer);
                if (adapter != null)
                {
                    validatorItem.Validator = adapter;
                    validatorItem.IsReusable = true;
                }
                context.Results.Add(validatorItem);
            }

            if (!hasRequiredAttribute && context.ModelMetadata.IsRequired)
            {
                // Add a default '[Required]' validator for generating HTML if necessary.
                context.Results.Add(new ClientValidatorItem
                {
                    Validator = _validationAttributeAdapterProvider.GetAttributeAdapter(
                        new RequiredAttribute(),
                        stringLocalizer),
                    IsReusable = true
                });
            }
        }
    }
}
