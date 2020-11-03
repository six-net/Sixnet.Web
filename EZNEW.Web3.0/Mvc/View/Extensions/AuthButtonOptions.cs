using System.Collections.Generic;

namespace EZNEW.Web.Mvc.View.Extension
{
    /// <summary>
    /// Auth button options
    /// </summary>
    public class AuthButtonOptions
    {
        /// <summary>
        /// Gets or sets the text
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the size
        /// </summary>
        public ButtonSize Size { get; set; } = ButtonSize.Normal;

        /// <summary>
        /// Gets or sets the forbid style
        /// </summary>
        public ForbidStyle ForbidStyle { get; set; } = ForbidStyle.Disable;

        /// <summary>
        /// Gets or sets the action options
        /// </summary>
        public AuthorizationActionOptions ActionOptions { get; set; }

        /// <summary>
        /// Gets or sets whether use now verify result
        /// </summary>
        public bool UseNowVerifyResult { get; set; }

        /// <summary>
        /// Gets or sets whether allow access
        /// </summary>
        public bool AllowAccess { get; set; }

        /// <summary>
        /// Gets or sets whether use ico
        /// </summary>
        public bool UseIco { get; set; }

        /// <summary>
        /// Gets or sets the html attributes
        /// </summary>
        public IDictionary<string, object> HtmlAttributes { get; set; }

        /// <summary>
        /// Gets or seets the ico html attributes
        /// </summary>
        public IDictionary<string, object> IcoHtmlAttributes { get; set; }
    }
}
