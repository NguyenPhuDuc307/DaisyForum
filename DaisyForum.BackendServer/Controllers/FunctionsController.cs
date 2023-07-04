using DaisyForum.BackendServer.Authorization;
using DaisyForum.BackendServer.Constants;
using DaisyForum.BackendServer.Data;
using DaisyForum.BackendServer.Data.Entities;
using DaisyForum.BackendServer.Helpers;
using DaisyForum.ViewModels;
using DaisyForum.ViewModels.Systems;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DaisyForum.BackendServer.Controllers;

public class FunctionsController : BaseController
{
    private readonly ApplicationDbContext _context;

    public FunctionsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    [ClaimRequirement(FunctionCode.SYSTEM_FUNCTION, CommandCode.CREATE)]
    public async Task<IActionResult> PostFunction([FromBody] FunctionCreateRequest request)
    {
        var dbFunction = await _context.Functions.FindAsync(request.Id);
        if (dbFunction != null)
            return BadRequest($"Function with id {request.Id} is existed.");

        var function = new Function()
        {
            Id = request.Id,
            Name = request.Name,
            ParentId = request.ParentId,
            SortOrder = request.SortOrder,
            Url = request.Url,
            Icon = request.Icon
        };
        _context.Functions.Add(function);
        var result = await _context.SaveChangesAsync();

        if (result > 0)
        {
            return CreatedAtAction(nameof(GetById), new { id = function.Id }, request);
        }
        else
        {
            return BadRequest(new ApiBadRequestResponse($"Create function is failed"));
        }
    }

    [HttpGet]
    [ClaimRequirement(FunctionCode.SYSTEM_FUNCTION, CommandCode.VIEW)]
    public async Task<IActionResult> GetFunctions()
    {
        var query = _context.Functions.AsQueryable();
        query = query.Where(x => x.ParentId == null);
        var items = await query.ToListAsync();

        var functionViewModels = items.Select(c => new FunctionTreeViewModel
        {
            Id = c.Id,
            Name = c.Name,
            Url = c.Url,
            SortOrder = c.SortOrder,
            ParentId = c.ParentId,
            Icon = c.Icon,
            Children = GetChildrenFunctions(c.Id)
        }).ToList();

        return Ok(functionViewModels);
    }

    private List<FunctionTreeViewModel>? GetChildrenFunctions(string? parentId)
    {
        var query = _context.Functions.Where(x => x.ParentId == parentId);

        var items = query.ToList();

        if (items.Count == 0)
        {
            return null;
        }

        return items.Select(c => new FunctionTreeViewModel
        {
            Id = c.Id,
            Name = c.Name,
            Url = c.Url,
            SortOrder = c.SortOrder,
            ParentId = c.ParentId,
            Icon = c.Icon,
            Children = GetChildrenFunctions(c.Id)
        }).ToList();
    }

    [HttpGet("{functionId}/parents")]
    [ClaimRequirement(FunctionCode.SYSTEM_FUNCTION, CommandCode.VIEW)]
    public async Task<IActionResult> GetFunctionsByParentId(string functionId)
    {
        var functions = _context.Functions.Where(x => x.ParentId == functionId);

        var functionViewModels = await functions.Select(u => new FunctionViewModel()
        {
            Id = u.Id,
            Name = u.Name,
            Url = u.Url,
            SortOrder = u.SortOrder,
            ParentId = u.ParentId,
            Icon = u.Icon
        }).ToListAsync();

        return Ok(functionViewModels);
    }

    [HttpGet("filter")]
    [ClaimRequirement(FunctionCode.SYSTEM_FUNCTION, CommandCode.VIEW)]
    public async Task<IActionResult> GetFunctionsPaging(string? keyword, int page = 1, int pageSize = 10)
    {
        var query = _context.Functions.AsQueryable();
        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(x =>
                (x.Id != null && x.Id.Contains(keyword))
                || (x.Name != null && x.Name.Contains(keyword))
                || (x.Url != null && x.Url.Contains(keyword)));
        }
        query = query.Where(x => x.ParentId == null);

