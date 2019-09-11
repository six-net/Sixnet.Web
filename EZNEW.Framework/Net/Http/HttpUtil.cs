using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using EZNEW.Framework.Extension;
using EZNEW.Framework.Serialize;
using EZNEW.Framework.Upload;

namespace EZNEW.Framework.Net.Http
{
    /// <summary>
    /// http util
    /// </summary>
    public static class HttpUtil
    {
        #region Http call

        /// <summary>
        /// send a http request and get a string response value
        /// </summary>
        /// <param name="option">request options</param>
        /// <returns></returns>
        public static async Task<string> HttpResponseStringAsync(HttpRequestOption option)
        {
            var response = await HttpCallAsync(option).ConfigureAwait(false);
            using (var streamReader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                return await streamReader.ReadToEndAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// send a http request and receive response value
        /// </summary>
        /// <param name="option">request options</param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> HttpCallAsync(HttpRequestOption option)
        {
            if (option == null || option.RequestMessage == null)
            {
                throw new ArgumentException(nameof(option));
            }
            if (option.RequestMessage.Method == HttpMethod.Get)
            {
                return await HttpGetAsync(option).ConfigureAwait(false);
            }
            if (option.RequestMessage.Method == HttpMethod.Post)
            {
                return await HttpPostAsync(option).ConfigureAwait(false);
            }
            if (option.RequestMessage.Method == HttpMethod.Delete)
            {
                return await HttpDeleteAsync(option).ConfigureAwait(false);
            }
            //if (option.RequestMessage.Method == HttpMethod.Patch)
            //{
            //    return await HttpPatchAsync(option).ConfigureAwait(false);
            //}
            if (option.RequestMessage.Method == HttpMethod.Put)
            {
                return await HttpPutAsync(option).ConfigureAwait(false);
            }
            return new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("")
            };
        }

        /// <summary>
        /// send a http request by Get
        /// </summary>
        /// <param name="option">option</param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> HttpGetAsync(HttpRequestOption option)
        {
            if (option == null)
            {
                throw new ArgumentNullException(nameof(option));
            }
            using (var httpHandler = new HttpClientHandler() { UseCookies = option.UseCookie })
            {
                using (var httpClient = new HttpClient(httpHandler))
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(option.TimeOutSeconds);
                    string url = option.RequestMessage?.RequestUri.AbsoluteUri;
                    if (string.IsNullOrWhiteSpace(url))
                    {
                        throw new ArgumentNullException("uri");
                    }

                    #region parameters

                    if (option.RequestParameters != null || option.RequestParameters.Count > 0)
                    {
                        List<string> queryParameters = new List<string>();
                        queryParameters.AddRange(option.RequestParameters.Select(c => string.Format("{0}={1}", c.Key, c.Value)));
                        url = url.Trim('?', '&');
                        url += (url.IndexOf('?') > 0 ? "&" : "?") + string.Join("&", queryParameters.ToArray());
                    }

                    #endregion

                    return await httpClient.GetAsync(url).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// send a http request by Post
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> HttpPostAsync(HttpRequestOption option)
        {
            if (option == null)
            {
                throw new ArgumentNullException(nameof(option));
            }
            using (var httpHandler = new HttpClientHandler() { UseCookies = option.UseCookie })
            {
                using (var httpClient = new HttpClient(httpHandler))
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(option.TimeOutSeconds);
                    string url = option.RequestMessage?.RequestUri.AbsoluteUri;
                    if (string.IsNullOrWhiteSpace(url))
                    {
                        throw new ArgumentNullException("uri");
                    }
                    var formContentData = new MultipartFormDataContent();

                    #region files

                    if (option.RequestFiles != null && option.RequestFiles.Count > 0)
                    {
                        int fileCount = 0;
                        foreach (var item in option.RequestFiles)
                        {
                            if (item.Value == null || item.Value.Length <= 0)
                            {
                                continue;
                            }
                            HttpContent content = new StreamContent(new MemoryStream(item.Value));
                            formContentData.Add(content, "file" + fileCount.ToString(), item.Key);
                            fileCount++;
                        }
                    }

                    #endregion

                    #region parameters

                    if (option.RequestParameters != null && option.RequestParameters.Count > 0)
                    {
                        foreach (string key in option.RequestParameters.Keys)
                        {
                            var stringContent = new StringContent(option.RequestParameters[key]);
                            formContentData.Add(stringContent, key);
                        }
                    }

                    #endregion

                    return await httpClient.PostAsync(url, formContentData).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// send a http request by DELETE
        /// </summary>
        /// <param name="option">option</param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> HttpDeleteAsync(HttpRequestOption option)
        {
            if (option == null)
            {
                throw new ArgumentNullException(nameof(option));
            }
            using (var httpHandler = new HttpClientHandler() { UseCookies = option.UseCookie })
            {
                using (var httpClient = new HttpClient(httpHandler))
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(option.TimeOutSeconds);
                    string url = option.RequestMessage?.RequestUri.AbsoluteUri;
                    if (string.IsNullOrWhiteSpace(url))
                    {
                        throw new ArgumentNullException("uri");
                    }

                    #region parameters

                    if (option.RequestParameters != null || option.RequestParameters.Count > 0)
                    {
                        List<string> queryParameters = new List<string>();
                        queryParameters.AddRange(option.RequestParameters.Select(c => string.Format("{0}={1}", c.Key, c.Value)));
                        url = url.Trim('?', '&');
                        url += (url.IndexOf('?') > 0 ? "&" : "?") + string.Join("&", queryParameters.ToArray());
                    }

                    #endregion

                    return await httpClient.DeleteAsync(url).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// send a http request by Patch
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> HttpPatchAsync(HttpRequestOption option)
        {
            if (option == null)
            {
                throw new ArgumentNullException(nameof(option));
            }
            using (var httpHandler = new HttpClientHandler() { UseCookies = option.UseCookie })
            {
                using (var httpClient = new HttpClient(httpHandler))
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(option.TimeOutSeconds);
                    string url = option.RequestMessage?.RequestUri.AbsoluteUri;
                    if (string.IsNullOrWhiteSpace(url))
                    {
                        throw new ArgumentNullException("uri");
                    }
                    var formContentData = new MultipartFormDataContent();

                    #region files

                    if (option.RequestFiles != null && option.RequestFiles.Count > 0)
                    {
                        int fileCount = 0;
                        foreach (var item in option.RequestFiles)
                        {
                            if (item.Value == null || item.Value.Length <= 0)
                            {
                                continue;
                            }
                            HttpContent content = new StreamContent(new MemoryStream(item.Value));
                            formContentData.Add(content, "file" + fileCount.ToString(), item.Key);
                            fileCount++;
                        }
                    }

                    #endregion

                    #region parameters

                    if (option.RequestParameters != null && option.RequestParameters.Count > 0)
                    {
                        foreach (string key in option.RequestParameters.Keys)
                        {
                            var stringContent = new StringContent(option.RequestParameters[key]);
                            formContentData.Add(stringContent, key);
                        }
                    }

                    #endregion

                    return await Task.FromResult(new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.BadGateway
                    }); //httpClient.PatchAsync(url, formContentData).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// send a http request by Put
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> HttpPutAsync(HttpRequestOption option)
        {
            if (option == null)
            {
                throw new ArgumentNullException(nameof(option));
            }
            using (var httpHandler = new HttpClientHandler() { UseCookies = option.UseCookie })
            {
                using (var httpClient = new HttpClient(httpHandler))
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(option.TimeOutSeconds);
                    string url = option.RequestMessage?.RequestUri.AbsoluteUri;
                    if (string.IsNullOrWhiteSpace(url))
                    {
                        throw new ArgumentNullException("uri");
                    }
                    var formContentData = new MultipartFormDataContent();

                    #region files

                    if (option.RequestFiles != null && option.RequestFiles.Count > 0)
                    {
                        int fileCount = 0;
                        foreach (var item in option.RequestFiles)
                        {
                            if (item.Value == null || item.Value.Length <= 0)
                            {
                                continue;
                            }
                            HttpContent content = new StreamContent(new MemoryStream(item.Value));
                            formContentData.Add(content, "file" + fileCount.ToString(), item.Key);
                            fileCount++;
                        }
                    }

                    #endregion

                    #region parameters

                    if (option.RequestParameters != null && option.RequestParameters.Count > 0)
                    {
                        foreach (string key in option.RequestParameters.Keys)
                        {
                            var stringContent = new StringContent(option.RequestParameters[key]);
                            formContentData.Add(stringContent, key);
                        }
                    }

                    #endregion

                    return await httpClient.PutAsync(url, formContentData).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// 请求Json数据
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="jsonData">json数据</param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> HttpPostJsonAsync(string url, string jsonData = "")
        {
            using (var httpClient = new HttpClient())
            {
                var content = new StringContent(jsonData, Encoding.UTF8);
                content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");
                var response = await httpClient.PostAsync(url, content);
                return response;
            }
        }
        /// <summary>
        /// 请求Json数据
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="data">请求数据</param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> HttpPostJsonAsync(string url, object data = null)
        {
            string jsonData = string.Empty;
            if (data != null)
            {
                jsonData = JsonSerialize.ObjectToJson(data);
            }
            return await HttpPostJsonAsync(url, jsonData).ConfigureAwait(false);
        }

        #endregion

        #region download file

        /// <summary>
        /// download file
        /// </summary>
        /// <param name="url">url</param>
        /// <returns>file byte</returns>
        public static async Task<byte[]> DownloadDataAsync(string url)
        {
            return await new WebClient().DownloadDataTaskAsync(url).ConfigureAwait(false);
        }

        #endregion

        #region upload file

        /// <summary>
        /// upload file
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="files">files</param>
        /// <param name="parameters">parameters</param>
        /// <returns>upload result</returns>
        public static async Task<UploadResult> UploadAsync(string url, Dictionary<string, byte[]> files, Dictionary<string, string> parameters = null)
        {
            #region verify args

            if (url.IsNullOrEmpty())
            {
                return new UploadResult()
                {
                    Success = false,
                    ErrorMessage = "url is null or empty"
                };
            }
            if (files == null || files.Count <= 0)
            {
                return new UploadResult()
                {
                    Success = false,
                    ErrorMessage = "not set any files to upload"
                };
            }

            #endregion

            string responseVal = await HttpResponseStringAsync(new HttpRequestOption()
            {
                RequestFiles = files,
                RequestMessage = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(url)
                },
                RequestParameters = parameters,
                UseCookie = true
            }).ConfigureAwait(false);
            var result = JsonSerialize.JsonToObject<UploadResult>(responseVal);
            result?.Files?.ForEach(f =>
            {
                f.Target = UploadTarget.Remote;
            });
            return result;
        }

        /// <summary>
        /// upload file
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="file">file</param>
        /// <param name="parameters">parameters</param>
        /// <returns>upload result</returns>
        public static async Task<UploadResult> UploadAsync(string url, byte[] file, object parameters)
        {
            if (file == null || file.Length <= 0)
            {
                return new UploadResult()
                {
                    ErrorMessage = "not set any files to upload",
                    Success = false
                };
            }
            Dictionary<string, string> parameterDic = null;
            if (parameters != null)
            {
                parameterDic = parameters.ObjectToStringDcitionary();
            }
            return await UploadAsync(url, new Dictionary<string, byte[]>() { { "file1", file } }, parameterDic).ConfigureAwait(false);
        }

        #endregion
    }
}
