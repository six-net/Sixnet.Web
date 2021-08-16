using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EZNEW.Web.API
{
    /// <summary>
    /// Defines api response
    /// </summary>
    public class ApiResponse
    {
        /// <summary>
        /// Gets or sets the response code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the response message
        /// </summary>
        [JsonPropertyName("msg")]
        public string Message { get; set; }

        /// <summary>
        /// Get a success response
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns></returns>
        public static ApiResponse Successful(string message = "")
        {
            return Create(ApiConstants.ResponseCodes.RC200, message);
        }

        /// <summary>
        /// Get a response object
        /// </summary>
        /// <param name="code">Response code</param>
        /// <param name="message">Response message</param>
        /// <returns></returns>
        public static ApiResponse Create(string code, string message = "")
        {
            _ = string.IsNullOrWhiteSpace(message) && ApiConstants.ResponseMessages.TryGetValue(code, out message);
            return new ApiResponse()
            {
                Code = code,
                Message = message
            };
        }
    }

    /// <summary>
    /// Defines api response
    /// </summary>
    /// <typeparam name="T">T</typeparam>
    public class ApiResponse<T> : ApiResponse
    {
        /// <summary>
        /// Gets or sets the data
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Get a success response
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="message">Message</param>
        /// <returns></returns>
        public static ApiResponse<T> Successful(T data, string message = "")
        {
            return Create(data, ApiConstants.ResponseCodes.RC200, message);
        }

        /// <summary>
        /// Get a response object
        /// </summary>
        /// <param name="code">Response code</param>
        /// <param name="message">Response message</param>
        /// <returns></returns>
        public static ApiResponse<T> Create(T data, string code, string message = "")
        {
            _ = string.IsNullOrWhiteSpace(message) && ApiConstants.ResponseMessages.TryGetValue(code, out message);
            return new ApiResponse<T>()
            {
                Data = data,
                Code = code,
                Message = message
            };
        }
    }
}
