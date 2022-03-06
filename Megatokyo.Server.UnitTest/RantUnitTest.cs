using IG.MaRH.ClientAPI.UnitTest.Server;
using Megatokyo.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Megatokyo.Server.UnitTest
{
    [TestClass]
    public class RantUnitTest
    {
        [TestMethod]
        public async Task RantsAllAsyncTestMethod()
        {
            HttpClient client = TestServer.GetClient();
            IMegatokyoClient service = new MegatokyoClient(client);
            ICollection<RantOutputDTO> result = await service.GetAllRantsAsync();
            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod]
        [DynamicData(nameof(RantNumberData), DynamicDataSourceType.Method)]
        public async Task GetByCategoryTestmethod(int number)
        {
            HttpClient client = TestServer.GetClient();
            IMegatokyoClient service = new MegatokyoClient(client);
            RantOutputDTO result = await service.GetRantAsync(number);
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
