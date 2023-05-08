using DaisyForum.BackendServer.Data;
using DaisyForum.BackendServer.Helpers;
using DaisyForum.ViewModels.Contents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DaisyForum.BackendServer.Controllers;

public class LabelsController : BaseController
{
    private readonly ApplicationDbContext _context;

    public LabelsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("popular/{take:int}")]
    [AllowAnonymous]
    public async Task<List<LabelViewModel>> GetPopularLabels(int take)
    {
        var query = from l in _context.Labels
                    join lik in _context.LabelInKnowledgeBases on l.Id equals lik.LabelId
                    group new { l.Id, l.Name } by new { l.Id, l.Name } into g
                    select new
                    {
                        g.Key.Id,
                        g.Key.Name,
                        Count = g.Count()
                    };
        var labels = await query.OrderByDescending(x => x.Count).Take(take)
            .Select(l => new LabelViewModel()
            {
                Id = l.Id,
                Name = l.Name
            }).ToListAsync();

        return labels;
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(string id)
    {
        var label = await _context.Labels.FindAsync(id);
        if (label == null)
            return NotFound(new ApiNotFoundResponse($"Label with id: {id} is not found"));

        var labelVm = new LabelViewModel()
        {
            Id = label.Id,
            Name = label.Name
        };

        return Ok(labelVm);
    }
}