using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Sixnet.Net.Upload;
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
        /// Upload by current http request
        /// </summary>
        /// <returns>Return upload result</returns>
        public static Task<UploadResult> UploadAsync()
        {
            return UploadAsync(HttpContextHelper.Current.Request);
        }

        /// <summary>
        /// Upload by http request
        /// </summary>
        /// <param name="request">Http request</param>
        /// <returns>Return upload result</returns>
        public static async Task<UploadResult> UploadAsync(HttpRequest request)
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
            return await UploadAsync(uploadParameter.Files, request.GetAllParameters()).ConfigureAwait(false);
        }

        /// <summary>
        /// Upload file
        /// </summary>
        /// <param name="files">File</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return upload result</returns>
        public static async Task<UploadResult> UploadAsync(IEnumerable<UploadFile> files, object parameters = null)
        {
            return await UploadAsync(files, parameters?.ToStringDictionary()).ConfigureAwait(false);
        }

        /// <summary>
        /// Upload file
        /// </summary>
        /// <param name="file">File</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return upload result</returns>
        public static async Task<UploadResult> UploadAsync(UploadFile file, Dictionary<string, string> parameters = null)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }
            return await UploadAsync(new List<UploadFile>(1) { file }, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Upload by configuration
        /// </summary>
        /// <param name="files">Upload files</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return upload result</returns>
        public static async Task<UploadResult> UploadAsync(IEnumerable<UploadFile> files, Dictionary<string, string> parameters = null)
        {
            var result = await SixnetUploader.UploadAsync(files, parameters).ConfigureAwait(false);
            return HandleUploadResult(result);
        }

        #endregion
    }
}
