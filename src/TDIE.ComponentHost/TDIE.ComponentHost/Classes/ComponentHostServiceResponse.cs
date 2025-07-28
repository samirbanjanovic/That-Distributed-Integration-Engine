using TDIE.ComponentHost.Core;

namespace TDIE.ComponentHost.Classes
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
