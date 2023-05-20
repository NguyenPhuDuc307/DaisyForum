using DaisyForum.BackendServer.Helpers;

namespace DaisyForum.BackendServer.UnitTest.Helpers;

public class ApiNotFoundResponseTest
{
    [Fact]
    public void Constructor_CreateInstance_ShouldBe_NotNull()
    {
        var obj = new ApiNotFoundResponse("error");
        Assert.NotNull(obj);
        Assert.Equal(404, obj.StatusCode);
    }
}