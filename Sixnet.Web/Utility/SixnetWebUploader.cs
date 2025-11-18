using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        #region Fields

        /// <summary>
        /// Default content root
        /// </summary>
        const string _defaultContentRoot = "wwwroot";

        #endregion

        #region Upload

        /// <summary>
        /// Upload file
        /// </summary>
        /// <param name="parameter">Upload files</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return upload result</returns>
        public static SixnetUploadResult Upload(SixnetWebUploadParameter parameter, Dictionary<string, string> parameters = null)
        {
            SixnetThrower.ThrowArgNullIf(parameter?.Items.IsNullOrEmpty() ?? true, "Files is null or empty");
            var result = SixnetFileManager.Upload(parameter.Items.Select(c => new SixnetUploadFile()
            {
                FileName = c.FileName,
                ObjectName = c.ObjectName,
                Suffix = c.Suffix,
                Content = c.Content.OpenReadStream().ToBytes()
            }), parameters);
            return HandleUploadResult(result);
        }

        #endregion

        #region Handle upload result

        /// <summary>
        /// Handle upload result
        /// </summary>
        /// <param name="result">Original result</param>
        /// <returns>Return the newest upload result</returns>
        internal static SixnetUploadResult HandleUploadResult(SixnetUploadResult result)
        {
            if (result == null)
            {
                return null;
            }
            result.Files?.ForEach(r =>
            {
                r.RelativePath = r.RelativePath.LSplit(_defaultContentRoot)[0].Trim('\\', '/');
                r.FullPath = HttpClientHelper.GetFullPath(r.RelativePath);
            });
            return result;
        }

        #endregion
    }

    /// <summary>
    /// Web upload parameter
    /// </summary>
    public class SixnetWebUploadParameter
    {
        /// <summary>
        /// Gets or sets the file items
        /// </summary>
        public List<SixnetWebUploadFile> Items { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SixnetWebUploadFile
    {
        /// <summary>
        /// Gets or sets the file object name
        /// </summary>
        public string ObjectName { get; set; }

        /// <summary>
        /// Gets or sets file name
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets file suffix
        /// </summary>
        public string Suffix { get; set; }

        /// <summary>
        /// Gets or sets the file
        /// </summary>
        public IFormFile Content { get; set; }
    }
}
