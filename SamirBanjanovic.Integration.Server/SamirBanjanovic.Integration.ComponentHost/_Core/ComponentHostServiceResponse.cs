using System;
using System.Collections.Generic;
using System.Text;
using OnTrac.Integration.ComponentHost._Core.Interfaces;

namespace OnTrac.Integration.ComponentHost._Core
{
    public class ComponentHostServiceResponse
        : IComponentHostServiceResponse
    {
        public MemberState State { get; set; }
        public string Message { get; set; }
    }

    public class ComponentHostServiceResponse<T>
        : ComponentHostServiceResponse
        , IComponentHostServiceResponse<T>
        where T : class
    {
        public T Result { get; set; }
    }
}
