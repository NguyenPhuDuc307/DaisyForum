using DaisyForum.BackendServer.Data;
using DaisyForum.ViewModels.Systems;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DaisyForum.BackendServer.Controllers
{
    public class CommandsController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public CommandsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet()]
        public async Task<IActionResult> GetCommands()
        {
            var commands = _context.Commands;

            var commandViewModels = await commands.Select(u => new CommandViewModel()
            {
                Id = u.Id,
                Name = u.Name,
            }).ToListAsync();

            return Ok(commandViewModels);
        }
    }
}