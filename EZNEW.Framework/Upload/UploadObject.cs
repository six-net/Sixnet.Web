using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Framework.Upload
{
    /// <summary>
    /// upload object
    /// </summary>
    public class UploadObject
    {
        /// <summary>
        /// upload object name
        /// </summary>
        public string Name
        {
            get; set;
        }

        /// <summary>
        /// upload option
        /// </summary>
        public UploadOption UploadOption
        {
            get; set;
        }
    }
}
