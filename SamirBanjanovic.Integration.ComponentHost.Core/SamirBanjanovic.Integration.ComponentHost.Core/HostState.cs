using System;
using System.Collections.Generic;
using System.Text;

namespace OnTrac.Integration.ComponentHost.Core
{
    public enum HostState
    {
        AwaitingConfiguration = -1,
        Configured = 0,
        Started = 1,
        Stopped = 2,
        Errored = 3,
        Destroyed = 99
    }
}
