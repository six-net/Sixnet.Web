using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Framework.Upload
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
        public string ErrorMessage
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
        /// get or set upload files
        /// </summary>
        public List<UploadFileResult> Files
        {
            get; set;
        }

        /// <summary>
        /// empty upload result
        /// </summary>
        public static readonly UploadResult Empty = new UploadResult();

        #endregion

        /// <summary>
        /// combine upload result
        /// </summary>
        /// <param name="results">other upload results</param>
        /// <returns></returns>
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
                    ErrorMessage = result.ErrorMessage;
                    Code = result.Code;
                }
                if (result.Files != null)
                {
                    Files = Files ?? new List<UploadFileResult>();
                    Files.AddRange(result.Files);
                }
            }
            return this;
        }
    }
}
