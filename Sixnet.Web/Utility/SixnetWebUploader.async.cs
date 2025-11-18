using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

using Sixnet.Exceptions;
using Sixnet.IO;
using Sixnet.Serialization.Json;

namespace Sixnet.Web.Utility
{
    /// <summary>
    /// Web uploader
    /// </summary>
    public static partial class SixnetWebUploader
    {
        #region Upload

        /// <summary>
        /// Upload by configuration
        /// </summary>
        /// <param name="parameter">Upload files</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return upload result</returns>
        public static async Task<SixnetUploadResult> UploadAsync(SixnetWebUploadParameter parameter, Dictionary<string, string> parameters = null)
        {
            SixnetThrower.ThrowArgNullIf(parameter?.Items.IsNullOrEmpty() ?? true, "Files is null or empty");
            var result = await SixnetFileManager.UploadAsync(parameter.Items.Select(c => new SixnetUploadFile()
            {
                FileName = c.FileName,
                ObjectName = c.ObjectName,
                Suffix = c.Suffix,
                Content = c.Content.OpenReadStream().ToBytes()
            }), parameters).ConfigureAwait(false);
            return HandleUploadResult(result);
        }

        #endregion
    }
}
