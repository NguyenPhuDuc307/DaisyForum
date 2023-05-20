using DaisyForum.ViewModels.Contents;
using DaisyForum.ViewModels.Contents.Validators;
using Xunit;

namespace DaisyForum.ViewModels.UnitTest.Contents
{
    public class VoteCreateRequestValidatorTest
    {
        private VoteCreateRequestValidator validator;
        private VoteCreateRequest request;

        public VoteCreateRequestValidatorTest()
        {
            request = new VoteCreateRequest()
            {
                KnowledgeBaseId = 1
            };
            validator = new VoteCreateRequestValidator();
        }

        [Fact]
        public void Should_Valid_Result_When_Valid_Request()
        {
            var result = validator.Validate(request);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Should_Error_Result_When_KnowledgeBaseId_Is_Zero()
        {
            request.KnowledgeBaseId = 0;
            var result = validator.Validate(request);
            Assert.False(result.IsValid);
        }
    }
}