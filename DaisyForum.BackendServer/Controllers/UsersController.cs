using System.Collections;
using AutoMapper;
using DaisyForum.BackendServer.Authorization;
using DaisyForum.BackendServer.Constants;
using DaisyForum.BackendServer.Extensions;
using DaisyForum.BackendServer.Data;
using DaisyForum.BackendServer.Data.Entities;
using DaisyForum.BackendServer.Helpers;
using DaisyForum.ViewModels;
using DaisyForum.ViewModels.Contents;
using DaisyForum.ViewModels.Systems;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DaisyForum.BackendServer.Controllers;

public class UsersController : BaseController
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UsersController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context, IMapper mapper)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
        _mapper = mapper;
    }

    [HttpGet("suggested")]
    [AllowAnonymous]
    public async Task<IActionResult> GetKnowledgeBasesSuggested(string userId, int size = 5)
    {
        var listKnowledgeBases = await _context.KnowledgeBases.ToListAsync();
        List<KnowledgeBasesFromCSV> listPost = _mapper.Map<List<KnowledgeBase>, List<KnowledgeBasesFromCSV>>(listKnowledgeBases);

        HashSet<string> setTag = new HashSet<string>();
        foreach (var post in listPost)
        {
            if (post.Tags != null && post.Tags.Any())
            {
                foreach (var tag in post.Tags)
                {
                    setTag.Add(tag);
                }
            }
        }
        List<string> listTag = setTag.ToList();

        var userBase = await _userManager.FindByIdAsync(userId);
        if (userBase == null)
        {
            return NotFound(new ApiNotFoundResponse($"Cannot found user with id: {userId}"));
        }

        UserBases user = new UserBases()
        {
            UserId = userBase.Id,
            Tags = TextHelper.Split(userBase.Labels, ",")
        };

        BitArray vector_user = new BitArray(listTag.Count);
        for (int i = 0; i < listTag.Count; i++)
        {
            if (user.Tags != null && user.Tags.Contains(listTag[i]))
            {
                vector_user[i] = true;
            }
        }

        List<KnowledgeBasesVector> listVectors = new List<KnowledgeBasesVector>();
        foreach (var post in listPost)
        {
            if (post.Tags != null && post.Tags.Any())
            {
                BitArray vector_post = new BitArray(listTag.Count);
                for (int i = 0; i < listTag.Count; i++)
                {
                    if (post.Tags.Contains(listTag[i]))
                    {
                        vector_post[i] = true;
                    }
                }
                listVectors.Add(new KnowledgeBasesVector()
                {
                    Id = post.Id,
                    Vector = vector_post
                });
            }
        }

        List<UserBases> userBases = new List<UserBases>();
        for (int i = 0; i < listVectors.Count; i++)
        {
            double cosine = Content_Based.CosineSimilarity(listVectors[i].Vector, vector_user);
            if (cosine > 0)
            {
                userBases.Add(new UserBases()
                {
                    UserId = user.UserId,
                    PostId = listVectors[i].Id,
                    Cosine = cosine,
                });
            }
        }
        userBases = userBases.OrderByDescending(x => x.Cosine).Take(size).ToList();
        var listKnowledgeBaseViewModel = new List<KnowledgeBaseQuickViewModel>();
        foreach (var kb in userBases)
        {
            var knowledgeBase = await _context.KnowledgeBases.FindAsync(kb.PostId);
            if (knowledgeBase == null)
                return NotFound(new ApiNotFoundResponse($"Cannot found knowledge base with id: {kb.PostId}"));
            var category = await _context.Categories.FindAsync(knowledgeBase.CategoryId);
            if (category == null)
                return NotFound(new ApiNotFoundResponse($"Cannot found category with id: {knowledgeBase.CategoryId}"));
            var attachments = await _context.Attachments
                .Where(x => x.KnowledgeBaseId == kb.PostId)
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
            knowledgeBaseViewModel.CategoryAlias = category.SeoAlias;
            listKnowledgeBaseViewModel.Add(knowledgeBaseViewModel);
        }
        return Ok(listKnowledgeBaseViewModel);
    }

    private static KnowledgeBaseQuickViewModel CreateKnowledgeBaseViewModel(KnowledgeBase knowledgeBase)
    {
        return new KnowledgeBaseQuickViewModel()
        {
            Id = knowledgeBase.Id,

            CategoryId = knowledgeBase.CategoryId,

            Title = knowledgeBase.Title,

            SeoAlias = knowledgeBase.SeoAlias,

            Description = knowledgeBase.Description,

            Labels = !string.IsNullOrEmpty(knowledgeBase.Labels) ? knowledgeBase.Labels.Split(',') : null,

            CreateDate = knowledgeBase.CreateDate,

            NumberOfComments = knowledgeBase.NumberOfComments,

            NumberOfVotes = knowledgeBase.NumberOfVotes
        };
    }

    [HttpGet("analysisKnowledgeBases")]
    [AllowAnonymous]
    public async Task<IActionResult> AnalysisKnowledgeBases()
    {

        var listKnowledgeBases = await _context.KnowledgeBases.ToListAsync();
        List<KnowledgeBasesFromCSV> listPost = _mapper.Map<List<KnowledgeBase>, List<KnowledgeBasesFromCSV>>(listKnowledgeBases);

        HashSet<string> setTag = new HashSet<string>();
        foreach (var post in listPost)
        {
            if (post.Tags != null && post.Tags.Any())
            {
                foreach (var tag in post.Tags)
                {
                    setTag.Add(tag);
                }
            }
        }
        List<string> listTag = setTag.ToList();

        UserBases user1 = new UserBases()
        {
            UserId = "1",
            Tags = new string[]{
            "java",
            ".Net",
            "C#",
            "optional",
            "haskell"
            }
        };

        BitArray vector_user = new BitArray(listTag.Count);
        for (int i = 0; i < listTag.Count; i++)
        {
            if (user1.Tags != null && user1.Tags.Contains(listTag[i]))
            {
                vector_user[i] = true;
            }
        }

        List<KnowledgeBasesVector> listVectors = new List<KnowledgeBasesVector>();
        foreach (var post in listPost)
        {
            if (post.Tags != null && post.Tags.Any())
            {
                BitArray vector_post = new BitArray(listTag.Count);
                for (int i = 0; i < listTag.Count; i++)
                {
                    if (post.Tags.Contains(listTag[i]))
                    {
                        vector_post[i] = true;
                    }
                }
                listVectors.Add(new KnowledgeBasesVector()
                {
                    Id = post.Id,
                    Vector = vector_post
                });
            }
        }
        List<UserBases> userBases = new List<UserBases>();
        for (int i = 0; i < listVectors.Count; i++)
        {
            double cosine = Content_Based.CosineSimilarity(listVectors[i].Vector, vector_user);
            if (cosine > 0)
            {
                userBases.Add(new UserBases()
                {
                    UserId = user1.UserId,
                    PostId = listVectors[i].Id,
                    Cosine = cosine
                });
            }
        }
        userBases = userBases.OrderByDescending(x => x.Cosine).ToList();

        return Ok(userBases);
    }

    [HttpPost]
    [ClaimRequirement(FunctionCode.SYSTEM_USER, CommandCode.CREATE)]
    [ApiValidationFilter]
    public async Task<IActionResult> PostUser(UserCreateRequest request)
    {
        var user = new User()
        {
            Id = Guid.NewGuid().ToString(),
            Email = request.Email,
            Dob = request.Dob != null ? DateTime.Parse(request.Dob) : DateTime.Now,
            UserName = request.UserName,
            LastName = request.LastName,
            FirstName = request.FirstName,
            PhoneNumber = request.PhoneNumber,
            CreateDate = DateTime.Now
        };

        if (request.Password == null)
            return BadRequest(new ApiBadRequestResponse("Password is required"));

        var result = await _userManager.CreateAsync(user, request.Password);
        if (result.Succeeded)
        {
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, request);
        }
        else
        {
            return BadRequest(new ApiBadRequestResponse(result));
        }
    }

    private static UserViewModel CreateUserViewModel(User user)
    {
        return new UserViewModel()
        {
            Id = user.Id,
            UserName = user.UserName,
            Dob = user.Dob,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            FirstName = user.FirstName,
            LastName = user.LastName,
            CreateDate = user.CreateDate,
            Labels = !string.IsNullOrEmpty(user.Labels) ? user.Labels.Split(',') : null,
            Avatar = user.Avatar
        };
    }

    [HttpGet]
    [ClaimRequirement(FunctionCode.SYSTEM_USER, CommandCode.VIEW)]
    public async Task<IActionResult> GetUsers()
    {
        var users = _userManager.Users;

        var userViewModels = await users.Select(u => new UserViewModel()
        {
            Id = u.Id,
            UserName = u.UserName,
            Dob = u.Dob,
            Email = u.Email,
            PhoneNumber = u.PhoneNumber,
            FirstName = u.FirstName,
            LastName = u.LastName,
            CreateDate = u.CreateDate,
            Labels = TextHelper.Split(u.Labels, ","),
            Avatar = u.Avatar,
            Description = u.Description
        }).ToListAsync();

        return Ok(userViewModels);
    }

    [HttpGet("filter")]
    [ClaimRequirement(FunctionCode.SYSTEM_USER, CommandCode.VIEW)]
    public async Task<IActionResult> GetUsersPaging(string? keyword, int page = 1, int pageSize = 10)
    {
        var query = _userManager.Users;

        if (!String.IsNullOrEmpty(keyword))
        {
            query = query.Where(x =>
                (x.UserName != null && x.UserName.Contains(keyword))
                || (x.Email != null && x.Email.Contains(keyword))
                || (x.PhoneNumber != null && x.PhoneNumber.Contains(keyword)));
        }

        var item = await query.Skip((page - 1) * pageSize).Take(pageSize).Select(x => new UserViewModel()
        {
            Id = x.Id,
            UserName = x.UserName,
            Dob = x.Dob,
            Email = x.Email,
            PhoneNumber = x.PhoneNumber,
            FirstName = x.FirstName,
            LastName = x.LastName,
            CreateDate = x.CreateDate,
            Avatar = x.Avatar,
            Description = x.Description
        }).ToListAsync();

        var totalRecords = await query.CountAsync();

        var pagination = new Pagination<UserViewModel>
        {
            Items = item,
            TotalRecords = totalRecords
        };

        return Ok(pagination);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound(new ApiNotFoundResponse($"Cannot found user with id: {id}"));

        var userViewModel = new UserViewModel()
        {
            Id = user.Id,
            UserName = user.UserName,
            Dob = user.Dob,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            FirstName = user.FirstName,
            LastName = user.LastName,
            CreateDate = user.CreateDate,
            Labels = TextHelper.Split(user.Labels, ","),
            Avatar = user.Avatar,
            Description = user.Description,
            NumberOfFollowers = user.NumberOfFollowers
        };
        return Ok(userViewModel);
    }

    [HttpPut("{id}")]
    [ClaimRequirement(FunctionCode.SYSTEM_USER, CommandCode.UPDATE)]
    public async Task<IActionResult> PutUser(string id, [FromBody] UserCreateRequest request)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound(new ApiNotFoundResponse($"Cannot found user with id: {id}"));

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Dob = request.Dob != null ? DateTime.Parse(request.Dob) : DateTime.Now;
        user.LastModifiedDate = DateTime.Now;

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            return NoContent();
        }
        return BadRequest(new ApiBadRequestResponse(result));
    }

    [HttpPut("{id}/change-password")]
    [ClaimRequirement(FunctionCode.SYSTEM_USER, CommandCode.UPDATE)]
    public async Task<IActionResult> PutUserPassword(string id, [FromBody] UserPasswordChangeRequest request)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound(new ApiNotFoundResponse($"Cannot found user with id: {id}"));

        if (request.CurrentPassword == null || request.NewPassword == null)
            return BadRequest(new ApiBadRequestResponse("The current password and new password cannot be null."));
        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (result.Succeeded)
        {
            return NoContent();
        }
        return BadRequest(new ApiBadRequestResponse(result));
    }

    [HttpDelete("{id}")]
    [ClaimRequirement(FunctionCode.SYSTEM_USER, CommandCode.DELETE)]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound();

        var adminUsers = await _userManager.GetUsersInRoleAsync(Constants.SystemConstants.Roles.Admin);
        var otherUsers = adminUsers.Where(x => x.Id != id).ToList();
        if (otherUsers.Count == 0)
        {
            return BadRequest(new ApiBadRequestResponse("You cannot remove the only admin user remaining."));
        }
        var result = await _userManager.DeleteAsync(user);

        if (result.Succeeded)
        {
            var userViewModel = new UserViewModel()
            {
                Id = user.Id,
                UserName = user.UserName,
                Dob = user.Dob,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
                CreateDate = user.CreateDate,
                Avatar = user.Avatar,
                Description = user.Description
            };
            return Ok(userViewModel);
        }
        return BadRequest(new ApiBadRequestResponse(result));
    }

    [HttpGet("{userId}/menu")]
    public async Task<IActionResult> GetMenuByUserPermission(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound(new ApiNotFoundResponse($"Cannot found user with id: {userId}"));
        }
        var roles = await _userManager.GetRolesAsync(user);
        var query = from f in _context.Functions
                    join p in _context.Permissions
                        on f.Id equals p.FunctionId
                    join r in _roleManager.Roles on p.RoleId equals r.Id
                    join a in _context.Commands
                        on p.CommandId equals a.Id
                    where !string.IsNullOrEmpty(r.Name) && roles.Contains(r.Name) && a.Id == "VIEW"
                    select new FunctionViewModel
                    {
                        Id = f.Id,
                        Name = f.Name,
                        Url = f.Url,
                        ParentId = f.ParentId,
                        SortOrder = f.SortOrder,
                        Icon = f.Icon
                    };
        var data = await query.Distinct()
            .OrderBy(x => x.ParentId)
            .ThenBy(x => x.SortOrder)
            .ToListAsync();
        return Ok(data);
    }

    [HttpGet("{userId}/roles")]
    [ClaimRequirement(FunctionCode.SYSTEM_USER, CommandCode.VIEW)]
    public async Task<IActionResult> GetUserRoles(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound(new ApiNotFoundResponse($"Cannot found user with id: {userId}"));
        var roles = await _userManager.GetRolesAsync(user);
        return Ok(roles);
    }

    [HttpPost("{userId}/roles")]
    [ClaimRequirement(FunctionCode.SYSTEM_USER, CommandCode.UPDATE)]
    public async Task<IActionResult> PostRolesToUserUser(string userId, [FromBody] RoleAssignRequest request)
    {
        if (request.RoleNames == null)
        {
            return BadRequest(new ApiBadRequestResponse("Role names cannot empty"));
        }
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound(new ApiNotFoundResponse($"Cannot found user with id: {userId}"));
        var result = await _userManager.AddToRolesAsync(user, request.RoleNames);
        if (result.Succeeded)
            return Ok();

        return BadRequest(new ApiBadRequestResponse(result));
    }

    [HttpDelete("{userId}/roles")]
    [ClaimRequirement(FunctionCode.SYSTEM_USER, CommandCode.VIEW)]
    public async Task<IActionResult> RemoveRolesFromUser(string userId, [FromQuery] RoleAssignRequest request)
    {
        if (request.RoleNames?.Length == 0)
        {
            return BadRequest(new ApiBadRequestResponse("Role names cannot empty"));
        }
        if (request.RoleNames.Length == 1 && request.RoleNames[0] == Constants.SystemConstants.Roles.Admin)
        {
            return base.BadRequest(new ApiBadRequestResponse($"Cannot remove {Constants.SystemConstants.Roles.Admin} role"));
        }
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound(new ApiNotFoundResponse($"Cannot found user with id: {userId}"));
        var result = await _userManager.RemoveFromRolesAsync(user, request.RoleNames);
        if (result.Succeeded)
            return Ok();

        return BadRequest(new ApiBadRequestResponse(result));
    }

    [HttpGet("{userId}/knowledgeBases")]
    public async Task<IActionResult> GetKnowledgeBasesByUserId(string userId, int pageIndex, int pageSize)
    {
        var query = from k in _context.KnowledgeBases
                    join c in _context.Categories on k.CategoryId equals c.Id
                    where k.OwnerUserId == userId
                    orderby k.CreateDate descending
                    select new { k, c };

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
               IsProcessed = u.k.IsProcessed,
           }).ToListAsync();

        var pagination = new Pagination<KnowledgeBaseQuickViewModel>
        {
            Items = items,
            TotalRecords = totalRecords,
            PageIndex = pageIndex,
            PageSize = pageSize
        };
        return Ok(pagination);
    }

    [HttpPut("{userId}/putLabels")]
    [ClaimRequirement(FunctionCode.SYSTEM_USER, CommandCode.UPDATE)]
    public async Task<IActionResult> PutLabels(string userId, [FromForm] string[] labels)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound(new ApiNotFoundResponse($"Cannot found user with userId: {userId}"));

        user.Labels = string.Join(',', labels);
        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {

            return NoContent();
        }
        return BadRequest(new ApiBadRequestResponse(result));
    }

    public async Task<UserViewModel?> GetUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return null;

        var userViewModel = new UserViewModel()
        {
            Id = user.Id,
            UserName = user.UserName,
            Dob = user.Dob,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            FirstName = user.FirstName,
            LastName = user.LastName,
            CreateDate = user.CreateDate,
            Labels = TextHelper.Split(user.Labels, ","),
            Avatar = user.Avatar,
            Description = user.Description,
            NumberOfFollowers = user.NumberOfFollowers
        };
        return userViewModel;
    }

    [HttpGet("{userId}/followers")]
    public async Task<IActionResult> GetFollowers(string userId, int pageIndex, int pageSize)
    {
        var query = from f in _context.Followers
                    where f.FollowerId == userId
                    select new { f };

        int totalRecords = await query.CountAsync();

        var followers = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new FollowerViewModel()
            {
                OwnerUserId = u.f.OwnerUserId,
                FollowerId = u.f.FollowerId,
                CreateDate = u.f.CreateDate,
                Notification = u.f.Notification
            }).ToListAsync();
        var items = new List<FollowerViewModel>();
        foreach (var follower in followers)
        {
            var followerViewModel = new FollowerViewModel()
            {
                OwnerUserId = follower.OwnerUserId,
                FollowerId = follower.FollowerId,
                CreateDate = follower.CreateDate,
                Notification = follower.Notification
            };

            followerViewModel.Owner = await GetUser(follower.OwnerUserId);
            followerViewModel.Follower = await GetUser(follower.FollowerId);
            items.Add(followerViewModel);
        }
        var pagination = new Pagination<FollowerViewModel>
        {
            Items = items,
            TotalRecords = totalRecords,
            PageIndex = pageIndex,
            PageSize = pageSize
        };

        return Ok(pagination);
    }

    [HttpGet("{userId}/subscribers")]
    public async Task<IActionResult> GetSubscribers(string userId, int pageIndex, int pageSize)
    {
        var query = from f in _context.Followers
                    where f.OwnerUserId == userId
                    select new { f };

        int totalRecords = await query.CountAsync();

        var followers = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new FollowerViewModel()
            {
                OwnerUserId = u.f.OwnerUserId,
                FollowerId = u.f.FollowerId,
                CreateDate = u.f.CreateDate,
                Notification = u.f.Notification
            }).ToListAsync();
        var items = new List<FollowerViewModel>();
        foreach (var follower in followers)
        {
            var followerViewModel = new FollowerViewModel()
            {
                OwnerUserId = follower.OwnerUserId,
                FollowerId = follower.FollowerId,
                CreateDate = follower.CreateDate,
                Notification = follower.Notification
            };

            followerViewModel.Owner = await GetUser(follower.OwnerUserId);
            followerViewModel.Follower = await GetUser(follower.FollowerId);
            items.Add(followerViewModel);
        }
        var pagination = new Pagination<FollowerViewModel>
        {
            Items = items,
            TotalRecords = totalRecords,
            PageIndex = pageIndex,
            PageSize = pageSize
        };

        return Ok(pagination);
    }

    [HttpPost("follow/{ownerUserId}")]
    public async Task<IActionResult> Follow(string ownerUserId)
    {
        if (ownerUserId == User.GetUserId())
        {
            return BadRequest(new ApiBadRequestResponse($"Không thể theo dõi chính mình"));
        }
        var checkOwnerUser = await _userManager.FindByIdAsync(ownerUserId);
        if (checkOwnerUser == null)
            return NotFound(new ApiNotFoundResponse($"Cannot found ownerUser with id: {ownerUserId}"));

        var checkFollower = await _userManager.FindByIdAsync(User.GetUserId());
        if (checkFollower == null)
            return NotFound(new ApiNotFoundResponse($"Cannot found follower with id: {User.GetUserId()}"));

        var numberOfFollowers = await _context.Followers.CountAsync(x => x.OwnerUserId == ownerUserId);

        var follower = await _context.Followers.FindAsync(ownerUserId, User.GetUserId());
        if (follower != null)
        {
            _context.Followers.Remove(follower);
            numberOfFollowers -= 1;
        }
        else
        {
            follower = new Follower()
            {
                OwnerUserId = ownerUserId,
                FollowerId = User.GetUserId(),
                CreateDate = DateTime.Now
            };
            _context.Followers.Add(follower);
            numberOfFollowers += 1;
        }

        checkOwnerUser.NumberOfFollowers = numberOfFollowers;
        var result = await _userManager.UpdateAsync(checkOwnerUser);

        var result1 = await _context.SaveChangesAsync();
        if (result.Succeeded)
        {
            return Ok(numberOfFollowers);
        }
        else
        {
            return BadRequest(new ApiBadRequestResponse($"Follow failed"));
        }
    }

    [HttpPost("notification/{ownerUserId}")]
    public async Task<IActionResult> Notification(string ownerUserId)
    {
        if (ownerUserId == User.GetUserId())
        {
            return BadRequest(new ApiBadRequestResponse($"Không thể theo dõi chính mình"));
        }
        var checkOwnerUser = await _userManager.FindByIdAsync(ownerUserId);
        if (checkOwnerUser == null)
            return NotFound(new ApiNotFoundResponse($"Cannot found ownerUser with id: {ownerUserId}"));

        var checkFollower = await _userManager.FindByIdAsync(User.GetUserId());
        if (checkFollower == null)
            return NotFound(new ApiNotFoundResponse($"Cannot found follower with id: {User.GetUserId()}"));

        var follower = await _context.Followers.FindAsync(ownerUserId, User.GetUserId());
        if (follower != null)
        {
            if (follower.Notification == true)
            {
                follower.Notification = false;
            }
            else
            {
                follower.Notification = true;
            }
            var result = await _context.SaveChangesAsync();

            return Ok(follower.Notification);
        }
        else
        {
            return BadRequest(new ApiBadRequestResponse($"Update notification failed"));
        }
    }

    [HttpPost("un-follow/{ownerUserId}")]
    public async Task<IActionResult> UnFollow(string ownerUserId)
    {
        var checkOwnerUser = await _userManager.FindByIdAsync(ownerUserId);
        if (checkOwnerUser == null)
            return NotFound(new ApiNotFoundResponse($"Cannot found ownerUser with id: {ownerUserId}"));

        var checkFollower = await _userManager.FindByIdAsync(User.GetUserId());
        if (checkFollower == null)
            return NotFound(new ApiNotFoundResponse($"Cannot found follower with id: {User.GetUserId()}"));

        var numberOfFollowers = await _context.Followers.CountAsync(x => x.OwnerUserId == ownerUserId && x.FollowerId == User.GetUserId());

        var follower = await _context.Followers.FindAsync(ownerUserId, User.GetUserId());
        if (follower != null)
        {
            _context.Followers.Remove(follower);
            numberOfFollowers -= 1;
        }
        checkOwnerUser.NumberOfFollowers = numberOfFollowers;
        var result = await _userManager.UpdateAsync(checkOwnerUser);

        var result1 = await _context.SaveChangesAsync();
        if (result.Succeeded && result1 > 0)
        {
            return Ok(numberOfFollowers);
        }
        else
        {
            return BadRequest(new ApiBadRequestResponse($"Un follow failed"));
        }
    }
}