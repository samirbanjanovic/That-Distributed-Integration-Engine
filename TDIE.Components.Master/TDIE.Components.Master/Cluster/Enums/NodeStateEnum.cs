using System;
using System.Collections.Generic;
using System.Text;

namespace TDIE.Components.Master.Cluster.Enums
{
    public enum NodeStateEnum
    {
        NodeSynchronizedWithCluster = 0,
        NodeRemovedFromSystem,
        FailedToAcquireDistributedLock = 99
    }

}
