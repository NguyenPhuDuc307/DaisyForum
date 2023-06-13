using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DaisyForum.WebPortal.Models;
using DaisyForum.WebPortal.Services;
using DaisyForum.WebPortal.Extensions;
using Newtonsoft.Json;
using DaisyForum.ViewModels.Contents;

namespace DaisyForum.WebPortal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IKnowledgeBaseApiClient _knowledgeBaseApiClient;
        private readonly IUserApiClient _userApiClient;
        private readonly ILabelApiClient _labelApiClient;

        public HomeController(ILogger<HomeController> logger,
            ILabelApiClient labelApiClient,
            IKnowledgeBaseApiClient knowledgeBaseApiClient,
            IUserApiClient userApiClient)
        {
            _logger = logger;
            _labelApiClient = labelApiClient;
            _knowledgeBaseApiClient = knowledgeBaseApiClient;
            _userApiClient = userApiClient;
        }

        public async Task<IActionResult> Index()
        {
            var latestKbs = await _knowledgeBaseApiClient.GetLatestKnowledgeBases(12);
            var popularKbs = await _knowledgeBaseApiClient.GetPopularKnowledgeBases(12);

            var labels = await _labelApiClient.GetPopularLabels(100);
            var viewModel = new HomeViewModel()
            {
                LatestKnowledgeBases = latestKbs,
                PopularKnowledgeBases = popularKbs,
                PopularLabels = labels
            };

            if (User.Identity.IsAuthenticated)
            {
                string userId = User.GetUserId();

                // Kiểm tra xem cookie có tồn tại không
                if (Request.Cookies.TryGetValue("suggestedKbs", out string suggestedKbsJson))
                {
                    var suggestedKbs = JsonConvert.DeserializeObject<List<KnowledgeBaseQuickViewModel>>(suggestedKbsJson);
                    viewModel.SuggestedKnowledgeBases = suggestedKbs;
                }
                else
                {
                    var suggestedKbs = await _userApiClient.GetKnowledgeSuggested(userId, 10);
                    viewModel.SuggestedKnowledgeBases = suggestedKbs;

                    // Lưu danh sách suggestedKbs vào cookie
                    var options = new CookieOptions
                    {
                        Expires = DateTime.UtcNow.AddDays(7),
                        IsEssential = true
                    };
                    var suggestedKbsJsonSet = JsonConvert.SerializeObject(suggestedKbs);
                    Response.Cookies.Append("suggestedKbs", suggestedKbsJsonSet, options);
                }
            }

            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}