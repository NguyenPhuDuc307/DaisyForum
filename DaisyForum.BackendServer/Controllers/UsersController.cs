using DaisyForum.BackendServer.Data.Entities;
using DaisyForum.ViewModels;
using DaisyForum.ViewModels.Systems;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DaisyForum.BackendServer.Controllers
{
    public class UsersController : BaseController
    {
        private readonly UserManager<User> _userManager;
        public UsersController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> PostUser(UserCreateRequest request)
        {
            var user = new User()
            {
                Id = Guid.NewGuid().ToString(),
                Email = request.Email,
                Dob = request.Dob,
                UserName = request.UserName,
                LastName = request.LastName,
                FirstName = request.FirstName,
                PhoneNumber = request.PhoneNumber
            };

            if (request.Password == null)
                return BadRequest();

            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                // code: 201
                return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, request);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = _userManager.Users;

            var userViewModels = await users.Select(u => new UserViewModel()
            {
                Id = u.Id,
                UserName = u.UserName,
                Dob = u.Dob,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                FirstName = u.FirstName,
                LastName = u.LastName
            }).ToListAsync();

            return Ok(userViewModels);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetUsersPaging(string? keyword, int page = 1, int pageSize = 10)
        {
            var query = _userManager.Users;

            if (!String.IsNullOrEmpty(keyword))
            {
                query = query.Where(x =>
                    (x.UserName != null && x.UserName.Contains(keyword))
                    || (x.Email != null && x.Email.Contains(keyword))
                    || (x.PhoneNumber != null && x.PhoneNumber.Contains(keyword)));
            }

            var item = await query.Skip((page - 1) * pageSize).Take(pageSize).Select(x => new UserViewModel()
            {
                Id = x.Id,
                UserName = x.UserName,
                Dob = x.Dob,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                FirstName = x.FirstName,
                LastName = x.LastName
            }).ToListAsync();

            var totalRecords = await query.CountAsync();

            var pagination = new Pagination<UserViewModel>
            {
                Items = item,
                TotalRecords = totalRecords
            };

            return Ok(pagination);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var userViewModel = new UserViewModel()
            {
                Id = user.Id,
                UserName = user.UserName,
                Dob = user.Dob,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
            return Ok(userViewModel);
        }

        [HttpGet("{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return NotFound();

            var userViewModel = new UserViewModel()
            {
                Id = user.Id,
                UserName = user.UserName,
                Dob = user.Dob,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
            return Ok(userViewModel);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(string id, [FromBody] UserCreateRequest request)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                // code: 400
                return NotFound();

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Dob = request.Dob;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                // code: 200
                return NoContent();
            }
            return BadRequest(result.Errors);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                var userViewModel = new UserViewModel()
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Dob = user.Dob,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                };
                return Ok(userViewModel);
            }
            return BadRequest(result.Errors);
        }
    }
}