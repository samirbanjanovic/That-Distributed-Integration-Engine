using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace OnTrac.Integration.Core
{
    public interface IComponent
        : IIntegrationExtension
    {       
        IComponentSettings Settings { get; }
    }
}
