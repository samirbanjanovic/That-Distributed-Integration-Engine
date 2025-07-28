using System;
using System.Collections.Generic;
using System.Text;

namespace TDIE.ComponentHost.Core
{
    public interface IComponentHostServiceResponse
    {
        MemberState State { get; set; }

        string Message { get; set; }
    }

    public interface IComponentHostServiceResponse<T>
        : IComponentHostServiceResponse
        where T : class
    {
        T Result { get; set; }
    }
}
