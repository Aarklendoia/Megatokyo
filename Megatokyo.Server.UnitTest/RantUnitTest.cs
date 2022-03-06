using IG.MaRH.ClientAPI.UnitTest.Server;
using Megatokyo.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Megatokyo.Server.UnitTest
{
    [TestClass]
    public class RantUnitTest
    {
        [TestMethod]
        public void RantsAllAsyncTestMethod()
        {
            HttpClient client = TestServer.GetClient();
            IMegatokyoClient service = new MegatokyoClient(client);
            ICollection<RantOutputDTO> result = service.GetAllRantsAsync().GetAwaiter().GetResult();
            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod]
        [DynamicData(nameof(RantNumberData), DynamicDataSourceType.Method)]
        public void GetByCategoryTestmethod(int number)
        {
            HttpClient client = TestServer.GetClient();
            IMegatokyoClient service = new MegatokyoClient(client);
            RantOutputDTO result = service.GetRantAsync(number).GetAwaiter().GetResult();
            Assert.IsTrue(result.Number == number);
        }

        private static IEnumerable<object[]> RantNumberData()
        {
            yield return new object[] { 1 };
            yield return new object[] { 572 };
            yield return new object[] { 1024 };
        }
    }
}
