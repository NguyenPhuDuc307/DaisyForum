using DaisyForum.BackendServer.Controllers;
using DaisyForum.BackendServer.Data;
using DaisyForum.BackendServer.Services;
using Moq;

namespace DaisyForum.BackendServer.UnitTest.Controllers;

public class CategoriesControllerTest
{
    private ApplicationDbContext _context;
    private Mock<ICacheService> _mockCacheService;

    public CategoriesControllerTest()
    {
        _context = new InMemoryDbContextFactory().GetApplicationDbContext("CommentsControllerTest");
        _mockCacheService = new Mock<ICacheService>();
    }

    [Fact]
    public void ShouldCreateInstance_NotNull_Success()
    {
        var controller = new CategoriesController(_context, _mockCacheService.Object);
        Assert.NotNull(controller);
    }
}