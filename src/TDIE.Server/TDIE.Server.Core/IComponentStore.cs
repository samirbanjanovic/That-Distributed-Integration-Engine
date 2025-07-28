﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TDIE.Server.Core
{
    public interface IComponentStore
    {
        Task AddOrUpdateComponentMetadataAsync(IComponentMetadata componentMetadata);
        IReadOnlyDictionary<Guid, IComponentMetadata> GetComponentMetadataTable();
    }
}
