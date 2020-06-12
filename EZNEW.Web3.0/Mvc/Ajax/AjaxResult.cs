using System;
using EZNEW.Response;

namespace EZNEW.Web.Mvc
{
    /// <summary>
    /// Ajax result
    /// </summary>
    public class AjaxResult
    {
        #region Fields

        protected const string successDefaultMsg = "success";

        protected const string failedDefaultMsg = "failed";

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets whether execute successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the result code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the result message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the data
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Gets or sets whether need authentication
        /// </summary>
        public bool NeedAuthentication
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets whether refresh page when successful
        /// </summary>
        public bool RefreshPageWithSuccessful
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets whether close current page when successful
        /// </summary>
        public bool SuccessClose
        {
            get; set;
        }

        #endregion

        #region Methods

        #region Gets a success result

        /// <summary>
        /// Gets a success result
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="code">Code</param>
        /// <param name="data">Data</param>
        /// <returns>Return success result</returns>
        public static AjaxResult SuccessResult(string message = "", string code = "", object data = null)
        {
            AjaxResult result = new AjaxResult();
            result.Success = true;
            result.Code = code;
            result.Data = data;
            result.Message = string.IsNullOrWhiteSpace(message) ? successDefaultMsg : message;
            return result;
        }

        #endregion

        #region Gets a failed result

        /// <summary>
        /// Gets a failed result
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="code">Code</param>
        /// <param name="data">Data</param>
        /// <returns>Return failed result</returns>
        public static AjaxResult FailedResult(string message = "", string code = "", object data = null)
        {
            AjaxResult result = new AjaxResult();
            result.Success = false;
            result.Code = code;
            result.Data = data;
            result.Message = string.IsNullOrWhiteSpace(message) ? failedDefaultMsg : message;
            return result;
        }

        /// <summary>
        /// Gets a failed result
        /// </summary>
        /// <param name="exception">EException</param>
        /// <returns>Return failed result</returns>
        public static AjaxResult FailedResult(Exception exception)
        {
            AjaxResult result = new AjaxResult();
            result.Success = false;
            result.Message = exception.Message;
            return result;
        }

        #endregion

        #region Copy from result

        /// <summary>
        /// Copy from resultcopy from result
        /// </summary>
        /// <param name="result">Result</param>
        /// <returns>Return ajax result</returns>
        public static AjaxResult CopyFromResult(Result result)
        {
            if (result == null)
            {
                return FailedResult("");
            }
            return new AjaxResult()
            {
                Code = result.Code,
                Data = result.Data,
                Message = result.Message,
                Success = result.Success,
            };
        }

        #endregion

        #endregion
    }
}
