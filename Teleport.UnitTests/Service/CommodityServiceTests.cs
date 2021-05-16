using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Teleport.Models;
using Teleport.Repository;
using Teleport.Services;

namespace Teleport.UnitTests.Service
{
    [TestFixture]
    public class CommodityServiceTests
    {
        private ICommodityRepo _commodityRepo;
        private CommodityService _commodityService;

        [SetUp]
        public void SetUp()
        {
            _commodityRepo = Substitute.For<ICommodityRepo>();

            _commodityService = new CommodityService(_commodityRepo);
        }

        [Test]
        public void should_return_all_commodities()
        {
            var commodities = new List<Commodity> { new() { Id = 1, Title = "book" } };
            _commodityRepo.GetAll().Returns(commodities);

            _commodityService.GetAllCommodities().Should().BeEquivalentTo(commodities);
        }
    }
}