using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Web.API
{
    public static class ApiConstants
    {
        /// <summary>
        /// Defines api response status code
        /// </summary>
        public static class ResponseCodes
        {
            /// <summary>
            /// Response success
            /// </summary>
            public const string RC200 = "200";

            /// <summary>
            /// Unauthorized
            /// </summary>
            public const string RC401 = "401";

            /// <summary>
            /// Token expired
            /// </summary>
            public const string RC402 = "402";

            /// <summary>
            /// Access is denied
            /// </summary>
            public const string RC403 = "403";

            /// <summary>
            /// Not found
            /// </summary>
            public const string RC404 = "404";

            /// <summary>
            /// Internal server error
            /// </summary>
            public const string RC500 = "500";
        }

        public static Dictionary<string, string> ResponseMessages = new Dictionary<string, string>()
        {
            { ResponseCodes.RC200,"Successful" },
            { ResponseCodes.RC401,"Unauthorized" },
            { ResponseCodes.RC402,"Token expired" },
            { ResponseCodes.RC403,"Access is denied" },
            { ResponseCodes.RC404,"Not found" },
            { ResponseCodes.RC500,"Internal server error" }
        };
    }
}
