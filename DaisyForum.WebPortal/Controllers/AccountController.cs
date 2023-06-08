using DaisyForum.ViewModels.Contents;
using DaisyForum.WebPortal.Extensions;
using DaisyForum.WebPortal.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DaisyForum.WebPortal.Controllers;

public class AccountController : Controller
{
    private readonly IUserApiClient _userApiClient;
    private readonly IKnowledgeBaseApiClient _knowledgeBaseApiClient;
    private readonly ICategoryApiClient _categoryApiClient;

    public AccountController(IUserApiClient userApiClient,
        IKnowledgeBaseApiClient knowledgeBaseApiClient,
        ICategoryApiClient categoryApiClient)
    {
        _userApiClient = userApiClient;
        _categoryApiClient = categoryApiClient;
        _knowledgeBaseApiClient = knowledgeBaseApiClient;
    }

    public IActionResult SignIn()
    {
        return Challenge(new AuthenticationProperties { RedirectUri = "/" }, "oidc");
    }

    public new IActionResult SignOut()
    {
        return SignOut(new AuthenticationProperties { RedirectUri = "/" }, "Cookies", "oidc");
    }

    [Authorize]
    public async Task<ActionResult> MyProfile()
    {
        var user = await _userApiClient.GetById(User.GetUserId());
        return View(user);
    }

    [HttpGet]
    public async Task<ActionResult> Labels()
    {
        var user = await _userApiClient.GetById(User.GetUserId());
        return View(user);
    }

    [HttpPost]
    public async Task<ActionResult> Labels(string txt_label_submit)
    {
        if (txt_label_submit == null)
        {
            return BadRequest(ModelState);
        }

        string[] listLabel = txt_label_submit.Split(',');

        var user = await _userApiClient.PutLabels(User.GetUserId(), listLabel);
        return RedirectToAction("MyProfile");
    }

    public async Task<ActionResult> GetLabels(string? keyword, int page = 1, int pageSize = 100)
    {
        var labels = await _userApiClient.GetLabels(keyword, page, pageSize);
        return Ok(labels);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> MyKnowledgeBases(int page = 1, int pageSize = 10)
    {
        var kbs = await _userApiClient.GetKnowledgeBasesByUserId(User.GetUserId(), page, pageSize);
        return View(kbs);
    }



    [HttpGet]
    public async Task<IActionResult> CreateNewKnowledgeBase()
    {
        await SetCategoriesViewBag();
        var categories = await _categoryApiClient.GetCategories();
        var treeNodes = TreeNode.ConvertToTreeNodes(categories);
        var viewModel = new KnowledgeBaseCreateRequest()
        {
            TreeNodes = treeNodes,
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> CreateNewKnowledgeBase([FromForm] KnowledgeBaseCreateRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (request.Labels?.Count() > 0)
        {
            string[] listLabel = request.Labels[0].Split(',');
            request.Labels = listLabel;
        }
        var result = await _knowledgeBaseApiClient.PostKnowledgeBase(request);
        if (result)
        {
            return Ok();
        }
        return BadRequest();
    }

    [HttpGet]
    public async Task<IActionResult> EditKnowledgeBase(int id)
    {
        var knowledgeBase = await _knowledgeBaseApiClient.GetKnowledgeBaseDetail(id);
        await SetCategoriesViewBag();
        return View(new KnowledgeBaseCreateRequest()
        {
            CategoryId = knowledgeBase.CategoryId,
            Description = knowledgeBase.Description,
            Environment = knowledgeBase.Environment,
            ErrorMessage = knowledgeBase.ErrorMessage,
            Labels = knowledgeBase.Labels,
            Note = knowledgeBase.Note,
            Problem = knowledgeBase.Problem,
            StepToReproduce = knowledgeBase.StepToReproduce,
            Title = knowledgeBase.Title,
            Workaround = knowledgeBase.Workaround,
            Id = knowledgeBase.Id
        });
    }

    [HttpPost]
    public async Task<IActionResult> EditKnowledgeBase([FromForm] KnowledgeBaseCreateRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _knowledgeBaseApiClient.PutKnowledgeBase(request.Id.Value, request);
        if (result)
        {
            return Ok();
        }
        return BadRequest();
    }

    private List<CategoryViewModel> BuildCategoryTree(List<CategoryViewModel> categories, int? parentId = null)
    {
        var tree = new List<CategoryViewModel>();
        foreach (var category in categories.Where(x => x.ParentId == parentId))
        {
            var node = new CategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
                ParentId = category.ParentId,
                NumberOfTickets = category.NumberOfTickets,
                SeoAlias = category.SeoAlias,
                SeoDescription = category.SeoDescription,
                SortOrder = category.SortOrder,
            };
            node.Children = BuildCategoryTree(categories, category.Id);
            tree.Add(node);
        }
        return tree;
    }

    private async Task SetCategoriesViewBag(int? selectedValue = null)
    {
        var categories = await _categoryApiClient.GetCategories();
        var items = categories.Select(i => new SelectListItem()
        {
            Text = i.Name,
            Value = i.Id.ToString(),
        }).ToList();

        items.Insert(0, new SelectListItem()
        {
            Value = null,
            Text = "--Chọn danh mục--"
        });
        ViewBag.Categories = new SelectList(items, "Value", "Text", selectedValue);
    }
}