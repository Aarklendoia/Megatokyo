using IG.MaRH.ClientAPI.UnitTest.Server;
using Megatokyo.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Megatokyo.Server.UnitTest
{
    [TestClass]
    public class ChapterUnitTest
    {
        [TestMethod]
        public async Task ChaptersAllAsyncTestMethod()
        {
            HttpClient client = TestServer.GetClient();
            IMegatokyoClient service = new MegatokyoClient(client);
            ICollection<ChapterOutputDTO> result = await service.GetAllChaptersAsync();
            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod]
        [DynamicData(nameof(ChaptersData), DynamicDataSourceType.Method)]
        public async Task GetByCategoryTestmethod(string category)
        {
            HttpClient client = TestServer.GetClient();
            IMegatokyoClient service = new MegatokyoClient(client);
            ChapterOutputDTO result = await service.GetChapterAsync(category);
            Assert.IsTrue(result.Category == category);
        }

        private static IEnumerable<object[]> ChaptersData()
        {
            yield return new object[] { "C-0" };
            yield return new object[] { "C-10" };
            yield return new object[] { "DPD" };
        }
    }
}
