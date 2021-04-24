using EIG.Formation.ClientAPI.UnitTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Net.Http;
using Megatokyo.Server;

namespace Megatokyo.Server.UnitTest
{
    [TestClass]
    public class ChapterUnitTest
    {
        [TestMethod]
        
        public void ChaptersAllAsyncTestMethod()
        {
            HttpClient client = TestServer.GetClient();
            IMegatokyoClient service = new MegatokyoClient(client);
            ICollection<ChapterOutputDTO> result = service.GetAllChaptersAsync().GetAwaiter().GetResult();
            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod]
        [DynamicData(nameof(ChaptersData), DynamicDataSourceType.Method)]
        public void GetByCategoryTestmethod(string category)
        {
            HttpClient client = TestServer.GetClient();
            IMegatokyoClient service = new MegatokyoClient(client);
            ChapterOutputDTO result = service.GetChapterAsync(category).GetAwaiter().GetResult();
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
