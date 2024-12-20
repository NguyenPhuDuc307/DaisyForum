using DaisyForum.BackendServer.Authorization;
using DaisyForum.BackendServer.Constants;
using DaisyForum.BackendServer.Data;
using DaisyForum.BackendServer.Data.Entities;
using DaisyForum.BackendServer.Extensions;
using DaisyForum.BackendServer.Helpers;
using DaisyForum.BackendServer.Services;
using DaisyForum.ViewModels;
using DaisyForum.ViewModels.Contents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using Google.Cloud.Language.V1;
using DaisyForum.ViewModels.Systems;
using DaisyForum.BackendServer.Models;

namespace DaisyForum.BackendServer.Controllers;

public partial class KnowledgeBasesController : BaseController
{
    private readonly ApplicationDbContext _context;
    private readonly ISequenceService _sequenceService;
    private readonly IStorageService _storageService;
    private readonly ILogger<KnowledgeBasesController> _logger;
    private readonly IEmailSender _emailSender;
    private readonly IViewRenderService _viewRenderService;
    private readonly ICacheService _cacheService;
    private readonly IOneSignalService _oneSignalService;
    private readonly IContentBasedService _cSVService;
    private readonly LanguageServiceClient _languageServiceClient;

    public KnowledgeBasesController(ApplicationDbContext context,
            ISequenceService sequenceService,
            IStorageService storageService,
            ILogger<KnowledgeBasesController> logger,
            IEmailSender emailSender,
            IViewRenderService viewRenderService,
            ICacheService cacheService,
            IOneSignalService oneSignalService,
            IContentBasedService cSVService)
    {
        _context = context;
        _sequenceService = sequenceService;
        _storageService = storageService;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _emailSender = emailSender;
        _viewRenderService = viewRenderService;
        _cacheService = cacheService;
        _oneSignalService = oneSignalService;
        _cSVService = cSVService;

        var credentialsPath = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");

        if (string.IsNullOrEmpty(credentialsPath))
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var filePath = Path.Combine(basePath, "google-natural-language.json");
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", filePath);
        }

