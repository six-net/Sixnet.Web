using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace Sixnet.Web.Mvc.ModelBinding.Display
{
    public class SixnetModelDisplayProvider : IDisplayMetadataProvider
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
