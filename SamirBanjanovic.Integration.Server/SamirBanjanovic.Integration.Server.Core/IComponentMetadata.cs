using System;
using System.Collections.Generic;
using System.Text;

namespace OnTrac.Integration.Server.Core
{
    public interface IComponentMetadata
    {
        Guid Id { get; set; }

        string ComponentDirectory { get; set; }

        IComponentPackageConfiguration ComponentPackageConfiguration { get; set; }
    }
}
