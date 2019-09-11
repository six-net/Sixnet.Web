using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Framework.Application
{
    /// <summary>
    /// Application Type
    /// </summary>
    public enum ApplicationType
    {
        WebSite = 110,
        WebAPI = 120,
        WinService = 130,
        Console = 140,
        WinForm = 150,
        AppService = 160
    }

    /// <summary>
    /// Application Status
    /// </summary>
    public enum ApplicationStatus
    {
        ready = 200,
        starting = 205,
        running = 210,
        paused = 215,
        stoped = 220,
        closed = 225
    }
}
