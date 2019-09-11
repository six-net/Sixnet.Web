using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Framework.Upload
{
    /// <summary>
    /// remote option
    /// </summary>
    public class RemoteOption
    {
        /// <summary>
        /// upload action
        /// </summary>
        public string UploadAction
        {
            get; set;
        } = "upfile";

        /// <summary>
        /// file list action
        /// </summary>
        public string FileListAction
        {
            get; set;
        } = "filelist";

        /// <summary>
        /// server
        /// </summary>
        public string Server
        {
            get; set;
        }

        /// <summary>
        /// get upload url
        /// </summary>
        /// <returns></returns>
        public string GetUploadUrl()
        {
            if (string.IsNullOrWhiteSpace(UploadAction))
            {
                return Server;
            }
            return string.Format("{0}/{1}", Server.Trim('/'), UploadAction);
        }

        /// <summary>
        /// get file list access url
        /// </summary>
        /// <returns></returns>
        public string GetFileListUrl()
        {
            if (string.IsNullOrWhiteSpace(FileListAction))
            {
                return Server;
            }
            return string.Format("{0}/{1}", Server.Trim('/'), FileListAction);
        }
    }
}
