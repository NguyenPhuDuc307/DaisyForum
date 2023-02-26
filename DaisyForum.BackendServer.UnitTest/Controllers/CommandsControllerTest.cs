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
        public async Task GetCommand_HasData_ReturnSuccess()
        {
            _context.Commands.AddRange(new List<Command>()
            {
                new Command(){
                    Id = "Create",
                    Name = "Thêm"
                }
            });
            await _context.SaveChangesAsync();
            var CommandsController = new CommandsController(_context);
            var result = await CommandsController.GetCommands();
            var okResult = result as OkObjectResult;
            var UserViewModels = okResult != null ? okResult.Value as IEnumerable<CommandViewModel> : null;
            Assert.True(UserViewModels != null ? UserViewModels.Count() > 0 : false);
        }
    }
}