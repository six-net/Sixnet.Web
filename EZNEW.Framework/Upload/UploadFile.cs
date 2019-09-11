using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Framework.Upload
{
    /// <summary>
    /// upload file
    /// </summary>
    public class UploadFile
    {
        /// <summary>
        /// upload object name
        /// </summary>
        public string UploadObjectName
        {
            get; set;
        }

        /// <summary>
        /// file name
        /// </summary>
        public string FileName
        {
            get; set;
        }

        /// <summary>
        /// save folder
        /// </summary>
        public string Folder
        {
            get; set;
        }

        /// <summary>
        /// file suffix
        /// </summary>
        public string Suffix
        {
            get; set;
        }

        /// <summary>
        /// rename file name
        /// </summary>
        public bool Rename
        {
            get; set;
        }
    }
}
