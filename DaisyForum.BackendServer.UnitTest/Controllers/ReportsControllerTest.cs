using DaisyForum.BackendServer.Controllers;
using DaisyForum.BackendServer.Data;
using DaisyForum.BackendServer.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace DaisyForum.BackendServer.UnitTest.Controllers
{
    public class ReportsControllerTest
    {
        private ApplicationDbContext _context;
        private Mock<ISequenceService> _mockSequenceService;
        private Mock<IStorageService> _mockStorageService;
        private Mock<ILogger<KnowledgeBasesController>> _mockLoggerService;
        private Mock<IEmailSender> _mockEmailSender;
        private Mock<IViewRenderService> _mockViewRenderService;
        private Mock<ICacheService> _mockCacheService;

        public ReportsControllerTest()
        {
            _context = new InMemoryDbContextFactory().GetApplicationDbContext();
            _mockSequenceService = new Mock<ISequenceService>();
            _mockStorageService = new Mock<IStorageService>();
            _mockLoggerService = new Mock<ILogger<KnowledgeBasesController>>();
            _mockEmailSender = new Mock<IEmailSender>();
            _mockViewRenderService = new Mock<IViewRenderService>();
            _mockCacheService = new Mock<ICacheService>();
        }
    }
}