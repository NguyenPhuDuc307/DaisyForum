using DaisyForum.BackendServer.Authorization;
using DaisyForum.BackendServer.Constants;
using DaisyForum.BackendServer.Data;
using DaisyForum.ViewModels.Statistics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DaisyForum.BackendServer.Controllers;

public class StatisticsController : BaseController
{
    private readonly ApplicationDbContext _context;

    public StatisticsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("monthly-comments")]
    [ClaimRequirement(FunctionCode.STATISTIC, CommandCode.VIEW)]
    public async Task<IActionResult> GetMonthlyNewComments(int year)
    {
        var data = await _context.Comments.Where(x => x.CreateDate.Date.Year == year)
            .GroupBy(x => x.CreateDate.Date.Month)
            .OrderBy(x => x.Key)
            .Select(g => new MonthlyCommentsViewModel()
            {
                Month = g.Key,
                NumberOfComments = g.Count()
            })
            .ToListAsync();

        return Ok(data);
    }

    [HttpGet("monthly-newKnowledgeBases")]
    [ClaimRequirement(FunctionCode.STATISTIC, CommandCode.VIEW)]
    public async Task<IActionResult> GetMonthlyNewKnowledgeBases(int year)
    {
        var data = await _context.KnowledgeBases.Where(x => x.CreateDate.Date.Year == year)
            .GroupBy(x => x.CreateDate.Date.Month)
            .Select(g => new MonthlyNewKnowledgeBasesViewModel()
            {
                Month = g.Key,
                NumberOfNewKnowledgeBases = g.Count()
            })
            .ToListAsync();

        return Ok(data);
    }

    [HttpGet("monthly-registers")]
    [ClaimRequirement(FunctionCode.STATISTIC, CommandCode.VIEW)]
    public async Task<IActionResult> GetMonthlyNewRegisters(int year)
    {
        var data = await _context.Users.Where(x => x.CreateDate.Date.Year == year)
           .GroupBy(x => x.CreateDate.Date.Month)
           .Select(g => new MonthlyNewKnowledgeBasesViewModel()
           {
               Month = g.Key,
               NumberOfNewKnowledgeBases = g.Count()
           })
           .ToListAsync();

        return Ok(data);
    }
}