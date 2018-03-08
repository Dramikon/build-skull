using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BuildSkull.Contracts;
using BuildSkull.TFS;
using BuildSkull.ViewModel;

namespace BuildSkull_UnitTests.ViewModelTests
{
    [TestClass]
    public class BuildSkullVMTests
    {
        [TestMethod]
        public void GetBuildTreeReqTest()
        {
            IBuildFacade facade = new TFSBuildFacade();
            facade.LogIn(
                new RepositoryUser() { Name = "", Password = "" },
                new RepositoryInfo() { ProjectName = "VSBuildAddin", ServerUrl = new Uri("https://restcode.visualstudio.com") });


            var builds = ((TFSBuildFacade)facade).GetBuildList();

            var vm = new BuildSkullVM();
            var tree = vm.GetBuildTree(builds);

            Assert.AreNotEqual(0, tree.Length);
        }
    }
}