        _languageServiceClient = LanguageServiceClient.Create();
    }

    public async Task<UserViewModel?> GetEmailUser(string id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return null;

        var userViewModel = new UserViewModel()
        {
            Email = user.Email
        };
        return userViewModel;
    }

    public async Task<string> GetEmailNotification(string userId)
    {
        var query = from f in _context.Followers
                    where f.OwnerUserId == userId && f.Notification == true
                    select new { f };

        int totalRecords = await query.CountAsync();

        var followers = await query
            .Select(u => new FollowerViewModel()
            {
                FollowerId = u.f.FollowerId,
                Notification = u.f.Notification
            }).ToListAsync();
        var items = new List<FollowerViewModel>();
        foreach (var follower in followers)
        {
            var followerViewModel = new FollowerViewModel()
            {
                FollowerId = follower.FollowerId,
                Notification = follower.Notification
            };

            followerViewModel.Follower = await GetEmailUser(follower.FollowerId);
            items.Add(followerViewModel);
        }

        string emailList = string.Join(",", items
            .Where(item => item.Follower != null)
            .Select(item => item.Follower.Email));

        return emailList;
    }

    [HttpPost]
    [ClaimRequirement(FunctionCode.CONTENT_KNOWLEDGE_BASE, CommandCode.CREATE)]
    [ApiValidationFilter]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> PostKnowledgeBase([FromForm] KnowledgeBaseCreateRequest request)
    {
        _logger.LogInformation("Begin PostKnowledgeBase API");
        KnowledgeBase knowledgeBase = CreateKnowledgeBaseEntity(request);
        knowledgeBase.OwnerUserId = User.GetUserId();
        var user = await _context.Users.FindAsync(User.GetUserId());
        if (string.IsNullOrEmpty(knowledgeBase.SeoAlias))
        {
            knowledgeBase.SeoAlias = TextHelper.ToUnsignedString(knowledgeBase.Title != null ? knowledgeBase.Title : "");
        }
        knowledgeBase.Id = await _sequenceService.GetKnowledgeBaseNewId();

        //Process attachment
        if (request.Attachments != null && request.Attachments.Count > 0)
        {
            foreach (var attachment in request.Attachments)
            {
                var attachmentEntity = await SaveFile(knowledgeBase.Id, attachment);
                _context.Attachments.Add(attachmentEntity);
            }
        }
        _context.KnowledgeBases.Add(knowledgeBase);

        //Process label
        if (request.Labels?.Length > 0)
        {
            await ProcessLabel(request, knowledgeBase);
        }

        var result = await _context.SaveChangesAsync();
        // var htmlContent = await _viewRenderService.RenderToStringAsync("_RepliedCommentEmail", emailModel);
        var emailModel = new PostKnowledgeBasesViewModel()
        {
            OwnerName = user.FirstName + " " + user.LastName,
            KnowledgeBaseId = knowledgeBase.Id,
            KnowledgeBaseTitle = knowledgeBase.Title,
            KnowledgeBaseSeoAlias = knowledgeBase.SeoAlias
        };
        var htmlContent = await _viewRenderService.RenderToStringAsync("_PostKnowledgeBasesEmail", emailModel);
        await _emailSender.SendEmailAsync(await GetEmailNotification(User.GetUserId()), user.FirstName + " " + user.LastName + " vừa đăng một bài mới.", htmlContent);

        if (result > 0)
        {
            await _cacheService.RemoveAsync(CacheConstants.LatestKnowledgeBases);
            await _cacheService.RemoveAsync(CacheConstants.PopularKnowledgeBases);
            await _cacheService.RemoveAsync(CacheConstants.PopularLabels);
            _logger.LogInformation("End PostKnowledgeBase API - Success");
            await _oneSignalService.SendAsync(request.Title, request.Description,
                     string.Format(CommonConstants.KnowledgeBaseUrl, knowledgeBase.SeoAlias, knowledgeBase.Id));
            return CreatedAtAction(nameof(GetById), new
            {
                id = knowledgeBase.Id
            });
        }
        else
        {
            _logger.LogInformation("End PostKnowledgeBase API - Failed");
            return BadRequest(new ApiBadRequestResponse("Create knowledge failed"));
        }
    }

    [HttpGet]
    [ClaimRequirement(FunctionCode.CONTENT_KNOWLEDGE_BASE, CommandCode.VIEW)]
    public async Task<IActionResult> GetKnowledgeBases()
    {
        var knowledgeBases = _context.KnowledgeBases;

        var knowledgeBaseViewModels = await knowledgeBases.Select(u => new KnowledgeBaseQuickViewModel()
        {
            Id = u.Id,
            CategoryId = u.CategoryId,
            Description = u.Description,
            SeoAlias = u.SeoAlias,
            Title = u.Title,
            IsProcessed = u.IsProcessed
        }).ToListAsync();

        return Ok(knowledgeBaseViewModels);
    }

    [HttpGet("filter")]
    [AllowAnonymous]
    public async Task<IActionResult> GetKnowledgeBasesPaging(string filter, int? categoryId, int pageIndex = 1, int pageSize = 10)
    {
        var query = from k in _context.KnowledgeBases
                    join c in _context.Categories on k.CategoryId equals c.Id
                    select new { k, c };
        if (!string.IsNullOrEmpty(filter))
        {
            query = query.Where(x => x.k.Title != null && x.k.Title.Contains(filter));
        }
        if (categoryId.HasValue)
        {
            query = query.Where(x => x.k.CategoryId == categoryId.Value);
        }
        var totalRecords = await query.CountAsync();
        var items = await query.Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new KnowledgeBaseQuickViewModel()
            {
                Id = u.k.Id,
                CategoryId = u.k.CategoryId,
                Description = u.k.Description,
                SeoAlias = u.k.SeoAlias,
                Title = u.k.Title,
                CategoryAlias = u.c.SeoAlias,
                CategoryName = u.c.Name,
                NumberOfVotes = u.k.NumberOfVotes,
                CreateDate = u.k.CreateDate,
                NumberOfComments = u.k.NumberOfComments,
                NumberOfReports = u.k.NumberOfReports,
                IsProcessed = u.k.IsProcessed
            })
            .ToListAsync();

        var pagination = new Pagination<KnowledgeBaseQuickViewModel>
        {
            PageSize = pageSize,
            PageIndex = pageIndex,
            Items = items,
            TotalRecords = totalRecords,
        };
        return Ok(pagination);
    }

    [HttpGet("latest/{take:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetLatestKnowledgeBases(int take)
    {
        var cachedData = await _cacheService.GetAsync<List<KnowledgeBaseQuickViewModel>>(CacheConstants.LatestKnowledgeBases);
        if (cachedData == null)
        {
            var knowledgeBases = from k in _context.KnowledgeBases
                                 join c in _context.Categories on k.CategoryId equals c.Id
                                 orderby k.CreateDate descending
                                 select new { k, c };

            var knowledgeBaseViewModels = await knowledgeBases.Take(take)
                .Select(u => new KnowledgeBaseQuickViewModel()
                {
                    Id = u.k.Id,
                    CategoryId = u.k.CategoryId,
                    Description = u.k.Description,
                    SeoAlias = u.k.SeoAlias,
                    Title = u.k.Title,
                    CategoryAlias = u.c.SeoAlias,
                    CategoryName = u.c.Name,
                    NumberOfVotes = u.k.NumberOfVotes,
                    CreateDate = u.k.CreateDate,
                    IsProcessed = u.k.IsProcessed,
                    Labels = TextHelper.Split(u.k.Labels, ",")
                }).ToListAsync();
            await _cacheService.SetAsync(CacheConstants.LatestKnowledgeBases, knowledgeBaseViewModels, 2);
            cachedData = knowledgeBaseViewModels;
        }

        return Ok(cachedData);
    }

    [HttpGet("popular/{take:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPopularKnowledgeBases(int take)
    {
        var cachedData = await _cacheService.GetAsync<List<KnowledgeBaseQuickViewModel>>(CacheConstants.PopularKnowledgeBases);
        if (cachedData == null)
        {
            var knowledgeBases = from k in _context.KnowledgeBases
                                 join c in _context.Categories on k.CategoryId equals c.Id
                                 orderby k.ViewCount descending
                                 select new { k, c };

            var knowledgeBasedViewModels = await knowledgeBases.Take(take)
                .Select(u => new KnowledgeBaseQuickViewModel()
                {
                    Id = u.k.Id,
                    CategoryId = u.k.CategoryId,
                    Description = u.k.Description,
                    SeoAlias = u.k.SeoAlias,
                    Title = u.k.Title,
                    CategoryAlias = u.c.SeoAlias,
                    CategoryName = u.c.Name,
                    NumberOfVotes = u.k.NumberOfVotes,
                    CreateDate = u.k.CreateDate,
                    IsProcessed = u.k.IsProcessed,
                    Labels = TextHelper.Split(u.k.Labels, ",")
                }).ToListAsync();
            await _cacheService.SetAsync(CacheConstants.PopularKnowledgeBases, knowledgeBasedViewModels, 24);
            cachedData = knowledgeBasedViewModels;
        }

        return Ok(cachedData);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var knowledgeBase = await _context.KnowledgeBases.FindAsync(id);
        if (knowledgeBase == null)
            return NotFound(new ApiNotFoundResponse($"Cannot found knowledge base with id: {id}"));
        var category = await _context.Categories.FindAsync(knowledgeBase.CategoryId);
        if (knowledgeBase == null)
            return NotFound(new ApiNotFoundResponse($"Cannot found category with id: {knowledgeBase.CategoryId}"));

        var user = await _context.Users.FindAsync(knowledgeBase.OwnerUserId);
        if (user == null)
            return NotFound(new ApiNotFoundResponse($"Cannot found user with id: {knowledgeBase.OwnerUserId}"));
        var attachments = await _context.Attachments
            .Where(x => x.KnowledgeBaseId == id)
            .Select(x => new AttachmentViewModel()
            {
                FileName = x.FileName,
                FilePath = x.FilePath,
                FileSize = x.FileSize,
                Id = x.Id,
                FileType = x.FileType
            }).ToListAsync();
        var knowledgeBaseViewModel = CreateKnowledgeBaseViewModel(knowledgeBase);
        knowledgeBaseViewModel.CategoryName = category.Name;
        knowledgeBaseViewModel.Attachments = attachments;
        knowledgeBaseViewModel.Avatar = user.Avatar;
        knowledgeBaseViewModel.FullName = user.FirstName + " " + user.LastName;
        knowledgeBaseViewModel.IsProcessed = knowledgeBase.IsProcessed;

        return Ok(knowledgeBaseViewModel);
    }

    [HttpPut("{id}")]
    [ClaimRequirement(FunctionCode.CONTENT_KNOWLEDGE_BASE, CommandCode.UPDATE)]
    [ApiValidationFilter]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> PutKnowledgeBase(int id, [FromForm] KnowledgeBaseCreateRequest request)
    {
        var knowledgeBase = await _context.KnowledgeBases.FindAsync(id);
        if (knowledgeBase == null)
            return NotFound(new ApiNotFoundResponse($"Cannot found knowledge base with id {id}"));
        UpdateKnowledgeBase(request, knowledgeBase);

        //Process attachment
        if (request.Attachments != null && request.Attachments.Count > 0)
        {
            foreach (var attachment in request.Attachments)
            {
                var attachmentEntity = await SaveFile(knowledgeBase.Id, attachment);
                _context.Attachments.Add(attachmentEntity);
            }
        }

        _context.KnowledgeBases.Update(knowledgeBase);

        if (request.Labels?.Length > 0)
        {
            await ProcessLabel(request, knowledgeBase);
        }
        var result = await _context.SaveChangesAsync();

        if (result > 0)
        {
            await _cacheService.RemoveAsync("LatestKnowledgeBases");
            await _cacheService.RemoveAsync("PopularKnowledgeBases");
            return NoContent();
        }
        return BadRequest(new ApiBadRequestResponse($"Update knowledge base failed"));
    }

    [HttpGet("tags/{labelId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetKnowledgeBasesByTagId(string labelId, int pageIndex, int pageSize)
    {
        var query = from k in _context.KnowledgeBases
                    join lik in _context.LabelInKnowledgeBases on k.Id equals lik.KnowledgeBaseId
                    join l in _context.Labels on lik.LabelId equals l.Id
                    join c in _context.Categories on k.CategoryId equals c.Id
                    where lik.LabelId == labelId
                    select new { k, l, c };

        var totalRecords = await query.CountAsync();
        var items = await query.Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new KnowledgeBaseQuickViewModel()
            {
                Id = u.k.Id,
                CategoryId = u.k.CategoryId,
                Description = u.k.Description,
                SeoAlias = u.k.SeoAlias,
                Title = u.k.Title,
                CategoryAlias = u.c.SeoAlias,
                CategoryName = u.c.Name,
                NumberOfVotes = u.k.NumberOfVotes,
                CreateDate = u.k.CreateDate,
                NumberOfComments = u.k.NumberOfComments,
                IsProcessed = u.k.IsProcessed
            })
            .ToListAsync();

        var pagination = new Pagination<KnowledgeBaseQuickViewModel>
        {
            PageSize = pageSize,
            PageIndex = pageIndex,
            Items = items,
            TotalRecords = totalRecords,
        };
        return Ok(pagination);
    }

    [HttpDelete("{id}")]
    [ClaimRequirement(FunctionCode.CONTENT_KNOWLEDGE_BASE, CommandCode.DELETE)]
    public async Task<IActionResult> DeleteKnowledgeBase(int id)
    {
        var knowledgeBase = await _context.KnowledgeBases.FindAsync(id);
        if (knowledgeBase == null)
            return NotFound();

        _context.KnowledgeBases.Remove(knowledgeBase);
        var result = await _context.SaveChangesAsync();
        if (result > 0)
        {
            await _cacheService.RemoveAsync(CacheConstants.LatestKnowledgeBases);
            await _cacheService.RemoveAsync(CacheConstants.PopularKnowledgeBases);
            KnowledgeBaseViewModel knowledgeBaseViewModel = CreateKnowledgeBaseViewModel(knowledgeBase);
            return Ok(knowledgeBaseViewModel);
        }
        return BadRequest(new ApiBadRequestResponse($"Delete knowledge base failed"));
    }

    [HttpGet("{knowledgeBaseId}/labels")]
    [AllowAnonymous]
    public async Task<IActionResult> GetLabelsByKnowledgeBaseId(int knowledgeBaseId)
    {
        var query = from lik in _context.LabelInKnowledgeBases
                    join l in _context.Labels on lik.LabelId equals l.Id
                    orderby l.Name ascending
                    where lik.KnowledgeBaseId == knowledgeBaseId
                    select new { l.Id, l.Name };

        var labels = await query.Select(u => new LabelViewModel()
        {
            Id = u.Id,
            Name = u.Name
        }).ToListAsync();

        return Ok(labels);
    }

    [HttpPut("{id}/view-count")]
    [AllowAnonymous]
    public async Task<IActionResult> UpdateViewCount(int id)
    {
        var knowledgeBase = await _context.KnowledgeBases.FindAsync(id);
        if (knowledgeBase == null)
            return NotFound();
        if (knowledgeBase.ViewCount == null)
            knowledgeBase.ViewCount = 0;

        knowledgeBase.ViewCount += 1;
        _context.KnowledgeBases.Update(knowledgeBase);
        var result = await _context.SaveChangesAsync();
        if (result > 0)
        {
            return Ok();
        }
        return BadRequest();
    }

    #region Private methods

    private static KnowledgeBaseViewModel CreateKnowledgeBaseViewModel(KnowledgeBase knowledgeBase)
    {
        return new KnowledgeBaseViewModel()
        {
            Id = knowledgeBase.Id,

            CategoryId = knowledgeBase.CategoryId,

            Title = knowledgeBase.Title,

            SeoAlias = knowledgeBase.SeoAlias,

            Description = knowledgeBase.Description,

            Environment = knowledgeBase.Environment,

            Problem = knowledgeBase.Problem,

            StepToReproduce = knowledgeBase.StepToReproduce,

            ErrorMessage = knowledgeBase.ErrorMessage,

            Workaround = knowledgeBase.Workaround,

            Note = knowledgeBase.Note,

            OwnerUserId = knowledgeBase.OwnerUserId,

            Labels = !string.IsNullOrEmpty(knowledgeBase.Labels) ? knowledgeBase.Labels.Split(',') : null,

            CreateDate = knowledgeBase.CreateDate,

            LastModifiedDate = knowledgeBase.LastModifiedDate,

            NumberOfComments = knowledgeBase.NumberOfComments,

            NumberOfVotes = knowledgeBase.NumberOfVotes,

            NumberOfReports = knowledgeBase.NumberOfReports,

            IsProcessed = knowledgeBase.IsProcessed
        };
    }

    private static KnowledgeBase CreateKnowledgeBaseEntity(KnowledgeBaseCreateRequest request)
    {
        var entity = new KnowledgeBase()
        {
            CategoryId = request.CategoryId,

            Title = request.Title,

            SeoAlias = request.SeoAlias,

            Description = request.Description,

            Environment = request.Environment,

            Problem = request.Problem,

            StepToReproduce = request.StepToReproduce,

            ErrorMessage = request.ErrorMessage,

            Workaround = request.Workaround,

            Note = request.Note,

            IsProcessed = false
        };
        if (request.Labels?.Length > 0)
        {
            entity.Labels = string.Join(',', request.Labels);
        }
        return entity;
    }

    private async Task<Attachment> SaveFile(int knowledgeBaseId, IFormFile file)
    {
        if (ContentDispositionHeaderValue.TryParse(file.ContentDisposition, out var contentDisposition))
        {
            var originalFileName = contentDisposition.FileName != null ? contentDisposition.FileName.Trim('"') : string.Empty;
            var fileName = $"{originalFileName.Substring(0, originalFileName.LastIndexOf('.'))}{Path.GetExtension(originalFileName)}";
            await _storageService.SaveFileAsync(file.OpenReadStream(), fileName);
            var attachmentEntity = new Attachment()
            {
                FileName = fileName,
                FilePath = _storageService.GetFileUrl(fileName),
                FileSize = file.Length,
                FileType = Path.GetExtension(fileName),
                KnowledgeBaseId = knowledgeBaseId,
            };
            return attachmentEntity;
        }
        return new Attachment();
    }

    private async Task ProcessLabel(KnowledgeBaseCreateRequest request, KnowledgeBase knowledgeBase)
    {
        if (request.Labels != null)
        {
            foreach (var labelText in request.Labels)
            {
                if (labelText == null) continue;
                var labelId = TextHelper.ToUnsignedString(labelText.ToString()); var existingLabel = await _context.Labels.FindAsync(labelId);
                if (existingLabel == null)
                {
                    var labelEntity = new Label()
                    {
                        Id = labelId,
                        Name = labelText.ToString()
                    };
                    _context.Labels.Add(labelEntity);
                }
                if (await _context.LabelInKnowledgeBases.FindAsync(labelId, knowledgeBase.Id) == null)
                {
                    _context.LabelInKnowledgeBases.Add(new LabelInKnowledgeBase()
                    {
                        KnowledgeBaseId = knowledgeBase.Id,
                        LabelId = labelId
                    });
                }
            }
        }
    }

    private static void UpdateKnowledgeBase(KnowledgeBaseCreateRequest request, KnowledgeBase knowledgeBase)
    {
        if (request.CategoryId != 0)
        {
            knowledgeBase.CategoryId = request.CategoryId;
        }

        knowledgeBase.Title = request.Title;

        if (string.IsNullOrEmpty(request.SeoAlias) && request.Title != null)
            knowledgeBase.SeoAlias = TextHelper.ToUnsignedString(request.Title);
        else
            knowledgeBase.SeoAlias = request.SeoAlias;

        knowledgeBase.Description = request.Description;

        knowledgeBase.Environment = request.Environment;

        knowledgeBase.Problem = request.Problem;

        knowledgeBase.StepToReproduce = request.StepToReproduce;

        knowledgeBase.ErrorMessage = request.ErrorMessage;

        knowledgeBase.Workaround = request.Workaround;

        knowledgeBase.Note = request.Note;

        if (request.IsProcessed != null)
        {
            knowledgeBase.IsProcessed = request.IsProcessed;
        }

        if (request.Labels != null)
        {
            knowledgeBase.Labels = string.Join(',', request.Labels);
        }
    }

    #endregion Private methods
}