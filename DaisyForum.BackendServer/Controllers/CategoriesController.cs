using DaisyForum.BackendServer.Data;
using DaisyForum.BackendServer.Data.Entities;
using DaisyForum.ViewModels;
using DaisyForum.ViewModels.Contents;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DaisyForum.BackendServer.Authorization;
using DaisyForum.BackendServer.Constants;
using DaisyForum.BackendServer.Helpers;
using Microsoft.AspNetCore.Authorization;
using DaisyForum.BackendServer.Services;

namespace DaisyForum.BackendServer.Controllers;

public class CategoriesController : BaseController
{
    private readonly ApplicationDbContext _context;
    private readonly ICacheService _cacheService;

    public CategoriesController(ApplicationDbContext context,
            ICacheService cacheService)
    {
        _context = context;
        _cacheService = cacheService;
    }

    [HttpPost]
    [ClaimRequirement(FunctionCode.CONTENT_CATEGORY, CommandCode.CREATE)]
    public async Task<IActionResult> PostCategory([FromBody] CategoryCreateRequest request)
    {
        var category = new Category()
        {
            Name = request.Name,
            ParentId = request.ParentId,
            SortOrder = request.SortOrder,
            SeoAlias = request.SeoAlias,
            SeoDescription = request.SeoDescription
        };
        _context.Categories.Add(category);
        var result = await _context.SaveChangesAsync();

        if (result > 0)
        {
            await _cacheService.RemoveAsync("Categories");
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, request);
        }
        else
        {
            return BadRequest(new ApiBadRequestResponse("Create category failed"));
        }
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetCategories()
    {
        var cachedData = await _cacheService.GetAsync<List<CategoryViewModel>>("Categories");
        if (cachedData == null)
        {
            var categories = await _context.Categories.ToListAsync();

            var categoryViewModels = categories.Select(c => CreateCategoryViewModel(c)).ToList();
            await _cacheService.SetAsync("Categories", categoryViewModels);
            cachedData = categoryViewModels;
        }

        return Ok(cachedData);
    }

    [HttpGet("filter")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCategoriesPaging(string? keyword, int page = 1, int pageSize = 10)
    {
        var query = _context.Categories.AsQueryable();
        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(x => x.Name != null && x.Name.Contains(keyword));
        }
        var totalRecords = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize)
            .Take(pageSize).ToListAsync();

        var data = items.Select(c => CreateCategoryViewModel(c)).ToList();

        var pagination = new Pagination<CategoryViewModel>
        {
            Items = data,
            TotalRecords = totalRecords,
        };
        return Ok(pagination);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
            return NotFound(new ApiNotFoundResponse($"Category with id: {id} is not found"));

        CategoryViewModel categoryViewModel = CreateCategoryViewModel(category);

        return Ok(categoryViewModel);
    }

    [HttpPut("{id}")]
    [ClaimRequirement(FunctionCode.CONTENT_CATEGORY, CommandCode.UPDATE)]
    public async Task<IActionResult> PutCategory(int id, [FromBody] CategoryCreateRequest request)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
            return NotFound(new ApiNotFoundResponse($"Category with id: {id} is not found"));

        if (id == request.ParentId)
        {
            return BadRequest(new ApiBadRequestResponse("Category cannot be a child itself."));
        }

        category.Name = request.Name;
        category.ParentId = request.ParentId;
        category.SortOrder = request.SortOrder;
        category.SeoDescription = request.SeoDescription;
        category.SeoAlias = request.SeoAlias;

        _context.Categories.Update(category);
        var result = await _context.SaveChangesAsync();

        if (result > 0)
        {
            await _cacheService.RemoveAsync("Categories");
            return NoContent();
        }
        return BadRequest(new ApiBadRequestResponse("Update category failed"));
    }

    [HttpDelete("{id}")]
    [ClaimRequirement(FunctionCode.CONTENT_CATEGORY, CommandCode.DELETE)]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
            return NotFound(new ApiNotFoundResponse($"Category with id: {id} is not found"));

        _context.Categories.Remove(category);
        var result = await _context.SaveChangesAsync();
        if (result > 0)
        {
            await _cacheService.RemoveAsync("Categories");
            CategoryViewModel categoryViewModel = CreateCategoryViewModel(category);
            return Ok(categoryViewModel);
        }
        return BadRequest(new ApiBadRequestResponse("Delete category failed"));
    }

    private static CategoryViewModel CreateCategoryViewModel(Category category)
    {
        return new CategoryViewModel()
        {
            Id = category.Id,
            Name = category.Name,
            SortOrder = category.SortOrder,
            ParentId = category.ParentId,
            NumberOfTickets = category.NumberOfTickets,
            SeoDescription = category.SeoDescription,
            SeoAlias = category.SeoAlias
        };
    }
}