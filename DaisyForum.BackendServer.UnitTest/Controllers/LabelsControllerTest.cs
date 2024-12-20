using DaisyForum.BackendServer.Controllers;
using DaisyForum.BackendServer.Data;
using DaisyForum.BackendServer.Services;
using Moq;
using Xunit;

namespace DaisyForum.BackendServer.UnitTest.Controllers
{
    public class LabelsControllerTest
    {
        private ApplicationDbContext _context;
        private Mock<ICacheService> _mockCacheService;

        public LabelsControllerTest()
        {
            _context = new InMemoryDbContextFactory().GetApplicationDbContext();
            _mockCacheService = new Mock<ICacheService>();
        }

        [Fact]
        public void ShouldCreateInstance_NotNull_Success()
        {
            var controller = new LabelsController(_context, _mockCacheService.Object);
            Assert.NotNull(controller);
        }
    }
}