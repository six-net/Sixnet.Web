using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Framework.Net.Upload
{
    /// <summary>
    /// Upload Result
    /// </summary>
    [Serializable]
    public class UploadResult
    {
        #region Propertys

        /// <summary>
        /// get or set whether success
        /// </summary>
        public bool Success
        {
            get;
            set;
        }

        /// <summary>
        /// get or set errormsg
        /// </summary>
        public string ErrorMsg
        {
            get;
            set;
        }

        /// <summary>
        /// get or set code
        /// </summary>
        public string Code
        {
            get; set;
        }

        /// <summary>
        /// get or set upload file list
        /// </summary>
        public List<UploadFileResult> FileInfoList
        {
            get; set;
        }

        #endregion

        public UploadResult Combine(params UploadResult[] results)
        {
            if (results == null || results.Length <= 0)
            {
                return this;
            }
            foreach (var result in results)
            {
                if (!result.Success)
                {
                    Success = result.Success;
                    ErrorMsg = result.ErrorMsg;
                    Code = result.Code;
                }
                if (result.FileInfoList != null)
                {
                    FileInfoList = FileInfoList ?? new List<UploadFileResult>();
                    FileInfoList.AddRange(result.FileInfoList);
                }
            }
            return this;
        }
    }
}
