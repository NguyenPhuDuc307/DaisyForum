using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DaisyForum.ViewModels.Systems;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DaisyForum.BackendServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        // URL: POST: https://localhost:5000/api/roles
        [HttpPost]
        public async Task<IActionResult> CreateRole(RoleViewModel request)
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
                return CreatedAtAction(nameof(GetRole), new { id = request.RoleId }, request);
            else
                return BadRequest(result.Errors.First().Description);
        }

        //URL: GET: https://localhost:5000/api/roles
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRole(string id)
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

        //URL: PUT: https://localhost:5000/api/roles/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(string id, [FromBody] RoleViewModel roleViewModel)
        {
            if (id != roleViewModel.RoleId)
                // code: 400
                return BadRequest();

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                // code: 404
                return NotFound();
                
            role.Name = roleViewModel.RoleName;
            role.NormalizedName = roleViewModel.RoleName != null ? roleViewModel.RoleName.ToUpper() : null;
            var result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded)
                // code: 204
                return NoContent();

            return BadRequest(result.Equals(false) ? result.Errors.First() : null);
        }
    }
}