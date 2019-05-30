using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Framework.Net.Upload
{
    [Serializable]
    public class UploadFileResult
    {
        string _relativePath = string.Empty;

        string _fullPath = string.Empty;

        #region Propertys 

        /// <summary>
        /// Suffix
        /// </summary>
        public string Suffix
        {
            get;
            set;
        }

        /// <summary>
        /// OriginalFileName
        /// </summary>
        public string OriginalFileName
        {
            get; set;
        }

        /// <summary>
        /// FileName
        /// </summary>
        public string FileName
        {
            get;
            set;
        }

        /// <summary>
        /// RelativePath
        /// </summary>
        public string RelativePath
        {
            get
            {
                return _relativePath;
            }
            set
            {
                _relativePath = value.Replace("\\", "/");
            }
        }

        /// <summary>
        /// FullPath
        /// </summary>
        public string FullPath
        {
            get
            {
                return _fullPath;
            }
            set
            {
                _fullPath = value.Replace("\\", "/");
            }
        }

        /// <summary>
        /// 上传时间
        /// </summary>
        public DateTime UploadDate
        {
            get;set;
        }

        #endregion
    }
}