        var totalRecords = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize)
            .Take(pageSize).ToListAsync();

        var data = items.Select(c => new FunctionTreeViewModel
        {
            Id = c.Id,
            Name = c.Name,
            Url = c.Url,
            SortOrder = c.SortOrder,
            ParentId = c.ParentId,
            Icon = c.Icon,
            Children = GetChildrenFunctions(c.Id)
        }).OrderBy(x => x.SortOrder).ToList();

        var pagination = new Pagination<FunctionTreeViewModel>
        {
            PageIndex = page,
            PageSize = pageSize,
            Items = data,
            TotalRecords = totalRecords,
        };

        return Ok(pagination);
    }

    [HttpGet("{id}")]
    [ClaimRequirement(FunctionCode.SYSTEM_FUNCTION, CommandCode.VIEW)]
    public async Task<IActionResult> GetById(string id)
    {
        var function = await _context.Functions.FindAsync(id);
        if (function == null)
            return NotFound(new ApiNotFoundResponse($"Cannot found function with id {id}"));

        var functionViewModel = new FunctionViewModel()
        {
            Id = function.Id,
            Name = function.Name,
            Url = function.Url,
            SortOrder = function.SortOrder,
            ParentId = function.ParentId,
            Icon = function.Icon
        };
        return Ok(functionViewModel);
    }

    [HttpPut("{id}")]
    [ClaimRequirement(FunctionCode.SYSTEM_FUNCTION, CommandCode.UPDATE)]
    [ApiValidationFilter]
    public async Task<IActionResult> PutFunction(string id, [FromBody] FunctionCreateRequest request)
    {
        var function = await _context.Functions.FindAsync(id);
        if (function == null)
            return NotFound(new ApiNotFoundResponse($"Cannot found function with id {id}"));

        function.Name = request.Name;
        function.ParentId = request.ParentId;
        function.SortOrder = request.SortOrder;
        function.Url = request.Url;
        function.Icon = request.Icon;

        _context.Functions.Update(function);
        var result = await _context.SaveChangesAsync();

        if (result > 0)
        {
            return NoContent();
        }
        return BadRequest(new ApiBadRequestResponse("Create function failed"));
    }

    [HttpDelete("{id}")]
    [ClaimRequirement(FunctionCode.SYSTEM_FUNCTION, CommandCode.DELETE)]
    public async Task<IActionResult> DeleteFunction(string id)
    {
        var function = await _context.Functions.FindAsync(id);
        if (function == null)
            return NotFound(new ApiNotFoundResponse($"Cannot found function with id {id}"));

        _context.Functions.Remove(function);

        var commands = _context.CommandInFunctions.Where(x => x.FunctionId == id);
        _context.CommandInFunctions.RemoveRange(commands);

        var result = await _context.SaveChangesAsync();
        if (result > 0)
        {
            var functionViewModel = new FunctionViewModel()
            {
                Id = function.Id,
                Name = function.Name,
                Url = function.Url,
                SortOrder = function.SortOrder,
                ParentId = function.ParentId,
                Icon = function.Icon
            };
            return Ok(functionViewModel);
        }
        return BadRequest(new ApiBadRequestResponse("Delete function failed"));
    }

    [HttpGet("{functionId}/commands")]
    [ClaimRequirement(FunctionCode.SYSTEM_FUNCTION, CommandCode.VIEW)]
    public async Task<IActionResult> GetCommandsInFunction(string functionId)
    {
        var query = from a in _context.Commands
                    join cif in _context.CommandInFunctions on a.Id equals cif.CommandId into result1
                    from commandInFunction in result1.DefaultIfEmpty()
                    join f in _context.Functions on commandInFunction.FunctionId equals f.Id into result2
                    from function in result2.DefaultIfEmpty()
                    select new
                    {
                        a.Id,
                        a.Name,
                        commandInFunction.FunctionId
                    };

        query = query.Where(x => x.FunctionId == functionId);

        var data = await query.Select(x => new CommandViewModel()
        {
            Id = x.Id,
            Name = x.Name
        }).ToListAsync();

        return Ok(data);
    }

    [HttpPost("{functionId}/commands")]
    [ClaimRequirement(FunctionCode.SYSTEM_FUNCTION, CommandCode.CREATE)]
    [ApiValidationFilter]
    public async Task<IActionResult> PostCommandToFunction(string functionId, [FromBody] CommandAssignRequest request)
    {
        if (request.CommandIds == null)
            return BadRequest(new ApiBadRequestResponse("This command has been existed in function"));
        foreach (var commandId in request.CommandIds)
        {
            if (await _context.CommandInFunctions.FindAsync(commandId, functionId) != null)
                return BadRequest(new ApiBadRequestResponse("This command has been existed in function"));

            var entity = new CommandInFunction()
            {
                CommandId = commandId,
                FunctionId = functionId
            };

            _context.CommandInFunctions.Add(entity);
        }

        if (request.AddToAllFunctions)
        {
            var otherFunctions = _context.Functions.Where(x => x.ParentId == functionId);

            foreach (var function in otherFunctions)
            {
                foreach (var commandId in request.CommandIds)
                {
                    if (await _context.CommandInFunctions.FindAsync(commandId, function.Id) == null)
                    {
                        _context.CommandInFunctions.Add(new CommandInFunction()
                        {
                            CommandId = commandId,
                            FunctionId = function.Id
                        });
                    }
                }
            }
        }
        var result = await _context.SaveChangesAsync();

        if (result > 0)
        {
            return CreatedAtAction(nameof(GetById), new { request.CommandIds, functionId });
        }
        else
        {
            return BadRequest(new ApiBadRequestResponse("Add command to function failed"));
        }
    }

    [HttpDelete("{functionId}/commands")]
    [ClaimRequirement(FunctionCode.SYSTEM_FUNCTION, CommandCode.UPDATE)]
    public async Task<IActionResult> DeleteCommandToFunction(string functionId, [FromQuery] CommandAssignRequest request)
    {
        if (request.CommandIds == null)
            return BadRequest(new ApiBadRequestResponse("This command has been existed in function"));
        foreach (var commandId in request.CommandIds)
        {
            var entity = await _context.CommandInFunctions.FindAsync(commandId, functionId);
            if (entity == null)
                return BadRequest(new ApiBadRequestResponse("This command is not existed in function"));

            _context.CommandInFunctions.Remove(entity);
        }

        var result = await _context.SaveChangesAsync();

        if (result > 0)
        {
            return Ok();
        }
        else
        {
            return BadRequest(new ApiBadRequestResponse("Delete command to function failed"));
        }
    }
}