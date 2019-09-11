using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Framework.Upload
{
    /// <summary>
    /// upload target
    /// </summary>
    public enum UploadTarget
    {
        Local = 2,
        Remote = 4
    }

    /// <summary>
    /// remote server choice pattern
    /// </summary>
    public enum RemoteServerChoicePattern
    {
        First = 2,
        Latest = 4,
        Random = 8
    }
}
