using System;
using System.Threading;

namespace TDIE.Components.NodeManager.Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            var x = new MasterComponent(null, null, null);
            x.StartAsync(CancellationToken.None).GetAwaiter().GetResult();
            Console.ReadLine();
        }
    }
}
