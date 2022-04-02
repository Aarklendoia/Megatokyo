using Megatokyo.Infrastructure.Repository.EF;
using Megatokyo.Infrastructure.Repository.EF.Entity;
using Megatokyo.Server.DTO.v1;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Megatokyo.Server.UnitTest
{
    [TestClass]
    public class RantUnitTest
    {
        [TestMethod]
        public async Task GetAllRants()
        {
            DateTimeOffset expectedDate = new(2022, 1, 1, 12, 38, 44, TimeSpan.Zero);
            APIClient apiClient = TestServer.Client();
            apiClient.InsertData = (APIContext context) =>
            {
                RantEntity rant1 = new()
                {
                    Author = "Author 1",
                    Content = "Content 1",
                    Number = 1,
                    PublishDate = expectedDate,
                    Title = "Test rant 1",
                    Url = new("https://www.megatokyo.com/rants/1")
                };
                context.Rants.Add(rant1);
                RantEntity rant2 = new()
                {
                    Author = "Author 2",
                    Content = "Content 2",
                    Number = 2,
                    PublishDate = expectedDate,
                    Title = "Test rant 2",
                    Url = new("https://www.megatokyo.com/rants/2")

                };
                context.Rants.Add(rant2);
                context.SaveChanges();
            };
            IEnumerable<RantOutputDTO>? rants = await apiClient.GetAsync<IEnumerable<RantOutputDTO>>("rants");
            Assert.IsNotNull(rants);
            Assert.AreEqual(2, rants.Count());
        }

        [TestMethod]
        public async Task GetRant()
        {
            DateTimeOffset expectedDate = new(2022, 1, 1, 12, 38, 44, TimeSpan.Zero);
            APIClient apiClient = TestServer.Client();
            apiClient.InsertData = (APIContext context) =>
            {
                RantEntity rant1 = new()
                {
                    Author = "Author 1",
                    Content = "Content 1",
                    Number = 1,
                    PublishDate = expectedDate,
                    Title = "Test rant 1",
                    Url = new("https://www.megatokyo.com/rants/1")
                };
                context.Rants.Add(rant1);
                RantEntity rant2 = new()
                {
                    Author = "Author 2",
                    Content = "Content 2",
                    Number = 2,
                    PublishDate = expectedDate,
                    Title = "Test rant 2",
                    Url = new("https://www.megatokyo.com/rants/2")

                };
                context.Rants.Add(rant2);
                context.SaveChanges();
            };
            RantOutputDTO? rant = await apiClient.GetAsync<RantOutputDTO>("rants/2");
            Assert.IsNotNull(rant);
            Assert.AreEqual("Author 2", rant.Author);
            Assert.AreEqual("Content 2", rant.Content);
            Assert.AreEqual(2, rant.Number);
            Assert.AreEqual(expectedDate, rant.PublishDate);
            Assert.AreEqual("Test rant 2", rant.Title);
            Assert.AreEqual("https://www.megatokyo.com/rants/2", rant.Url?.ToString());
        }
    }
}
