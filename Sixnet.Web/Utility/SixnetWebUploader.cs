using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Sixnet.Net.Upload;
using Sixnet.Serialization;

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
        public static UploadResult Upload()
        {
            return Upload(HttpContextHelper.Current.Request);
        }

        /// <summary>
        /// Upload by http request
        /// </summary>
        /// <param name="request">Http request</param>
        /// <returns>Return upload result</returns>
        public static UploadResult Upload(HttpRequest request)
        {
            var uploadParameter = SixnetJsonSerializer.Deserialize<RemoteUploadParameter>(request?.Form[RemoteUploadParameter.RequestParameterName] ?? "");
            if (uploadParameter == null)
            {
                return UploadResult.FailResult();
            }
            uploadParameter.Files ??= new List<UploadFile>();
            var files = new Dictionary<string, byte[]>();
            if (!request.Form.Files.IsNullOrEmpty())
            {
                foreach (var file in request.Form.Files)
                {
                    var fileSetting = uploadParameter.Files.FirstOrDefault(c => c.FileName == file.FileName);
                    if (fileSetting == null)
                    {
                        uploadParameter.Files.Add(new UploadFile()
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
        public static UploadResult Upload(IEnumerable<UploadFile> files, object parameters = null)
        {
            return Upload(files, parameters?.ToStringDictionary());
        }

        /// <summary>
        /// Upload file
        /// </summary>
        /// <param name="file">File</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return upload result</returns>
        public static UploadResult Upload(UploadFile file, Dictionary<string, string> parameters = null)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }
            return Upload(new List<UploadFile>(1) { file }, parameters);
        }

        /// <summary>
        /// Upload file
        /// </summary>
        /// <param name="files">Upload files</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return upload result</returns>
        public static UploadResult Upload(IEnumerable<UploadFile> files, Dictionary<string, string> parameters = null)
        {
            var result = SixnetUploader.Upload(files, parameters);
            return HandleUploadResult(result);
        }

        #endregion

        #region Handle upload result

        /// <summary>
        /// Handle upload result
        /// </summary>
        /// <param name="result">Original result</param>
        /// <returns>Return the newest upload result</returns>
        internal static UploadResult HandleUploadResult(UploadResult result)
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
