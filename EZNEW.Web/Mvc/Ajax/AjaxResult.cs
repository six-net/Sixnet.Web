using System;
using EZNEW.Response;

namespace EZNEW.Web.Mvc
{
    public class AjaxResult
    {
        #region fields

        protected const string successDefaultMsg = "success";

        protected const string failedDefaultMsg = "failed";

        #endregion

        #region Propertys

        /// <summary>
        /// Success
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Data
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// need authentication
        /// </summary>
        public bool NeedAuthentication
        {
            get; set;
        }

        /// <summary>
        /// refresh current page when return success result
        /// </summary>
        public bool SuccessRefresh
        {
            get; set;
        }

        /// <summary>
        /// close current page when return success result
        /// </summary>
        public bool SuccessClose
        {
            get; set;
        }

        #endregion

        #region Methods

        #region get a success result

        /// <summary>
        /// get a success result
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="code">code</param>
        /// <param name="data">data</param>
        /// <returns>success result</returns>
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

        #region get a failed result

        /// <summary>
        /// get a failed result
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="code">code</param>
        /// <param name="data">data</param>
        /// <returns>failed result</returns>
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
        /// get a failed result
        /// </summary>
        /// <param name="ex">exception</param>
        /// <returns>failed</returns>
        public static AjaxResult FailedResult(Exception ex)
        {
            AjaxResult result = new AjaxResult();
            result.Success = false;
            result.Message = ex.Message;
            return result;
        }

        #endregion

        #region copy from result

        /// <summary>
        /// copy from result
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Copy from resultcopy from result
        /// </summary>
        /// <param name="result">Result</param>
        /// <returns>Return ajax result</returns>
        public static AjaxResult CopyFromResult<T>(Result<T> result)
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
