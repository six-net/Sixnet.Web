using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Sixnet.DependencyInjection;
using Sixnet.Net.Upload;
using Sixnet.Serialization;

namespace Sixnet.Web.Utility
{
    /// <summary>
    /// Web upload manager
    /// </summary>
    public static class WebUploadHelper
    {
        static WebUploadHelper()
        {
            var uploadConfiguration = ContainerManager.Resolve<IOptionsMonitor<UploadConfiguration>>()?.CurrentValue;
            UploadManager.Configure(uploadConfiguration);
        }

        /// <summary>
        /// Default content root
        /// </summary>
        const string DefaultContentRoot = "wwwroot";

        #region Upload by http request

        /// <summary>
        /// Upload by current http request
        /// </summary>
        /// <returns>Return upload result</returns>
        public static async Task<UploadResult> UploadByRequestAsync()
        {
            var request = HttpContextHelper.Current.Request;
            return await UploadByRequestAsync(request).ConfigureAwait(false);
        }

        /// <summary>
        /// Upload by http request
        /// </summary>
        /// <param name="request">Http request</param>
        /// <returns>Return upload result</returns>
        public static async Task<UploadResult> UploadByRequestAsync(HttpRequest request)
        {
            if (request == null)
            {
                return UploadResult.FailResult();
            }
            var uploadParameter = JsonSerializer.Deserialize<RemoteParameter>(request.Form[RemoteParameter.RequestParameterName]);
            if (uploadParameter == null)
            {
                return UploadResult.FailResult();
            }
            var files = new Dictionary<string, byte[]>();
            if (!request.Form.Files.IsNullOrEmpty())
            {
                foreach (var file in request.Form.Files)
                {
                    files.Add(file.FileName, file.OpenReadStream().ToBytes());
                }
            }
            return await UploadByConfigurationAsync(uploadParameter.Files, files, request.GetAllParameters()).ConfigureAwait(false);
        }

        #endregion

        #region Upload by configuration

        /// <summary>
        /// Upload by configuration
        /// </summary>
        /// <param name="fileOption">File option</param>
        /// <param name="fileBytes">File bytes</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return upload result</returns>
        public static async Task<UploadResult> UploadByConfigurationAsync(UploadFile fileOption, byte[] fileBytes, object parameters = null)
        {
            if (fileBytes.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(fileBytes));
            }
            if (fileOption == null)
            {
                throw new ArgumentNullException(nameof(fileOption));
            }
            Dictionary<string, string> parametersDict = null;
            if (parameters != null)
            {
                parametersDict = parameters.ToStringDictionary();
            }
            return await UploadByConfigurationAsync(fileOption, fileBytes, parametersDict).ConfigureAwait(false);
        }

        /// <summary>
        /// Upload by configuration
        /// </summary>
        /// <param name="fileOption">File option</param>
        /// <param name="fileBytes">File bytes</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return upload result</returns>
        public static async Task<UploadResult> UploadByConfigurationAsync(UploadFile fileOption, byte[] fileBytes, Dictionary<string, string> parameters = null)
        {
            if (fileBytes.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(fileBytes));
            }
            if (fileOption == null)
            {
                throw new ArgumentNullException(nameof(fileOption));
            }
            var files = new Dictionary<string, byte[]>()
            {
                {
                    fileOption.FileName,
                    fileBytes
                }
            };
            return await UploadByConfigurationAsync(new List<UploadFile>() { fileOption }, files, parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Upload by configuration
        /// </summary>
        /// <param name="fileOptions">File options</param>
        /// <param name="files">Upload files</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return upload result</returns>
        public static async Task<UploadResult> UploadByConfigurationAsync(IEnumerable<UploadFile> fileOptions, Dictionary<string, byte[]> files, object parameters = null)
        {
            Dictionary<string, string> parameterDict = null;
            if (parameters != null)
            {
                parameterDict = parameters.ToStringDictionary();
            }
            return await UploadByConfigurationAsync(fileOptions, files, parameterDict).ConfigureAwait(false);
        }

        /// <summary>
        /// Upload by configuration
        /// </summary>
        /// <param name="files">Upload files</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return upload result</returns>
        public static async Task<UploadResult> UploadByConfigurationAsync(IEnumerable<UploadFile> files, Dictionary<string, string> parameters = null)
        {
            var result = await UploadManager.UploadByConfigurationAsync(files, parameters).ConfigureAwait(false);
            return HandleUploadResult(result);
        }

        #endregion

        #region Remote upload

        /// <summary>
        /// Remote upload file
        /// </summary>
        /// <param name="remoteOptions">Remote options</param>
        /// <param name="files">Upload files</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return upload result</returns>
        public static async Task<UploadResult> RemoteUploadAsync(RemoteServerOptions remoteOptions, List<UploadFile> files, Dictionary<string, string> parameters = null)
        {
            var result = await UploadManager.RemoteUploadAsync(remoteOptions, files, parameters).ConfigureAwait(false);
            return HandleUploadResult(result);
        }

        /// <summary>
        /// Remote upload file
        /// </summary>
        /// <param name="remoteOption">Remote options</param>
        /// <param name="fileOption">File option</param>
        /// <param name="fileBytes">File bytes</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return upload result</returns>
        public static async Task<UploadResult> RemoteUploadAsync(RemoteServerOptions remoteOption, UploadFile fileOption, byte[] fileBytes, object parameters = null)
        {
            return await RemoteUploadAsync(remoteOption, fileOption, fileBytes, parameters).ConfigureAwait(false);
        }

        #endregion

        #region Local upload

        /// <summary>
        /// Local upload file
        /// </summary>
        /// <param name="uploadOptions">Upload options</param>
        /// <param name="files">Files</param>
        /// <param name="files">Files</param>
        /// <returns>Return upload result</returns>
        public static async Task<UploadResult> LocalUploadAsync(UploadOptions uploadOptions, List<UploadFile> files)
        {
            var result = await UploadManager.LocalUploadAsync(files, uploadOptions).ConfigureAwait(false);
            return HandleUploadResult(result);
        }

        /// <summary>
        ///  Local upload file
        /// </summary>
        /// <param name="uploadOption">Upload option</param>
        /// <param name="fileOption">File option</param>
        /// <param name="fileBytes">File</param>
        /// <returns>Return upload result</returns>
        public static async Task<UploadResult> LocalUploadAsync(UploadOptions uploadOption, UploadFile fileOption, byte[] fileBytes)
        {
            return await LocalUploadAsync(uploadOption, fileOption, fileBytes).ConfigureAwait(false);
        }

        #endregion

        #region Handle upload result

        /// <summary>
        /// Handle upload result
        /// </summary>
        /// <param name="originalResult">Original result</param>
        /// <returns>Return the newest upload result</returns>
        static UploadResult HandleUploadResult(UploadResult originalResult)
        {
            if (originalResult == null)
            {
                return null;
            }
            originalResult.Files?.ForEach(r =>
            {
                r.RelativePath = r.RelativePath.LSplit(DefaultContentRoot)[0].Trim('\\', '/');
                r.FullPath = HttpClientHelper.GetFullPath(r.RelativePath);
            });
            return originalResult;
        }

        #endregion
    }
}
