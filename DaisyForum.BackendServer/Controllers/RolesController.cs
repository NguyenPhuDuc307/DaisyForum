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
                Id = request.RoleId != null ? request.RoleId : Guid.NewGuid().ToString(),
                Name = request.RoleName,
                NormalizedName = request.RoleName != null ? request.RoleName.ToUpper() : string.Empty
            };

            var result = await _roleManager.CreateAsync(role);
            if (result.Succeeded)
                // code: 201
                return CreatedAtAction(nameof(GetRoleById), new { id = request.RoleId }, request);
            else
                return BadRequest(result.Errors);
        }

        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();

            var roleViewModel = roles.Select(r => new RoleViewModel()
            {
                RoleId = r.Id,
                RoleName = r.Name
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
                RoleId = x.Id,
                RoleName = x.Name
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
                RoleId = role.Id,
                RoleName = role.Name
            };
            // code: 200
            return Ok(RoleViewModel);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutRole(string id, [FromBody] RoleCreateRequest request)
        {
            if (id != request.RoleId)
                // code: 400
                return BadRequest();

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                // code: 404
                return NotFound();

            role.Name = request.RoleName;
            role.NormalizedName = request.RoleName != null ? request.RoleName.ToUpper() : null;
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
                    RoleId = role.Id,
                    RoleName = role.Name
                };
                // code: 200
                return Ok(RoleViewModel);
            }
            return BadRequest(result.Errors);
        }
    }
}