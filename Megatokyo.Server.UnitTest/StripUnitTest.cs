using EIG.Formation.ClientAPI.UnitTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Net.Http;

namespace Megatokyo.Server.UnitTest
{
    [TestClass]
    public class StripUnitTest
    {
        [TestMethod]
        public void StripsAllAsyncTestMethod()
        {
            HttpClient client = TestServer.GetClient();
            IMegatokyoClient service = new MegatokyoClient(client);
            ICollection<StripOutputDTO> result = service.GetAllStripsAsync().GetAwaiter().GetResult();
            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod]
        [DynamicData(nameof(StripCategoryData), DynamicDataSourceType.Method)]
        public void GetByCategoryTestmethod(string category, int count)
        {
            HttpClient client = TestServer.GetClient();
            IMegatokyoClient service = new MegatokyoClient(client);
            ICollection<StripOutputDTO> result = service.GetCategoryStripsAsync(category).GetAwaiter().GetResult();
            Assert.IsTrue(result.Count == count);
        }

        private static IEnumerable<object[]> StripCategoryData()
        {
            yield return new object[] { "C-0", 95 };
            yield return new object[] { "C-10", 110 };
        }

        [TestMethod]
        [DynamicData(nameof(StripNumberData), DynamicDataSourceType.Method)]
        public void GetByCategoryTestmethod(int  number)
        {
            HttpClient client = TestServer.GetClient();
            IMegatokyoClient service = new MegatokyoClient(client);
            StripOutputDTO result = service.GetStripAsync(number).GetAwaiter().GetResult();
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
