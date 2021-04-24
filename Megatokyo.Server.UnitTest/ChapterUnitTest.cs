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
            //HttpClient client = TestServer.GetClient();
            //IMegatokyoServer service = new MegatokyoServer(client);
            //ICollection<Chapter> result = service.GetChaptersAsync().GetAwaiter().GetResult();
            //Assert.IsTrue(result.Count > 0);
        }

        [TestMethod]
        [DynamicData(nameof(ChaptersData), DynamicDataSourceType.Method)]
        public void GetByCategoryTestmethod(string category)
        {
            //HttpClient client = TestServer.GetClient();
            //IMegatokyoServer service = new MegatokyoServer(client);
            //Chapters result = service.GetChapter(category).GetAwaiter().GetResult();
            //Assert.IsTrue(result.Category == category);
        }

        private static IEnumerable<object[]> ChaptersData()
        {
            yield return new object[] { "C-0", false };
            yield return new object[] { "C-0", true };
            yield return new object[] { "DPD", false };
        }
    }
}
