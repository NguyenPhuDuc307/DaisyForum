﻿using DaisyForum.ViewModels.Contents;
using DaisyForum.ViewModels.Contents.Validators;
using Xunit;

namespace DaisyForum.ViewModels.UnitTest.Contents
{
    public class ReportCreateRequestValidatorTest
    {
        private ReportCreateRequestValidator validator;
        private ReportCreateRequest request;

        public ReportCreateRequestValidatorTest()
        {
            request = new ReportCreateRequest()
            {
                Content = "test",
                KnowledgeBaseId = 1,
            };
            validator = new ReportCreateRequestValidator();
        }

        [Fact]
        public void Should_Valid_Result_When_Valid_Request()
        {
            var result = validator.Validate(request);
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Should_Error_Result_When_Miss_Content(string content)
        {
            request.Content = content;
            var result = validator.Validate(request);
            Assert.False(result.IsValid);
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