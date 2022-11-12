using Megatokyo.Infrastructure.Repository.EF;
using Megatokyo.Infrastructure.Repository.EF.Entity;
using Megatokyo.Server.DTO.v1;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Megatokyo.Server.UnitTest
{
    [TestClass]
    public class StripUnitTest
    {
        [TestMethod]
        public async Task GetAllStrips()
        {
            APIClient apiClient = TestServer.Client();
            apiClient.InsertData = (APIContext context) =>
            {
                StripEntity strip1 = new()
                {
                    Category = "C-0",
                    Number = 1,
                    PublishDate = DateTime.Now,
                    Title = "Test strip 1",
                    Url = new Uri("https://www.megatokyo.com/strips/1")
                };
                context.Strips.Add(strip1);
                StripEntity strip2 = new()
                {
                    Category = "C-0",
                    Number = 2,
                    PublishDate = DateTime.Now,
                    Title = "Test strip 2",
                    Url = new Uri("https://www.megatokyo.com/strips/2")
                };
                context.Strips.Add(strip2);
                context.SaveChanges();
            };
            IEnumerable<StripOutputDTO>? strips = await apiClient.GetAsync<IEnumerable<StripOutputDTO>>("strips");
            Assert.IsNotNull(strips);
            Assert.AreEqual(2, strips.Count());
        }

        [TestMethod]
        public async Task GetCategoryStrips()
        {
            APIClient apiClient = TestServer.Client();
            apiClient.InsertData = (APIContext context) =>
            {
                StripEntity strip1 = new()
                {
                    Category = "C-0",
                    Number = 1,
                    PublishDate = DateTime.Now,
                    Title = "Test strip 1",
                    Url = new Uri("https://www.megatokyo.com/strips/1")
                };
                context.Strips.Add(strip1);
                StripEntity strip2 = new()
                {
                    Category = "C-1",
                    Number = 2,
                    PublishDate = DateTime.Now,
                    Title = "Test strip 2",
                    Url = new Uri("https://www.megatokyo.com/strips/2")
                };
                context.Strips.Add(strip2);
                context.SaveChanges();
            };
            IEnumerable<StripOutputDTO>? strips = await apiClient.GetAsync<IEnumerable<StripOutputDTO>>("strips/category/C-0");
            Assert.IsNotNull(strips);
            Assert.AreEqual(1, strips.Count());
        }

        [TestMethod]
        public async Task GetStrip()
        {
            DateTimeOffset expectedDate = new(2022, 1, 1, 12, 38, 44, TimeSpan.Zero);
            APIClient apiClient = TestServer.Client();
            apiClient.InsertData = (APIContext context) =>
            {
                StripEntity strip = new()
                {
                    Category = "C-0",
                    Number = 1,
                    PublishDate = expectedDate,
                    Title = "Test strip 1",
                    Url = new Uri("https://www.megatokyo.com/strips/1")
                };
                context.Strips.Add(strip);
                context.SaveChanges();
            };
            StripOutputDTO? strip = await apiClient.GetAsync<StripOutputDTO>("strips/1");
            Assert.IsNotNull(strip);
            Assert.AreEqual("C-0", strip.Category);
            Assert.AreEqual(1, strip.Number);
            Assert.AreEqual(expectedDate, strip.PublishDate);
            Assert.AreEqual("Test strip 1", strip.Title);
            Assert.AreEqual("https://www.megatokyo.com/strips/1", strip.Url?.ToString());
        }
    }
}
