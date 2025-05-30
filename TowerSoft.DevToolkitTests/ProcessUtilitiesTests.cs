using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerSoft.DevToolkit.Models;
using TowerSoft.DevToolkit.Utilities;

namespace TowerSoft.DevToolkitTests {
    [TestClass]
    public class ProcessUtilitiesTests {
        [TestMethod]
        public async Task GetOutput_Echo_ShouldOutputEchoStatus() {
            ProcessResult result = await ProcessUtilities.GetOutput("cmd", "/c echo");
            Assert.AreEqual("ECHO is on.", result.Output.Trim());
            Assert.AreEqual(string.Empty, result.Error);
        }

        [TestMethod]
        public async Task GetOutput_Pint_ShouldReturnError() {
            ProcessResult result = await ProcessUtilities.GetOutput("cmd", "/c dir /invalidArg");
            Assert.AreEqual("Invalid switch - \"invalidArg\".", result.Error.Trim());
            Assert.AreEqual(string.Empty, result.Output);
        }
    }
}
