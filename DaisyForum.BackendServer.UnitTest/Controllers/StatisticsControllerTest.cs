using DaisyForum.BackendServer.Controllers;
using DaisyForum.BackendServer.Data;

namespace DaisyForum.BackendServer.UnitTest.Controllers;

public class StatisticsControllerTest
{
    private ApplicationDbContext _context;

    public StatisticsControllerTest()
    {
        _context = new InMemoryDbContextFactory().GetApplicationDbContext();
    }

    [Fact]
    public void ShouldCreateInstance_NotNull_Success()
    {
        var controller = new StatisticsController(_context);
        Assert.NotNull(controller);
    }
}