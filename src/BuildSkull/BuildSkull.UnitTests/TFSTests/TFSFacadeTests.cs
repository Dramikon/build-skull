using BuildSkull.Contracts;
using BuildSkull.TFS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildSkull_UnitTests.TFSTests
{
    [TestClass]
    public class TFSFacadeTests
    {
        [TestMethod]
        public void GetBuildsTest()
        {
            IBuildFacade facade = new TFSBuildFacade();
            facade.LogIn(
                new RepositoryUser(){ Name = "", Password = "" },
                new RepositoryInfo() { ProjectName = "VSBuildAddin", ServerUrl = new Uri("https://restcode.visualstudio.com") });

            var watch = Stopwatch.StartNew();
            var builds = ((TFSBuildFacade)facade).GetBuildList();
            watch.Stop();
        }
    }
}
