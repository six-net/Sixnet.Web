using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Framework.Response
{
    /// <summary>
    /// operation result
    /// </summary>
    public class Result
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
        public static Result SuccessResult(string message = "", string code = "", object data = null)
        {
            Result result = new Result();
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
        public static Result FailedResult(string message = "", string code = "", object data = null)
        {
            Result result = new Result();
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
        public static Result FailedResult(Exception ex)
        {
            Result result = new Result();
            result.Success = false;
            result.Message = ex.Message;
            return result;
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// result of generics version
    /// </summary>
    /// <typeparam name="T">data type</typeparam>
    public class Result<T> : Result
    {
        #region Propertys

        /// <summary>
        /// get or set data object
        /// </summary>
        public T Object
        {
            get
            {
                return (T)Data;
            }
            set
            {
                Data = value;
            }
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
        public static new Result<T> SuccessResult(string message = "", string code = "", object data = null)
        {
            Result<T> result = new Result<T>();
            result.Success = true;
            result.Code = code;
            result.Data = data;
            result.Message = string.IsNullOrWhiteSpace(message) ? successDefaultMsg : message;
            return result;
        }

        #endregion

        #region get failed success result

        /// <summary>
        /// get a failed result
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="code">code</param>
        /// <param name="data">data</param>
        /// <returns>failed result</returns>
        public static new Result<T> FailedResult(string message = "", string code = "", object data = null)
        {
            Result<T> result = new Result<T>();
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
        /// <returns>failed result</returns>
        public static new Result<T> FailedResult(Exception ex)
        {
            Result<T> result = new Result<T>();
            result.Success = false;
            result.Message = ex.Message;
            return result;
        }

        #endregion

        #endregion
    }
}
