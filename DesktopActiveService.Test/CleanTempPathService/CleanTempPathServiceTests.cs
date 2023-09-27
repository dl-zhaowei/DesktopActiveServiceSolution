using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DesktopActiveService.Tests
{
    [TestClass]
    public class CleanTempPathServiceTests
    {
        [TestMethod]
        public void CleanTempPathTest()
        {
            var service = new CleanTempPathService();
            service.CleanTempPath();
        }
    }
}
