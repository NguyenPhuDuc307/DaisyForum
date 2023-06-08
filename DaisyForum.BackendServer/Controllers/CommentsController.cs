using DaisyForum.BackendServer.Authorization;
using DaisyForum.BackendServer.Constants;
using DaisyForum.BackendServer.Data.Entities;
using DaisyForum.BackendServer.Extensions;
using DaisyForum.BackendServer.Helpers;
using DaisyForum.BackendServer.Models;
using DaisyForum.ViewModels;
using DaisyForum.ViewModels.Contents;
using Google.Cloud.Language.V1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DaisyForum.BackendServer.Controllers;

public partial class KnowledgeBasesController
{
    #region Comments

    [HttpGet("{knowledgeBaseId}/comments/filter")]
    [ClaimRequirement(FunctionCode.CONTENT_COMMENT, CommandCode.VIEW)]
    public async Task<IActionResult> GetCommentsPaging(int? knowledgeBaseId, string filter, int pageIndex, int pageSize)
    {
        var query = from c in _context.Comments
                    join u in _context.Users
                        on c.OwnerUserId equals u.Id
                    select new { c, u };
        if (knowledgeBaseId.HasValue)
        {
            query = query.Where(x => x.c.KnowledgeBaseId == knowledgeBaseId.Value);
        }
        if (!string.IsNullOrEmpty(filter))
        {
            query = query.Where(x => x.c.Content != null && x.c.Content.Contains(filter));
        }
        var totalRecords = await query.CountAsync();
        var items = await query.OrderByDescending(x => x.c.CreateDate)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CommentViewModel()
            {
                Id = c.c.Id,
                Content = c.c.Content,
                CreateDate = c.c.CreateDate,
                KnowledgeBaseId = c.c.KnowledgeBaseId,
                LastModifiedDate = c.c.LastModifiedDate,
                OwnerUserId = c.c.OwnerUserId,
                OwnerName = c.u.FirstName + " " + c.u.LastName
            })
            .ToListAsync();

