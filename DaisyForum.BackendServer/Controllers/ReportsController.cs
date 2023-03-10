using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DaisyForum.BackendServer.Data.Entities;
using DaisyForum.BackendServer.Helpers;
using DaisyForum.ViewModels;
using DaisyForum.ViewModels.Contents;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DaisyForum.BackendServer.Controllers
{
    public partial class KnowledgeBasesController
    {
        #region Reports

        [HttpGet("{knowledgeBaseId}/reports/filter")]
        public async Task<IActionResult> GetReportsPaging(int knowledgeBaseId, string filter, int pageIndex, int pageSize)
        {
            var query = _context.Reports.Where(x => x.KnowledgeBaseId == knowledgeBaseId).AsQueryable();
            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(x => x.Content != null && x.Content.Contains(filter));
            }
            var totalRecords = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1 * pageSize))
                .Take(pageSize)
                .Select(c => new ReportViewModel()
                {
                    Id = c.Id,
                    Content = c.Content,
                    CreateDate = c.CreateDate,
                    KnowledgeBaseId = c.KnowledgeBaseId,
                    LastModifiedDate = c.LastModifiedDate,
                    IsProcessed = false,
                    ReportUserId = c.ReportUserId
                })
                .ToListAsync();

            var pagination = new Pagination<ReportViewModel>
            {
                Items = items,
                TotalRecords = totalRecords,
            };
            return Ok(pagination);
        }

        [HttpGet("{knowledgeBaseId}/reports/{reportId}")]
        public async Task<IActionResult> GetReportDetail(int knowledgeBaseId, int reportId)
        {
            var report = await _context.Reports.FindAsync(reportId);
            if (report == null)
                return NotFound(new ApiNotFoundResponse($"Cannot found report with id {reportId}"));

            var reportViewModel = new ReportViewModel()
            {
                Id = report.Id,
                Content = report.Content,
                CreateDate = report.CreateDate,
                KnowledgeBaseId = report.KnowledgeBaseId,
                LastModifiedDate = report.LastModifiedDate,
                IsProcessed = report.IsProcessed,
                ReportUserId = report.ReportUserId
            };

            return Ok(reportViewModel);
        }

        [HttpPost("{knowledgeBaseId}/reports")]
        public async Task<IActionResult> PostReport(int knowledgeBaseId, [FromBody] ReportCreateRequest request)
        {
            var report = new Report()
            {
                Content = request.Content,
                KnowledgeBaseId = knowledgeBaseId,
                ReportUserId = request.ReportUserId,
                IsProcessed = false
            };
            _context.Reports.Add(report);

            var knowledgeBase = await _context.KnowledgeBases.FindAsync(knowledgeBaseId);
            if (knowledgeBase == null)
                return BadRequest(new ApiBadRequestResponse($"Cannot found knowledge base with id {knowledgeBaseId}"));
                
            knowledgeBase.NumberOfComments = knowledgeBase.NumberOfReports.GetValueOrDefault(0) + 1;
            _context.KnowledgeBases.Update(knowledgeBase);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return Ok();
            }
            else
            {
                return BadRequest(new ApiBadRequestResponse($"Create report failed"));
            }
        }

        [HttpPut("{knowledgeBaseId}/reports/{reportId}")]
        public async Task<IActionResult> PutReport(int reportId, [FromBody] CommentCreateRequest request)
        {
            var report = await _context.Reports.FindAsync(reportId);
            if (report == null)
                return BadRequest(new ApiNotFoundResponse($"Cannot found report with id {reportId}"));
            if (User.Identity != null)
                if (report.ReportUserId != User.Identity.Name)
                    return Forbid();

            report.Content = request.Content;
            _context.Reports.Update(report);

            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                return NoContent();
            }
            return BadRequest(new ApiBadRequestResponse($"Update report failed"));
        }

        [HttpDelete("{knowledgeBaseId}/reports/{reportId}")]
        public async Task<IActionResult> DeleteReport(int knowledgeBaseId, int reportId)
        {
            var report = await _context.Reports.FindAsync(reportId);
            if (report == null)
                return NotFound(new ApiNotFoundResponse($"Cannot found the report with id: {reportId}"));

            _context.Reports.Remove(report);

            var knowledgeBase = await _context.KnowledgeBases.FindAsync(knowledgeBaseId);
            if (knowledgeBase == null)
                return BadRequest(new ApiBadRequestResponse($"Cannot found report with id {reportId}"));
            knowledgeBase.NumberOfComments = knowledgeBase.NumberOfReports.GetValueOrDefault(0) - 1;
            _context.KnowledgeBases.Update(knowledgeBase);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return Ok();
            }
            return BadRequest(new ApiBadRequestResponse($"Delete report failed"));
        }

        #endregion Reports
    }
}