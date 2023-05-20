using DaisyForum.BackendServer.Authorization;
using DaisyForum.BackendServer.Constants;
using DaisyForum.BackendServer.Data;
using DaisyForum.BackendServer.Data.Entities;
using DaisyForum.BackendServer.Helpers;
using DaisyForum.ViewModels;
using DaisyForum.ViewModels.Systems;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DaisyForum.BackendServer.Controllers;

public class RolesController : BaseController
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;
    public RolesController(RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
    {
        _roleManager = roleManager;
        _context = context;
    }

    [HttpPost]
    [ClaimRequirement(FunctionCode.SYSTEM_ROLE, CommandCode.CREATE)]
    [ApiValidationFilter]
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
            return CreatedAtAction(nameof(GetRoleById), new { id = request.Id }, request);
        else
            return BadRequest(new ApiBadRequestResponse(result));
    }

    [HttpGet]
    [ClaimRequirement(FunctionCode.SYSTEM_ROLE, CommandCode.VIEW)]
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
    [ClaimRequirement(FunctionCode.SYSTEM_ROLE, CommandCode.VIEW)]
    public async Task<IActionResult> GetRolesPaging(string? keyword, int page = 1, int pageSize = 10)
    {
        var query = _roleManager.Roles;

        if (!String.IsNullOrEmpty(keyword))
        {
            query = query.Where(x =>
                (x.Id != null && x.Id.Contains(keyword))
                || (x.Name != null && x.Name.Contains(keyword)));
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
    [ClaimRequirement(FunctionCode.SYSTEM_ROLE, CommandCode.VIEW)]
    public async Task<IActionResult> GetRoleById(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);
        if (role == null)
            return NotFound(new ApiNotFoundResponse($"Cannot find role with id: {id}"));
        var RoleViewModel = new RoleViewModel()
        {
            Id = role.Id,
            Name = role.Name
        };
        return Ok(RoleViewModel);
    }

    [HttpPut("{id}")]
    [ClaimRequirement(FunctionCode.SYSTEM_ROLE, CommandCode.UPDATE)]
    public async Task<IActionResult> PutRole(string id, [FromBody] RoleCreateRequest request)
    {
        if (id != request.Id)
            return BadRequest(new ApiBadRequestResponse("Role id not match"));

        var role = await _roleManager.FindByIdAsync(id);
        if (role == null)
            return NotFound(new ApiNotFoundResponse($"Cannot find role with id: {id}"));

        role.Name = request.Name;
        role.NormalizedName = request.Name != null ? request.Name.ToUpper() : null;
        var result = await _roleManager.UpdateAsync(role);

        if (result.Succeeded)
            return NoContent();

        return BadRequest(new ApiBadRequestResponse(result));
    }

    [HttpDelete("{id}")]
    [ClaimRequirement(FunctionCode.SYSTEM_ROLE, CommandCode.VIEW)]
    public async Task<IActionResult> DeleteRole(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);
        if (role == null)
            return NotFound(new ApiNotFoundResponse($"Cannot find role with id: {id}"));

        var result = await _roleManager.DeleteAsync(role);
        if (result.Succeeded)
        {
            var RoleViewModel = new RoleViewModel()
            {
                Id = role.Id,
                Name = role.Name
            };
            return Ok(RoleViewModel);
        }
        return BadRequest(new ApiBadRequestResponse(result));
    }

    [HttpGet("{roleId}/permissions")]
    [ClaimRequirement(FunctionCode.SYSTEM_PERMISSION, CommandCode.VIEW)]
    public async Task<IActionResult> GetPermissionByRoleId(string roleId)
    {
        var permissions = from p in _context.Permissions
                          join a in _context.Commands
                          on p.CommandId equals a.Id
                          where p.RoleId == roleId
                          select new PermissionViewModel()
                          {
                              FunctionId = p.FunctionId,
                              CommandId = p.CommandId,
                              RoleId = p.RoleId
                          };

        return Ok(await permissions.ToListAsync());
    }

    [HttpPut("{roleId}/permissions")]
    [ClaimRequirement(FunctionCode.SYSTEM_PERMISSION, CommandCode.VIEW)]
    [ApiValidationFilter]
    public async Task<IActionResult> PutPermissionByRoleId(string roleId, [FromBody] UpdatePermissionRequest request)
    {
        var newPermissions = new List<Permission>();
        foreach (var p in request.Permissions)
        {
            if (p.FunctionId != null && p.CommandId != null)
                newPermissions.Add(new Permission(p.FunctionId, roleId, p.CommandId));
        }

        var existingPermissions = _context.Permissions.Where(x => x.RoleId == roleId);
        _context.Permissions.RemoveRange(existingPermissions);
        _context.Permissions.AddRange(newPermissions.Distinct(new MyPermissionComparer()));
        var result = await _context.SaveChangesAsync();
        if (result > 0)
        {
            return NoContent();
        }
        return BadRequest(new ApiBadRequestResponse("Save permission failed"));
    }

    internal class MyPermissionComparer : IEqualityComparer<Permission>
    {
        public bool Equals(Permission? x, Permission? y)
        {
            if (Object.ReferenceEquals(x, y)) return true;

            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            return x.CommandId == y.CommandId && x.FunctionId == x.FunctionId && x.RoleId == x.RoleId;
        }


        public int GetHashCode(Permission permission)
        {
            if (Object.ReferenceEquals(permission, null)) return 0;

            int hashProductId = (permission.CommandId + permission.FunctionId + permission.RoleId).GetHashCode();

            return hashProductId;
        }
    }
}