﻿using System;
using System.Collections.Generic;
using System.Text;

namespace OnTrac.Integration.Core
{
    public interface IComponentSettings
    {
        string Name { get; }

        long Id { get; }

        IReadOnlyDictionary<string, string> Properties { get; }
    }
}