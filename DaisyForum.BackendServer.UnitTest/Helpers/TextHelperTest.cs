using DaisyForum.BackendServer.Helpers;
using Xunit;

namespace DaisyForum.BackendServer.UnitTest.Helpers
{
    public class TextHelperTest
    {
        [Fact]
        public void ToUnsignString_ValidInput_SuccessResult()
        {
            var result = TextHelper.ToUnsignedString("tôi muốn chuyển sang không dấu");
            Assert.Equal("toi-muon-chuyen-sang-khong-dau", result);
        }
    }
}