using DaisyForum.BackendServer.Controllers;
using DaisyForum.BackendServer.Data;
using DaisyForum.BackendServer.Services;
using DaisyForum.ViewModels.Contents;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;

namespace DaisyForum.BackendServer.UnitTest.Controllers
{
    public class KnowledgeBasesControllerTest
    {
        private ApplicationDbContext _context;
        private Mock<ISequenceService> _mockSequenceService;
        private Mock<IStorageService> _mockStorageService;
        private Mock<ILogger<KnowledgeBasesController>> _mockLoggerService;
        private Mock<IEmailSender> _mockEmailSender;
        private Mock<IViewRenderService> _mockViewRenderService;
        private Mock<ICacheService> _mockCacheService;
        private Mock<IOneSignalService> _mockOneSignalService;
        private Mock<IContentBasedService> _mockCSVService;

        public KnowledgeBasesControllerTest()
        {
            _context = new InMemoryDbContextFactory().GetApplicationDbContext();
            _mockSequenceService = new Mock<ISequenceService>();
            _mockStorageService = new Mock<IStorageService>();
            _mockLoggerService = new Mock<ILogger<KnowledgeBasesController>>();
            _mockEmailSender = new Mock<IEmailSender>();
            _mockViewRenderService = new Mock<IViewRenderService>();
            _mockCacheService = new Mock<ICacheService>();
            _mockOneSignalService = new Mock<IOneSignalService>();
            _mockCSVService = new Mock<IContentBasedService>();
        }

        [Fact]
        public void ShouldCreateInstance_NotNull_Success()
        {
            var controller = new KnowledgeBasesController(_context, _mockSequenceService.Object, _mockStorageService.Object,
                _mockLoggerService.Object, _mockEmailSender.Object, _mockViewRenderService.Object, _mockCacheService.Object, _mockOneSignalService.Object, _mockCSVService.Object);
            Assert.NotNull(controller);
        }

        [Fact]
        public async Task PostKnowledgeBase_ValidInput_Success()
        {
            _mockSequenceService.Setup(x => x.GetKnowledgeBaseNewId()).ReturnsAsync(1);
            var controller = new KnowledgeBasesController(_context, _mockSequenceService.Object, _mockStorageService.Object,
                           _mockLoggerService.Object, _mockEmailSender.Object, _mockViewRenderService.Object, _mockCacheService.Object, _mockOneSignalService.Object, _mockCSVService.Object);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]{
                    new Claim(ClaimTypes.NameIdentifier, "1"),
                }, "mock"));

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
            var result = await controller.PostKnowledgeBase(new KnowledgeBaseCreateRequest()
            {
                Title = "test",
                Id = 1,
                Problem = "test",
                Note = "test"
            });

            Assert.IsType<CreatedAtActionResult>(result);
        }
    }
}