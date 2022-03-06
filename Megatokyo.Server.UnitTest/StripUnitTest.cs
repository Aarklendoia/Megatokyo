using IG.MaRH.ClientAPI.UnitTest.Server;
using Megatokyo.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Megatokyo.Server.UnitTest
{
    [TestClass]
    public class StripUnitTest
    {
        [TestMethod]
        public async Task StripsAllAsyncTestMethod()
        {
            HttpClient client = TestServer.GetClient();
            IMegatokyoClient service = new MegatokyoClient(client);
            ICollection<StripOutputDTO> result = await service.GetAllStripsAsync();
            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod]
        [DynamicData(nameof(StripCategoryData), DynamicDataSourceType.Method)]
        public async Task GetByCategoryTestmethod(string category, int count)
        {
            HttpClient client = TestServer.GetClient();
            IMegatokyoClient service = new MegatokyoClient(client);
            ICollection<StripOutputDTO> result = await service.GetCategoryStripsAsync(category);
            Assert.IsTrue(result.Count == count);
        }

        private static IEnumerable<object[]> StripCategoryData()
        {
            yield return new object[] { "C-0", 95 };
            yield return new object[] { "C-10", 110 };
        }

        [TestMethod]
        [DynamicData(nameof(StripNumberData), DynamicDataSourceType.Method)]
        public async Task GetByCategoryTestmethod(int number)
        {
            HttpClient client = TestServer.GetClient();
            IMegatokyoClient service = new MegatokyoClient(client);
            StripOutputDTO result = await service.GetStripAsync(number);
            Assert.IsTrue(result.Number == number);
        }

        private static IEnumerable<object[]> StripNumberData()
        {
            yield return new object[] { 1 };
            yield return new object[] { 572 };
            yield return new object[] { 1024 };
        }
    }
}
