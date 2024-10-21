using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
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
        /// Upload by current http request
        /// </summary>
        /// <returns>Return upload result</returns>
        public static SixnetUploadResult Upload()
        {
            return Upload(HttpContextHelper.Current.Request);
        }

        /// <summary>
        /// Upload by http request
        /// </summary>
        /// <param name="request">Http request</param>
        /// <returns>Return upload result</returns>
        public static SixnetUploadResult Upload(HttpRequest request)
        {
            var uploadParameter = SixnetJsonSerializer.Deserialize<SixnetRemoteUploadParameter>(request?.Form[SixnetRemoteUploadParameter.RequestParameterName] ?? "");
            if (uploadParameter == null)
            {
                return SixnetUploadResult.FailResult();
            }
            uploadParameter.Files ??= new List<SixnetUploadFile>();
            var files = new Dictionary<string, byte[]>();
            if (!request.Form.Files.IsNullOrEmpty())
            {
                foreach (var file in request.Form.Files)
                {
                    var fileSetting = uploadParameter.Files.FirstOrDefault(c => c.FileName == file.FileName);
                    if (fileSetting == null)
                    {
                        uploadParameter.Files.Add(new SixnetUploadFile()
                        {
                            FileName = file.FileName,
                            FileContent = file.OpenReadStream().ToBytes()
                        });
                    }
                    else
                    {
                        fileSetting.FileContent = file.OpenReadStream().ToBytes();
                    }
                }
            }
            return Upload(uploadParameter.Files, request.GetAllParameters());
        }

        /// <summary>
        /// Upload file
        /// </summary>
        /// <param name="files">File</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return upload result</returns>
        public static SixnetUploadResult Upload(IEnumerable<SixnetUploadFile> files, object parameters = null)
        {
            return Upload(files, parameters?.ToStringDictionary());
        }

        /// <summary>
        /// Upload file
        /// </summary>
        /// <param name="file">File</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return upload result</returns>
        public static SixnetUploadResult Upload(SixnetUploadFile file, Dictionary<string, string> parameters = null)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }
            return Upload(new List<SixnetUploadFile>(1) { file }, parameters);
        }

        /// <summary>
        /// Upload file
        /// </summary>
        /// <param name="files">Upload files</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return upload result</returns>
        public static SixnetUploadResult Upload(IEnumerable<SixnetUploadFile> files, Dictionary<string, string> parameters = null)
        {
            var result = SixnetFileManager.Upload(files, parameters);
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
}
