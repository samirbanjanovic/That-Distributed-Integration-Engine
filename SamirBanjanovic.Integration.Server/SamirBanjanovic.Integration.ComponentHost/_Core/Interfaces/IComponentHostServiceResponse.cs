using System;
using System.Collections.Generic;
using System.Text;

namespace OnTrac.Integration.ComponentHost._Core.Interfaces
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
