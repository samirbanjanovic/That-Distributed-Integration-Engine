using System;
using System.Collections.Generic;
using System.Text;

namespace TDIE.Server.Core
{
    public interface IComponentMetadata
    {
        Guid Id { get; set; }

        string ComponentDirectory { get; set; }

        IComponentPackageConfiguration ComponentPackageConfiguration { get; set; }
    }
}
