using DaisyForum.BackendServer.Controllers;
using DaisyForum.BackendServer.Data;
using DaisyForum.BackendServer.Data.Entities;
using DaisyForum.ViewModels.Systems;
using Microsoft.AspNetCore.Mvc;

namespace DaisyForum.BackendServer.UnitTest.Controllers
{
    public class CommandsControllerTest
    {
        private ApplicationDbContext _context;

        public CommandsControllerTest()
        {
            _context = new InMemoryDbContextFactory().GetApplicationDbContext();
        }

        [Fact]
        public void ShouldCreateInstance_NotNull_Success()
        {
            var usersController = new CommandsController(_context);
            Assert.NotNull(usersController);
        }
    }
}