using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Framework.Upload
{
    /// <summary>
    /// Upload Option
    /// </summary>
    public class UploadOption
    {
        /// <summary>
        /// remote upload
        /// </summary>
        public bool Remote
        {
            get; set;
        }

        /// <summary>
        /// save path
        /// </summary>
        public string SavePath
        {
            get; set;
        }

        /// <summary>
        /// save to content root folder
        /// </summary>
        public bool SaveToContentRoot
        {
            get; set;
        } = true;

        /// <summary>
        /// content root folder path
        /// </summary>
        public string ContentRootPath
        {
            get; set;
        } = "wwwroot";

        /// <summary>
        /// remote configs
        /// </summary>
        public List<RemoteOption> RemoteConfigs
        {
            get; set;
        }

        /// <summary>
        /// upload server choice pattern
        /// </summary>
        public RemoteServerChoicePattern RemoteServerChoicePattern
        {
            get; set;
        } = RemoteServerChoicePattern.Random;

        /// <summary>
        /// get remote option
        /// </summary>
        /// <returns></returns>
        public RemoteOption GetRemoteOption()
        {
            if (RemoteConfigs == null || RemoteConfigs.Count <= 0)
            {
                return null;
            }
            int serverCount = RemoteConfigs.Count;
            if (serverCount == 1)
            {
                return RemoteConfigs[0];
            }
            RemoteOption remoteOption = null;
            switch (RemoteServerChoicePattern)
            {
                case RemoteServerChoicePattern.First:
                    remoteOption = RemoteConfigs[0];
                    break;
                case RemoteServerChoicePattern.Latest:
                    remoteOption = RemoteConfigs[RemoteConfigs.Count - 1];
                    break;
                default:
                    var random = new Random();
                    int ranIndex = random.Next(0, serverCount);
                    remoteOption = RemoteConfigs[ranIndex];
                    break;
            }
            return remoteOption;
        }
    }
}
