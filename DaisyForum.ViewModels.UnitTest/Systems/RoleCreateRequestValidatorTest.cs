using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DaisyForum.ViewModels.Systems.Validators;
using DaisyForum.ViewModels.Systems;

namespace DaisyForum.ViewModels.UnitTest.Systems
{
    public class RoleCreateRequestValidatorTest
    {
        private RoleCreateRequestValidator validator;
        private RoleCreateRequest request;

        public RoleCreateRequestValidatorTest()
        {
            request = new RoleCreateRequest()
            {
                RoleId = "admin",
                RoleName = "admin"
            };
            validator = new RoleCreateRequestValidator();
        }

        [Fact]
        public void Should_Valid_Result_When_Valid_Request()
        {
            var result = validator.Validate(request);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Should_Error_Result_When_Request_Miss_RoleId()
        {
            request.RoleId = string.Empty;
            var result = validator.Validate(request);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Should_Error_Result_When_Request_Miss_RoleName()
        {
            request.RoleName = string.Empty;
            var result = validator.Validate(request);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Should_Error_Result_When_Request_Role_Empty()
        {
            request.RoleId = string.Empty;
            request.RoleName = string.Empty;
            var result = validator.Validate(request);
            Assert.False(result.IsValid);
        }
    }
}