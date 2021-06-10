using AutoMapper;
using Megatokyo.Client;
using Megatokyo.Domain;
using Megatokyo.Infrastructure.Repository.EF.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Megatokyo.Server.UnitTest
{
    [TestClass]
    public class MappingUnitTest
    {
        [TestMethod]
        public void ChapterOutputDTODomainTestMethod()
        {
            var configuration = new MapperConfiguration(cfg =>
              cfg.CreateMap<ChapterOutputDTO, ChapterDomain>());

            configuration.AssertConfigurationIsValid();
        }

        [TestMethod]
        public void ChapterEntityDomainTestMethod()
        {
            var configuration = new MapperConfiguration(cfg =>
              cfg.CreateMap<ChapterEntity, ChapterDomain>());

            configuration.AssertConfigurationIsValid();
        }

        [TestMethod]
        public void StripOutputDTODomainTestMethod()
        {
            var configuration = new MapperConfiguration(cfg =>
              cfg.CreateMap<StripOutputDTO, StripDomain>());

            configuration.AssertConfigurationIsValid();
        }

        [TestMethod]
        public void StripEntityDomainTestMethod()
        {
            var configuration = new MapperConfiguration(cfg =>
              cfg.CreateMap<StripEntity, StripDomain>());

            configuration.AssertConfigurationIsValid();
        }

        [TestMethod]
        public void RantOutputDTODomainTestMethod()
        {
            var configuration = new MapperConfiguration(cfg =>
              cfg.CreateMap<RantOutputDTO, RantDomain>());

            configuration.AssertConfigurationIsValid();
        }

        [TestMethod]
        public void RantEntityDomainTestMethod()
        {
            var configuration = new MapperConfiguration(cfg =>
              cfg.CreateMap<RantEntity, RantDomain>());

            configuration.AssertConfigurationIsValid();
        }

        [TestMethod]
        public void CheckingEntityDomainTestMethod()
        {
            var configuration = new MapperConfiguration(cfg =>
              cfg.CreateMap<CheckingEntity, CheckingDomain>());

            configuration.AssertConfigurationIsValid();
        }
    }
}
