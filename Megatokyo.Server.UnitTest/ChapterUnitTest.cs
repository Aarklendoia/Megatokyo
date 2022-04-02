using Megatokyo.Infrastructure.Repository.EF;
using Megatokyo.Infrastructure.Repository.EF.Entity;
using Megatokyo.Server.DTO.v1;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Megatokyo.Server.UnitTest
{
    [TestClass]
    public class ChapterUnitTest
    {
        [TestMethod]
        public async Task GetAllChapters()
        {
            using APIClient apiClient = new("/api/1.0");
            apiClient.InsertData += (APIContext context) =>
            {
                ChapterEntity chapter1 = new()
                {
                    Category = "C-0",
                    Number = 1,
                    Title = "Test chapter 1"
                };
                context.Chapters.Add(chapter1);
                ChapterEntity chapter2 = new()
                {
                    Category = "C-1",
                    Number = 2,
                    Title = "Test chapter 2"
                };
                context.Chapters.Add(chapter2);
                context.SaveChanges();
            };
            IEnumerable<ChapterOutputDTO>? chapters = await apiClient.GetAsync<IEnumerable<ChapterOutputDTO>>("chapters");
            Assert.IsNotNull(chapters);
            Assert.AreEqual(2, chapters.Count());
        }

        [TestMethod]
        public async Task GetChapter()
        {
            using APIClient apiClient = new("/api/1.0");
            apiClient.InsertData += (APIContext context) =>
            {
                ChapterEntity chapter1 = new()
                {
                    Category = "C-0",
                    Number = 1,
                    Title = "Test chapter 1"
                };
                context.Chapters.Add(chapter1);
                ChapterEntity chapter2 = new()
                {
                    Category = "C-1",
                    Number = 2,
                    Title = "Test chapter 2"
                };
                context.Chapters.Add(chapter2);
                context.SaveChanges();
            };
            ChapterOutputDTO? chapter = await apiClient.GetAsync<ChapterOutputDTO>("chapters/C-1");
            Assert.IsNotNull(chapter);
            Assert.AreEqual("C-1", chapter.Category);
            Assert.AreEqual(2, chapter.Number);
            Assert.AreEqual("Test chapter 2", chapter.Title);
        }
    }
}
