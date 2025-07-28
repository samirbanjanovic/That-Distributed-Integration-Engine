using System;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TDIE.Server.Core;
using TDIE.Core;

namespace TDIE.Server.Tests
{
    [TestClass]
    public class ComponentPackageTests
    {
        //[TestMethod]
        //public void TestAssemblyLoading()
        //{
        //    var fileInfo = new FileInfo(@"C:\repos\Integration Platform\TDIE.Components.FileWatcher\TDIE.Components.FileWatcher\bin\Debug\netstandard2.0\TDIE.Components.FileWatcher.dll");
        //    var assemblyExplorer = StaticResources.ServiceProvider.GetService<IAssemblyTypeResolver>();

        //    Type[] componentTypes = null;
        //    Assembly componentAssembly = null;

        //    bool success = assemblyExplorer.TryGetConcreteTypes(typeof(IComponent), fileInfo, out componentTypes, out componentAssembly);

        //    Assert.AreEqual(true, success);
        //    Assert.AreEqual(1, componentTypes.Length);
            
        //}
    }
}
