using EIG.Formation.ClientAPI.UnitTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Megatokyo.Server.UnitTest
{
    [TestClass]
    public class ChapterUnitTest
    {
        [TestMethod]
        public void ChaptersAllAsyncTestMethod()
        {
            HttpClient client = TestServer.GetClient();
            IMegatokyoServer service = new MegatokyoServer(client);

            //Act
            ICollection<Chapter> result = service.GetChaptersAsync().GetAwaiter().GetResult();


            //Assert
            Assert.IsTrue(result.Count > 0);
        }
    }
}
