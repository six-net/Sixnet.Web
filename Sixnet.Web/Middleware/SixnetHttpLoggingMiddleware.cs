using System;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Sixnet.DependencyInjection;
using Sixnet.Localization;
using Sixnet.Logging;
using Sixnet.Web.Utility;

namespace Sixnet.Web.Middleware
{
    public partial class SixnetHttpLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        const string fileDataContentType = "multipart/form-data";
        static readonly SixnetHttpLogOptions _defaultLoggingOptions = new SixnetHttpLogOptions();

        public SixnetHttpLoggingMiddleware(RequestDelegate next
            , ILogger<SixnetHttpLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Invoke
        /// </summary>
        public async Task Invoke(HttpContext httpContext)
        {
            var requestTime = DateTimeOffset.Now;

            var loggingOptions = SixnetContainer.GetOptions<SixnetLoggingOptions>(SixnetOptionsStyle.Monitor)?.Http ?? _defaultLoggingOptions;

            // path
            var path = (httpContext.Request.Path.ToString() ?? string.Empty).ToLower();
            if (string.IsNullOrWhiteSpace(path?.Trim('/')) && !loggingOptions.RecordRootPath)
            {
                await _next(httpContext).ConfigureAwait(false);
                return;
            }
            if (loggingOptions.IgnorePaths?.Any(p => path.Contains(p, StringComparison.OrdinalIgnoreCase)) ?? false)
            {
                await _next(httpContext).ConfigureAwait(false);
                return;
            }
            // method
            var method = httpContext.Request.Method;
            if (loggingOptions.IgnoreMethods?.Any(m => string.Equals(m, method, StringComparison.OrdinalIgnoreCase)) ?? false)
            {
                await _next(httpContext).ConfigureAwait(false);
                return;
            }
            // log level
            if (!_logger.IsEnabled(loggingOptions.LogLevel))
            {
                await _next(httpContext).ConfigureAwait(false);
                return;
            }
            // trace message
            var traceMessage = new SixnetHttpRequestTraceMessage()
            {
                Address = new SixnetHttpRequestAddressInfo()
                {
                    Client = HttpClientHelper.RemoteIp,
                    Host = HttpClientHelper.FullHost,
                    Protocol = HttpClientHelper.Protocol,
                    Url = HttpClientHelper.Url,
                    RawUrl = HttpClientHelper.RawUrl,
                }
            };
            try
            {
                // request info
                await BuildRequestInfoAsync(httpContext, traceMessage, loggingOptions).ConfigureAwait(false);
                // response info
                await BuildResponseInfoAsync(httpContext, traceMessage, loggingOptions).ConfigureAwait(false);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                var responseTime = DateTimeOffset.Now;
                traceMessage.Time = new SixnetHttpRequestTimeInfo()
                {
                    RequestTime = requestTime,
                    ResponseTime = responseTime,
                    TimeSpan = (responseTime - requestTime).TotalMilliseconds
                };
                _ = Task.Run(() =>
                {
                    var logMsg = JsonSerializer.Serialize(traceMessage, options: new JsonSerializerOptions()
                    {
                        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                    });
                    if (traceMessage.Exception != null)
                    {
                        SixnetLogger.LogError<SixnetHttpLoggingMiddleware>(logMsg);
                    }
                    else
                    {
                        SixnetLogger.LogInformation<SixnetHttpLoggingMiddleware>(logMsg);
                    }
                });
            }
        }

        /// <summary>
        /// Build request info
        /// </summary>
        /// <param name="request"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        private static async Task BuildRequestInfoAsync(HttpContext context, SixnetHttpRequestTraceMessage traceMessage, SixnetHttpLogOptions options)
        {
            try
            {
                var request = context.Request;

                #region Request body

                var requestBodyString = string.Empty;
                var contentType = request.ContentType ?? string.Empty;
                var recordRequestBody = request.ContentLength.HasValue && request.ContentLength.Value > 0
                    && options.RecordRequestBody
                    && !contentType.Contains(fileDataContentType, StringComparison.OrdinalIgnoreCase)
                    && (options.RecordBodyContentTypes?.Any(ct => ct.Contains(contentType, StringComparison.OrdinalIgnoreCase)) ?? false);
                if (recordRequestBody)
                {
                    request.EnableBuffering();
                    var limitSize = options.RequestBodyLimitSize;
                    limitSize = limitSize < 0 ? int.MaxValue : limitSize;
                    using var reader = new StreamReader(request.Body, options.Encoding, leaveOpen: true);
                    {
                        if (limitSize >= 0 && request.Body.Length > limitSize)
                        {
                            var readBuffer = new char[limitSize];
                            var readLength = await reader.ReadAsync(readBuffer, 0, limitSize).ConfigureAwait(false);
                            if (readLength > 0)
                            {
                                var bodyStringBuilder = new StringBuilder();
                                bodyStringBuilder.Append(readBuffer, 0, readLength);
                                bodyStringBuilder.Append("...");
                                requestBodyString = bodyStringBuilder.ToString();
                            }
                        }
                        else
                        {
                            requestBodyString = await reader.ReadToEndAsync().ConfigureAwait(false);
                        }
                        request.Body.Seek(0, SeekOrigin.Begin);
                    }
                }

                #endregion

                traceMessage.Request = new SixnetHttpRequestInfo()
                {
                    Path = $"{request.Path}{request.QueryString.Value}",
                    Method = request.Method,
                    Body = requestBodyString,
                    Headers = options.RecordHeader ? request.Headers : null,
                    ContentLength = request.ContentLength ?? 0,
                    ContentType = contentType
                };
            }
            catch (Exception ex)
            {
                traceMessage.Exception = new SixnetHttpExceptionInfo()
                {
                    OriginalException = ex,
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };
                throw;
            }
        }

        /// <summary>
        /// Build response info 
        /// </summary>
        /// <param name="response"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        private async Task BuildResponseInfoAsync(HttpContext context, SixnetHttpRequestTraceMessage traceMessage, SixnetHttpLogOptions options)
        {
            var response = context.Response;
            var originalBody = response.Body;
            try
            {
                #region Response body

                var responseBodyString = string.Empty;
                var contentType = response.ContentType ?? string.Empty;
                var recordResponsetBody = options.RecordResponseBody
                    && !contentType.Contains(fileDataContentType, StringComparison.OrdinalIgnoreCase)
                    && (options.RecordBodyContentTypes?.Any(ct => ct.Contains(contentType, StringComparison.OrdinalIgnoreCase)) ?? false);
                if (recordResponsetBody)
                {
                    using (var currentBody = new MemoryStream())
                    {
                        context.Response.Body = currentBody;
                        await _next(context).ConfigureAwait(false);
                        currentBody.Seek(0, SeekOrigin.Begin);
                        var limitSize = options.ResponseBodyLimitSize;
                        limitSize = limitSize < 0 ? int.MaxValue : limitSize;
                        using (var reader = new StreamReader(currentBody, options.Encoding, leaveOpen: true))
                        {
                            if (limitSize >= 0 && currentBody.Length > limitSize)
                            {
                                var readBuffer = new char[limitSize];
                                var readLength = await reader.ReadAsync(readBuffer, 0, limitSize).ConfigureAwait(false);
                                if (readLength > 0)
                                {
                                    var bodyStringBuilder = new StringBuilder();
                                    bodyStringBuilder.Append(readBuffer, 0, readLength);
                                    bodyStringBuilder.Append("...");
                                    responseBodyString = bodyStringBuilder.ToString();
                                }
                            }
                            else
                            {
                                responseBodyString = await reader.ReadToEndAsync().ConfigureAwait(false);
                            }
                            currentBody.Seek(0, SeekOrigin.Begin);
                            await currentBody.CopyToAsync(originalBody).ConfigureAwait(false);
                        }
                    }
                }
                else
                {
                    await _next(context).ConfigureAwait(false);
                }

                #endregion

                traceMessage.Response = new SixnetHttpResponseInfo
                {
                    Body = responseBodyString,
                    ContentLength = response.ContentLength ?? 0,
                    StatusCode = response.StatusCode,
                    ContentType = contentType,
                    Headers = options.RecordHeader ? response.Headers : null
                };
            }
            catch (Exception ex)
            {
                traceMessage.Exception = new SixnetHttpExceptionInfo()
                {
                    OriginalException = ex,
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };
                throw;
            }
            finally
            {
                response.Body = originalBody;
            }
        }
    }
}
