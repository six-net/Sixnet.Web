using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Linq;

namespace EZNEW.Framework.Upload.Config
{
    /// <summary>
    /// upload config
    /// </summary>
    public class UploadConfig
    {
        /// <summary>
        /// default upload option
        /// </summary>
        public UploadOption Default
        {
            get; set;
        }

        /// <summary>
        /// upload option items
        /// </summary>
        public List<UploadObject> UploadObjects
        {
            get; set;
        } = new List<UploadObject>();
    }
}
