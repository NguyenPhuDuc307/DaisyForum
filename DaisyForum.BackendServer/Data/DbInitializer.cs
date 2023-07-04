using System.Collections.Generic;
using AutoMapper;
using DaisyForum.BackendServer.Data.Entities;
using DaisyForum.BackendServer.Services;
using DaisyForum.ViewModels.Contents;
using Microsoft.AspNetCore.Identity;

namespace DaisyForum.BackendServer.Data;

public class DbInitializer
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IContentBasedService _contentBasedService;
    private readonly IMapper _mapper;
    private readonly string AdminRoleName = "Admin";
    private readonly string UserRoleName = "Member";

    public DbInitializer(ApplicationDbContext context,
      UserManager<User> userManager,
      RoleManager<IdentityRole> roleManager,
      IContentBasedService contentBasedService,
      IMapper mapper)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _contentBasedService = contentBasedService;
        _mapper = mapper;
    }

    public async Task Seed()
    {
        #region Quyền

        if (!_roleManager.Roles.Any())
        {
            await _roleManager.CreateAsync(new IdentityRole
            {
                Id = AdminRoleName,
                Name = AdminRoleName,
                NormalizedName = AdminRoleName.ToUpper(),
            });
            await _roleManager.CreateAsync(new IdentityRole
            {
                Id = UserRoleName,
                Name = UserRoleName,
                NormalizedName = UserRoleName.ToUpper(),
            });
        }

        #endregion Quyền

        #region Người dùng

        if (!_userManager.Users.Any())
        {
            var result = await _userManager.CreateAsync(new User
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "admin",
                FirstName = "Quản trị",
                LastName = "1",
                Email = "admin.ssdaisy@gmail.com",
                LockoutEnabled = false,
                PhoneNumber = "0964732231"
            }, "Admin@123");
            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync("admin");
                if (user != null)
                    await _userManager.AddToRoleAsync(user, AdminRoleName);
            }
        }

        #endregion Người dùng

        #region Chức năng

        if (!_context.Functions.Any())
        {
            _context.Functions.AddRange(new List<Function>
            {
                new Function {Id = "DASHBOARD", Name = "Thống kê", ParentId = null, SortOrder = 1,Url = "/dashboard",Icon="fa-dashboard" },

                new Function {Id = "CONTENT",Name = "Nội dung",ParentId = null,Url = "/contents",Icon="fa-table" },

                new Function {Id = "CONTENT_CATEGORY",Name = "Danh mục",ParentId ="CONTENT",Url = "/contents/categories", Icon="fa-edit"},
                new Function {Id = "CONTENT_KNOWLEDGE_BASE",Name = "Bài viết",ParentId = "CONTENT",SortOrder = 2,Url = "/contents/knowledge-bases",Icon="fa-edit" },
                new Function {Id = "CONTENT_COMMENT",Name = "Trang",ParentId = "CONTENT",SortOrder = 3,Url = "/contents/knowledge-bases/comments",Icon="fa-edit" },
                new Function {Id = "CONTENT_REPORT",Name = "Báo xấu",ParentId = "CONTENT",SortOrder = 3,Url = "/contents/knowledge-bases/reports",Icon="fa-edit" },

                new Function {Id = "STATISTIC",Name = "Thống kê", ParentId = null, Url = "/statistics",Icon="fa-bar-chart-o" },

                new Function {Id = "STATISTIC_MONTHLY_NEW_MEMBER",Name = "Đăng ký từng tháng",ParentId = "STATISTIC",SortOrder = 1,Url = "/statistics/monthly-registers",Icon = "fa-wrench"},
                new Function {Id = "STATISTIC_MONTHLY_NEW_KNOWLEDGE_BASE",Name = "Bài đăng hàng tháng",ParentId = "STATISTIC",SortOrder = 2,Url = "/statistics/monthly-new-knowledge-bases",Icon = "fa-wrench"},
                new Function {Id = "STATISTIC_MONTHLY_COMMENT",Name = "Comment theo tháng",ParentId = "STATISTIC",SortOrder = 3,Url = "/statistics/monthly-comments",Icon = "fa-wrench" },

                new Function {Id = "SYSTEM", Name = "Hệ thống", ParentId = null, Url = "/systems",Icon="fa-th-list" },

                new Function {Id = "SYSTEM_USER", Name = "Người dùng",ParentId = "SYSTEM",Url = "/systems/users",Icon="fa-desktop"},
                new Function {Id = "SYSTEM_ROLE", Name = "Nhóm quyền",ParentId = "SYSTEM",Url = "/systems/roles",Icon="fa-desktop"},
                new Function {Id = "SYSTEM_FUNCTION", Name = "Chức năng",ParentId = "SYSTEM",Url = "/systems/functions",Icon="fa-desktop"},
                new Function {Id = "SYSTEM_PERMISSION", Name = "Quyền hạn",ParentId = "SYSTEM",Url = "/systems/permissions",Icon="fa-desktop"},
            });
            await _context.SaveChangesAsync();
        }

        //KnowledgeBase
        if (!_context.KnowledgeBases.Any())
        {
            List<KnowledgeBasesFromCSV> listPost = _contentBasedService.GetData("/Users/daisy/Desktop/ĐỒ ÁN TỐT NGHIỆP 2023/DaisyForum/train copy.csv");
            List<KnowledgeBaseCreateRequest> listKnowledgeBase = _mapper.Map<List<KnowledgeBasesFromCSV>, List<KnowledgeBaseCreateRequest>>(listPost);
            await _contentBasedService.SeedData(listKnowledgeBase, "45c2a768-549e-4d57-95a7-ad19939403e0");
        }

        if (!_context.Commands.Any())
        {
            _context.Commands.AddRange(new List<Command>()
            {
                new Command(){Id = "VIEW", Name = "Xem"},
                new Command(){Id = "CREATE", Name = "Thêm"},
                new Command(){Id = "UPDATE", Name = "Sửa"},
                new Command(){Id = "DELETE", Name = "Xoá"},
                new Command(){Id = "APPROVE", Name = "Duyệt"},
            });
        }

        #endregion Chức năng

        var functions = _context.Functions;

        if (!_context.CommandInFunctions.Any())
        {
            foreach (var function in functions)
            {
                var createAction = new CommandInFunction()
                {
                    CommandId = "CREATE",
                    FunctionId = function.Id
                };
                _context.CommandInFunctions.Add(createAction);

                var updateAction = new CommandInFunction()
                {
                    CommandId = "UPDATE",
                    FunctionId = function.Id
                };
                _context.CommandInFunctions.Add(updateAction);
                var deleteAction = new CommandInFunction()
                {
                    CommandId = "DELETE",
                    FunctionId = function.Id
                };
                _context.CommandInFunctions.Add(deleteAction);

                var viewAction = new CommandInFunction()
                {
                    CommandId = "VIEW",
                    FunctionId = function.Id
                };
                _context.CommandInFunctions.Add(viewAction);
            }
        }

        if (!_context.Permissions.Any())
        {
            var adminRole = await _roleManager.FindByNameAsync(AdminRoleName);
            if (adminRole != null)
                foreach (var function in functions)
                {
                    if (function.Id != null && adminRole.Id != null)
                    {
                        _context.Permissions.Add(new Permission(function.Id, adminRole.Id, "CREATE"));
                        _context.Permissions.Add(new Permission(function.Id, adminRole.Id, "UPDATE"));
                        _context.Permissions.Add(new Permission(function.Id, adminRole.Id, "DELETE"));
                        _context.Permissions.Add(new Permission(function.Id, adminRole.Id, "VIEW"));
                    }

                }
        }

        await _context.SaveChangesAsync();
    }
}