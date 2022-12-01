using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonTools.Utils.Tests;

[TestClass()]
public class NetworkUtilsTests {
    [TestMethod()]
    public void GetLocalIpAddress() {
        Console.WriteLine($"local ip addresss is {NetworkUtils.GetLocalIpAddress()}");
    }

    [TestMethod()]
    public void GetInUsePorts() {
        Console.WriteLine("port in use:");
        foreach (var port in NetworkUtils.GetInUsePorts()) {
            Console.WriteLine(port);
        }
    }
}