using DaisyForum.BackendServer.Authorization;
using DaisyForum.BackendServer.Constants;

namespace DaisyForum.BackendServer.UnitTest.Authorization
{
    public class ClaimRequirementAttributeTest
    {
        [Fact]
        public void Constructor_CreateInstance_ShouldBe_NotNull()
        {
            var obj = new ClaimRequirementAttribute(FunctionCode.CONTENT, CommandCode.CREATE);
            Assert.NotNull(obj);
            Assert.NotNull(obj.Arguments);
        }
    }
}