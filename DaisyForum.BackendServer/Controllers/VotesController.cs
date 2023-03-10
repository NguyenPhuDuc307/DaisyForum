using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DaisyForum.BackendServer.Data.Entities;
using DaisyForum.BackendServer.Helpers;
using DaisyForum.ViewModels.Contents;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DaisyForum.BackendServer.Controllers
{
    public partial class KnowledgeBasesController
    {
        #region Votes

        [HttpGet("{knowledgeBaseId}/votes")]
        public async Task<IActionResult> GetVotes(int knowledgeBaseId)
        {
            var votes = await _context.Votes
                .Where(x => x.KnowledgeBaseId == knowledgeBaseId)
                .Select(x => new VoteViewModel()
                {
                    UserId = x.UserId,
                    KnowledgeBaseId = x.KnowledgeBaseId,
                    CreateDate = x.CreateDate,
                    LastModifiedDate = x.LastModifiedDate
                }).ToListAsync();

            return Ok(votes);
        }

        [HttpPost("{knowledgeBaseId}/votes")]
        public async Task<IActionResult> PostVote(int knowledgeBaseId, [FromBody] VoteCreateRequest request)
        {
            var vote = await _context.Votes.FindAsync(knowledgeBaseId, request.UserId);
            if (vote != null)
                return BadRequest(new ApiBadRequestResponse("This user has been voted for this Knowledge Bases"));

            vote = new Vote()
            {
                KnowledgeBaseId = knowledgeBaseId,
                UserId = request.UserId
            };
            _context.Votes.Add(vote);

            var knowledgeBase = await _context.KnowledgeBases.FindAsync(knowledgeBaseId);
            if (knowledgeBase == null)
                return BadRequest(new ApiBadRequestResponse($"Cannot found knowledge base with id {knowledgeBaseId}"));
            knowledgeBase.NumberOfVotes = knowledgeBase.NumberOfVotes.GetValueOrDefault(0) + 1;
            _context.KnowledgeBases.Update(knowledgeBase);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return NoContent();
            }
            else
            {
                return BadRequest(new ApiBadRequestResponse($"Vote failed"));
            }
        }

        [HttpDelete("{knowledgeBaseId}/votes/{userId}")]
        public async Task<IActionResult> DeleteComment(int knowledgeBaseId, string userId)
        {
            var vote = await _context.Votes.FindAsync(knowledgeBaseId, userId);
            if (vote == null)
                return NotFound(new ApiNotFoundResponse("Cannot found vote"));

            var knowledgeBase = await _context.KnowledgeBases.FindAsync(knowledgeBaseId);
            if (knowledgeBase == null)
                return BadRequest(new ApiBadRequestResponse($"Cannot found knowledge base with id {knowledgeBaseId}"));
            knowledgeBase.NumberOfVotes = knowledgeBase.NumberOfVotes.GetValueOrDefault(0) - 1;
            _context.KnowledgeBases.Update(knowledgeBase);

            _context.Votes.Remove(vote);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return Ok();
            }
            return BadRequest(new ApiBadRequestResponse($"Delete vote failed"));
        }

        #endregion Votes
    }
}