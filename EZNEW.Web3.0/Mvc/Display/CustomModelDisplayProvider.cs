using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace EZNEW.Web.Mvc.Display
{
    public class CustomModelDisplayProvider : IDisplayMetadataProvider
    {
        public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
        {
            DisplayText customDisplay = DisplayManager.GetPropertyDisplay(context.Key.ContainerType, context.Key.Name);
            if (customDisplay != null && !string.IsNullOrWhiteSpace(customDisplay.DisplayName))
            {
                context.DisplayMetadata.DisplayName = () => customDisplay.DisplayName;
            }
        }
    }
}
