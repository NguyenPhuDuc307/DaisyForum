using DaisyForum.ViewModels.Contents;
using DaisyForum.WebPortal.Models;
using DaisyForum.WebPortal.Services;
using Microsoft.AspNetCore.Mvc;

namespace DaisyForum.WebPortal.Controllers.Components;

public class SideBarViewComponent : ViewComponent
{
    private ICategoryApiClient _categoryApiClient;
    private IKnowledgeBaseApiClient _knowledgeBaseApiClient;

    public SideBarViewComponent(ICategoryApiClient categoryApiClient,
        IKnowledgeBaseApiClient knowledgeBaseApiClient)
    {
        _categoryApiClient = categoryApiClient;
        _knowledgeBaseApiClient = knowledgeBaseApiClient;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var categories = await _categoryApiClient.GetCategories();
        var treeNodes = TreeNode.ConvertToTreeNodes(categories);
        var popularKnowledgeBases = await _knowledgeBaseApiClient.GetPopularKnowledgeBases(5);
        var recentComments = await _knowledgeBaseApiClient.GetRecentComments(5);
        var viewModel = new SideBarViewModel()
        {
            TreeNodes = treeNodes,
            PopularKnowledgeBases = popularKnowledgeBases,
            RecentComments = recentComments
        };
        return View("Default", viewModel);
    }
}