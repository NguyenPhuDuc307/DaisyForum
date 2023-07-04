using DaisyForum.BackendServer.Extensions;
using DaisyForum.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DaisyForum.BackendServer.Controllers;

public class HomeController : Controller
{
    private readonly IRecaptchaExtension _recaptcha;

    public HomeController(IRecaptchaExtension recaptcha)
    {
        _recaptcha = recaptcha;
    }
    public IActionResult Index()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpGet]
    public async Task<JsonResult> Verify(string token)
    {
        var verified = await _recaptcha.VerifyAsync(token);
        return Json(verified);
    }
}