        var pagination = new Pagination<CommentViewModel>
        {
            Items = items,
            TotalRecords = totalRecords,
        };
        return Ok(pagination);
    }

    [HttpGet("natural-language")]
    public async Task<IActionResult> AnalyzeSentiment([FromQuery] string content)
    {
        var document = new Document
        {
            Content = content,
            Type = Document.Types.Type.Html
        };

        var response = await _languageServiceClient.AnalyzeSentimentAsync(document);

        var sentiment = response.DocumentSentiment;

        var result = new
        {
            Score = sentiment.Score,
            Magnitude = sentiment.Magnitude
        };

        return Ok(result);
    }


    [HttpGet("{knowledgeBaseId}/comments/{commentId}")]
    [ClaimRequirement(FunctionCode.CONTENT_COMMENT, CommandCode.VIEW)]
    public async Task<IActionResult> GetCommentDetail(int commentId)
    {
        var comment = await _context.Comments.FindAsync(commentId);
        if (comment == null)
            return NotFound(new ApiNotFoundResponse($"Cannot found comment with id: {commentId}"));
        var user = await _context.Users.FindAsync(comment.OwnerUserId);
        if (user == null)
            return NotFound(new ApiNotFoundResponse($"Cannot found user with id: {comment.OwnerUserId}"));

        var commentViewModel = new CommentViewModel()
        {
            Id = comment.Id,
            Content = comment.Content,
            CreateDate = comment.CreateDate,
            KnowledgeBaseId = comment.KnowledgeBaseId,
            LastModifiedDate = comment.LastModifiedDate,
            OwnerUserId = comment.OwnerUserId,
            OwnerName = user.FirstName + " " + user.LastName
        };

        return Ok(commentViewModel);
    }

    [HttpPost("{knowledgeBaseId}/comments")]
    public async Task<IActionResult> PostComment(int knowledgeBaseId, [FromBody] CommentCreateRequest request)
    {
        var document = new Document
        {
            Content = request.Content,
            Type = Document.Types.Type.Html
        };

        var response = await _languageServiceClient.AnalyzeSentimentAsync(document);

        var sentiment = response.DocumentSentiment;

        var comment = new Comment()
        {
            Content = request.Content,
            KnowledgeBaseId = knowledgeBaseId,
            OwnerUserId = User.GetUserId(),
            ReplyId = request.ReplyId,
            NavigationScore = sentiment.Score
        };
        _context.Comments.Add(comment);

        var knowledgeBase = await _context.KnowledgeBases.FindAsync(knowledgeBaseId);
        if (knowledgeBase == null)
            return BadRequest(new ApiBadRequestResponse($"Cannot found knowledge base with id: {knowledgeBaseId}"));
        knowledgeBase.NumberOfComments = knowledgeBase.NumberOfComments.GetValueOrDefault(0) + 1;
        _context.KnowledgeBases.Update(knowledgeBase);

        var result = await _context.SaveChangesAsync();
        if (result > 0)
        {
            await _cacheService.RemoveAsync(CacheConstants.RecentComments);

            if (comment.ReplyId.HasValue)
            {
                var repliedComment = await _context.Comments.FindAsync(comment.ReplyId.Value);
                var repledUser = await _context.Users.FindAsync(repliedComment.OwnerUserId);
                var emailModel = new RepliedCommentViewModel()
                {
                    CommentContent = request.Content,
                    KnowledgeBaseId = knowledgeBaseId,
                    KnowledgeBaseSeoAlias = knowledgeBase.SeoAlias,
                    KnowledgeBaseTitle = knowledgeBase.Title,
                    RepliedName = repledUser.FirstName + " " + repledUser.LastName
                };

                var htmlContent = await _viewRenderService.RenderToStringAsync("_RepliedCommentEmail", emailModel);
                await _emailSender.SendEmailAsync(repledUser.Email, "Có người đang trả lời bạn", htmlContent);
            }
            return CreatedAtAction(nameof(GetCommentDetail), new { id = knowledgeBaseId, commentId = comment.Id }, new CommentViewModel()
            {
                Id = comment.Id
            });
        }
        else
        {
            return BadRequest(new ApiBadRequestResponse("Create comment failed"));
        }
    }

    [HttpPut("{knowledgeBaseId}/comments/{commentId}")]
    [ClaimRequirement(FunctionCode.CONTENT_COMMENT, CommandCode.UPDATE)]
    public async Task<IActionResult> PutComment(int commentId, [FromBody] CommentCreateRequest request)
    {
        var document = new Document
        {
            Content = request.Content,
            Type = Document.Types.Type.Html
        };

        var response = await _languageServiceClient.AnalyzeSentimentAsync(document);

        var sentiment = response.DocumentSentiment;
        var comment = await _context.Comments.FindAsync(commentId);
        if (comment == null)
            return BadRequest(new ApiBadRequestResponse($"Cannot found comment with id: {commentId}"));
        if (User.Identity != null)
            if (comment.OwnerUserId != User.GetUserId())
                return Forbid();

        comment.Content = request.Content;
        comment.NavigationScore = sentiment.Score;
        _context.Comments.Update(comment);

        var result = await _context.SaveChangesAsync();

        if (result > 0)
        {
            return NoContent();
        }
        return BadRequest(new ApiBadRequestResponse($"Update comment failed"));
    }

    [HttpDelete("{knowledgeBaseId}/comments/{commentId}")]
    [ClaimRequirement(FunctionCode.CONTENT_COMMENT, CommandCode.DELETE)]
    public async Task<IActionResult> DeleteComment(int knowledgeBaseId, int commentId)
    {
        var comment = await _context.Comments.FindAsync(commentId);
        if (comment == null)
            return NotFound(new ApiNotFoundResponse($"Cannot found the comment with id: {commentId}"));

        _context.Comments.Remove(comment);

        var knowledgeBase = await _context.KnowledgeBases.FindAsync(knowledgeBaseId);
        if (knowledgeBase == null)
            return BadRequest(new ApiBadRequestResponse($"Cannot found knowledge base with id: {knowledgeBaseId}"));
        knowledgeBase.NumberOfComments = knowledgeBase.NumberOfComments.GetValueOrDefault(0) - 1;
        _context.KnowledgeBases.Update(knowledgeBase);

        var result = await _context.SaveChangesAsync();
        if (result > 0)
        {
            await _cacheService.RemoveAsync(CacheConstants.RecentComments);
            var commentViewModel = new CommentViewModel()
            {
                Id = comment.Id,
                Content = comment.Content,
                CreateDate = comment.CreateDate,
                KnowledgeBaseId = comment.KnowledgeBaseId,
                LastModifiedDate = comment.LastModifiedDate,
                OwnerUserId = comment.OwnerUserId
            };
            return Ok(commentViewModel);
        }
        return BadRequest(new ApiBadRequestResponse($"Delete comment failed"));
    }

    [HttpGet("comments/recent/{take}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetRecentComments(int take)
    {
        var cachedData = await _cacheService.GetAsync<List<CommentViewModel>>(CacheConstants.RecentComments);
        if (cachedData == null)
        {
            var query = from c in _context.Comments
                        join u in _context.Users
                            on c.OwnerUserId equals u.Id
                        join k in _context.KnowledgeBases
                        on c.KnowledgeBaseId equals k.Id
                        orderby c.CreateDate descending
                        select new { c, u, k };

            var comments = await query.Take(take).Select(x => new CommentViewModel()
            {
                Id = x.c.Id,
                CreateDate = x.c.CreateDate,
                KnowledgeBaseId = x.c.KnowledgeBaseId,
                OwnerUserId = x.c.OwnerUserId,
                KnowledgeBaseTitle = x.k.Title,
                OwnerName = x.u.FirstName + " " + x.u.LastName,
                KnowledgeBaseSeoAlias = x.k.SeoAlias
            }).ToListAsync();

            await _cacheService.SetAsync(CacheConstants.RecentComments, comments);
            cachedData = comments;
        }

        return Ok(cachedData);
    }


    [HttpGet("{knowledgeBaseId}/comments/tree")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCommentTreeByKnowledgeBaseId(int knowledgeBaseId, int pageIndex, int pageSize)
    {
        var rootCommentsQuery = from c in _context.Comments
                                join u in _context.Users on c.OwnerUserId equals u.Id
                                where c.KnowledgeBaseId == knowledgeBaseId && c.ReplyId == null // chỉ lấy các comment gốc
                                select new { c, u };

        var totalRootComments = await rootCommentsQuery.CountAsync();
        var rootComments = await rootCommentsQuery.OrderByDescending(x => x.c.CreateDate)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new CommentViewModel()
            {
                Id = x.c.Id,
                CreateDate = x.c.CreateDate,
                KnowledgeBaseId = x.c.KnowledgeBaseId,
                OwnerUserId = x.c.OwnerUserId,
                OwnerName = x.u.FirstName + " " + x.u.LastName,
                Content = x.c.Content,
                Children = new Pagination<CommentViewModel>()
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize
                }
            })
            .ToListAsync();

        foreach (var rootComment in rootComments)
        {
            // Set lại PageIndex và PageSize của Children cho đúng giá trị
            rootComment.Children.PageIndex = pageIndex;
            rootComment.Children.PageSize = pageSize;

            rootComment.Children = await GetCommentChildren(rootComment.Id, pageIndex, pageSize);
        }

        return Ok(new Pagination<CommentViewModel>()
        {
            Items = rootComments,
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalRecords = totalRootComments,
        });
    }

    private async Task<Pagination<CommentViewModel>> GetCommentChildren(int commentId, int pageIndex, int pageSize)
    {
        var childCommentsQuery = from c in _context.Comments
                                 join u in _context.Users on c.OwnerUserId equals u.Id
                                 where c.ReplyId == commentId
                                 select new { c, u };

        var totalChildComments = await childCommentsQuery.CountAsync();
        var childComments = await childCommentsQuery.OrderByDescending(x => x.c.CreateDate)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new CommentViewModel()
            {
                Id = x.c.Id,
                CreateDate = x.c.CreateDate,
                KnowledgeBaseId = x.c.KnowledgeBaseId,
                OwnerUserId = x.c.OwnerUserId,
                OwnerName = x.u.FirstName + " " + x.u.LastName,
                Content = x.c.Content,
                Children = new Pagination<CommentViewModel>()
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize
                }
            })
            .ToListAsync();

        // Đệ quy lấy danh sách các comment con của từng comment con
        foreach (var childComment in childComments)
        {
            // Set lại PageIndex và PageSize của Children cho đúng giá trị
            childComment.Children.PageIndex = pageIndex;
            childComment.Children.PageSize = pageSize;

            childComment.Children = await GetCommentChildren(childComment.Id, pageIndex, pageSize);
        }

        return new Pagination<CommentViewModel>()
        {
            Items = childComments,
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalRecords = totalChildComments
        };
    }

    [HttpGet("{knowledgeBaseId}/comments/{rootCommentId}/replied")]
    [AllowAnonymous]
    public async Task<IActionResult> GetRepliedCommentsPaging(int knowledgeBaseId, int rootCommentId, int pageIndex, int pageSize)
    {
        var query = from c in _context.Comments
                    join u in _context.Users
                        on c.OwnerUserId equals u.Id
                    where c.KnowledgeBaseId == knowledgeBaseId
                    where c.ReplyId == rootCommentId
                    select new { c, u };

        var totalRecords = await query.CountAsync();
        var comments = await query.OrderByDescending(x => x.c.CreateDate)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new CommentViewModel()
            {
                Id = x.c.Id,
                CreateDate = x.c.CreateDate,
                KnowledgeBaseId = x.c.KnowledgeBaseId,
                OwnerUserId = x.c.OwnerUserId,
                OwnerName = x.u.FirstName + " " + x.u.LastName,
                Content = x.c.Content
            })
            .ToListAsync();

        return Ok(new Pagination<CommentViewModel>
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            Items = comments,
            TotalRecords = totalRecords
        });
    }

    #endregion Comments
}