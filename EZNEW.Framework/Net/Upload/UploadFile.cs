using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Framework.Net.Upload
{
    /// <summary>
    /// 上传文件信息
    /// </summary>
    public class UploadFile
    {
        /// <summary>
        /// 文件类型
        /// </summary>
        public string FileType
        {
            get;set;
        }

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName
        {
            get;set;
        }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string Folder
        {
            get;set;
        }

        /// <summary>
        /// 文件后缀
        /// </summary>
        public string Suffix
        {
            get;set;
        }

        /// <summary>
        /// 文件重命名
        /// </summary>
        public bool Rename
        {
            get;set;
        }
    }
}
