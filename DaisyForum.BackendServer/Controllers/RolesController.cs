using DaisyForum.ViewModels;
using DaisyForum.ViewModels.Systems;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DaisyForum.BackendServer.Controllers
{
    public class RolesController : BaseController
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        [HttpPost]
        public async Task<IActionResult> PostRole(RoleCreateRequest request)
        {
            var role = new IdentityRole()
            {
                Id = request.Id != null ? request.Id : Guid.NewGuid().ToString(),
                Name = request.Name,
                NormalizedName = request.Name != null ? request.Name.ToUpper() : string.Empty
            };

            var result = await _roleManager.CreateAsync(role);
            if (result.Succeeded)
                // code: 201
                return CreatedAtAction(nameof(GetRoleById), new { id = request.Id }, request);
            else
                return BadRequest(result.Errors);
        }

        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();

            var roleViewModel = roles.Select(r => new RoleViewModel()
            {
                Id = r.Id,
                Name = r.Name
            });
            return Ok(roleViewModel);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetRolesPaging(string? keyword, int page = 1, int pageSize = 10)
        {
            var query = _roleManager.Roles;

            if (!String.IsNullOrEmpty(keyword))
            {
                query = query.Where(x => x.Id.Contains(keyword) || string.IsNullOrEmpty(x.Name) ? true : x.Name.Contains(keyword));
            }

            var item = await query.Skip((page - 1) * pageSize).Take(pageSize).Select(x => new RoleViewModel()
            {
                Id = x.Id,
                Name = x.Name
            }).ToListAsync();

            var totalRecords = await query.CountAsync();

            var pagination = new Pagination<RoleViewModel>
            {
                Items = item,
                TotalRecords = totalRecords
            };

            return Ok(pagination);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleById(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                // code: 404
                return NotFound();
            var RoleViewModel = new RoleViewModel()
            {
                Id = role.Id,
                Name = role.Name
            };
            // code: 200
            return Ok(RoleViewModel);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutRole(string id, [FromBody] RoleCreateRequest request)
        {
            if (id != request.Id)
                // code: 400
                return BadRequest();

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                // code: 404
                return NotFound();

            role.Name = request.Name;
            role.NormalizedName = request.Name != null ? request.Name.ToUpper() : null;
            var result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded)
                // code: 204
                return NoContent();

            return BadRequest(result.Errors);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                // code: 404
                return NotFound();

            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                var RoleViewModel = new RoleViewModel()
                {
                    Id = role.Id,
                    Name = role.Name
                };
                // code: 200
                return Ok(RoleViewModel);
            }
            return BadRequest(result.Errors);
        }
    }
}