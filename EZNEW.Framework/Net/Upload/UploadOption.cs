using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Framework.Net.Upload
{
    /// <summary>
    /// UploadOption
    /// </summary>
    public class UploadOption
    {
        /// <summary>
        /// 上传文件
        /// </summary>
        public List<UploadFile> Files
        {
            get;set;
        }

        /// <summary>
        /// 请求参数关键字
        /// </summary>
        public const string REQUEST_KEY = "upload_file_option";
    }
}
