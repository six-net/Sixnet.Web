using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Framework.Upload
{
    /// <summary>
    /// upload file result
    /// </summary>
    [Serializable]
    public class UploadFileResult
    {
        string relativePath = string.Empty;

        string fullPath = string.Empty;

        #region Propertys 

        /// <summary>
        /// file suffix
        /// </summary>
        public string Suffix
        {
            get;
            set;
        }

        /// <summary>
        /// original file name
        /// </summary>
        public string OriginalFileName
        {
            get; set;
        }

        /// <summary>
        /// file name
        /// </summary>
        public string FileName
        {
            get;
            set;
        }

        /// <summary>
        /// relative path
        /// </summary>
        public string RelativePath
        {
            get
            {
                return relativePath;
            }
            set
            {
                relativePath = value.Replace("\\", "/");
            }
        }

        /// <summary>
        /// full path
        /// </summary>
        public string FullPath
        {
            get
            {
                return fullPath;
            }
            set
            {
                fullPath = value.Replace("\\", "/");
            }
        }

        /// <summary>
        /// upload date
        /// </summary>
        public DateTimeOffset UploadDate
        {
            get; set;
        }

        /// <summary>
        /// upload target
        /// </summary>
        public UploadTarget Target
        {
            get; set;
        }

        #endregion
    }
}
